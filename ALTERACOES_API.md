# PetCare+ API – alterações para o portal do cliente, agenda e financeiro

## 1. Antes de rodar: gerar a migration

As mudanças criam tabelas/colunas novas (Vacinas, ConfiguracoesLoja, senha do cliente, peso/sexo/observações do pet,
observações do agendamento, vínculo `AgendamentoId`/`ServicoId` no lançamento com índice único).

No Visual Studio (Package Manager Console, projeto PetShop.API):

```
Add-Migration PortalClienteAgendaVacinas
Script-Migration -From SepararPorLojaESemCascata   # gera o SQL para rodar no banco da nuvem
# ou: Update-Database
```

Todas as colunas novas em tabelas existentes são opcionais, então os dados atuais não são afetados.

## 2. Segurança / isolamento

- Rotas administrativas usam a policy `Administrador` (token do admin; tokens antigos continuam válidos).
- Rotas do portal usam a policy `Cliente`. O token do cliente tem `clienteId` + `petshopId` e **não** tem `nameid`,
  então um cliente nunca acessa rota de admin (recebe 403).
- Loja e cliente vêm sempre do JWT; o corpo da requisição não é usado como prova de permissão.
- Registro de outra loja/cliente → 404 (não revela que existe). Conta de cliente excluída/inativa → 403.
- Erros: `{ "message": "..." }` (o campo `mensagem` continua vindo igual, por compatibilidade).
  Validação de campos: `{ "message": "...", "errors": { "Campo": ["..."] } }`.
- Listagens vazias retornam `200 []` (inclusive `BuscarPorPet`, `BuscarPorCliente`, `BuscarPetPorIdCliente`, que antes davam 404).

## 3. Autenticação do cliente

| Método | Rota | Observação |
|---|---|---|
| POST | /api/cliente/autenticacao/login | `{ token, cliente: { id, nome, sobrenome, email } }` |
| POST | /api/cliente/autenticacao/registrar | 201 com o mesmo formato do login (login automático) |
| GET | /api/cliente/autenticacao/petshops | lista `{ id, nomeLoja, cidade, estado }` |

Cadastro: campo opcional `petshopId`. Pode ser omitido quando só existe um petshop, ou quando o admin já cadastrou o
cliente com o mesmo CPF/e-mail (a conta "assume" esse cadastro, mantendo pets e histórico). Com mais de uma loja e sem
cadastro prévio, a API pede o `petshopId`. E-mail de login é único (409 se repetido). Senha com BCrypt, nunca retornada.

## 4. Portal do cliente (token de cliente)

- `GET /api/Cliente/MeuPerfil`, `PUT /api/Cliente/EditarMeuPerfil` (endereço em `endereco { cep, logradouro, numero, bairro, cidade, uf }`)
- `GET /api/Pet/MeusPets`, `POST /api/Pet/CriarMeuPet`, `PUT /api/Pet/EditarMeuPet`, `DELETE /api/Pet/ExcluirMeuPet/{id}`
  (exclusão lógica; bloqueada se o pet tiver agendamento futuro em aberto)
- `GET /api/Servico/ListarServicosAtivos` (cliente ou admin)
- `GET /api/Agendamento/MeusAgendamentos`
- `GET /api/Agendamento/HorariosDisponiveis?data=2026-10-05&servicoId=2[&agendamentoId=15]` → `["08:00","08:30",...]`
  (cliente ou admin; dia fechado/lotado → `[]`)
- `POST /api/Agendamento/CriarMeuAgendamento`, `PUT /api/Agendamento/EditarMeuAgendamento`
  → **409** se a vaga foi ocupada nesse meio-tempo
- `PATCH /api/Agendamento/CancelarMeuAgendamento/{id}` (status `Cancelado`, sem exclusão física)
- `GET /api/Vacina/MinhasVacinas`

## 5. Agenda: disponibilidade e reserva atômica

Configuração por petshop (admin): `GET /api/Configuracao/ObterConfiguracao`, `PUT /api/Configuracao/EditarConfiguracao`

```json
{
  "horaAbertura": "08:00", "horaFechamento": "18:00",
  "inicioIntervalo": "12:00", "fimIntervalo": "13:00",
  "intervaloEntreHorariosMinutos": 30,
  "capacidadeSimultanea": 1,
  "antecedenciaMinimaHoras": 2,
  "diasFuncionamento": [1,2,3,4,5,6],
  "metaAgendamentosMensal": 120,
  "fusoHorario": "America/Sao_Paulo"
}
```

Sem configuração cadastrada valem esses padrões (sem meta). Fuso: America/Sao_Paulo.
Criação/edição (portal e admin) roda dentro de uma transação com trava exclusiva da agenda da loja (`sp_getapplock`)
e revalida a capacidade no banco: duas reservas simultâneas para a última vaga → uma grava, a outra recebe 409.
O admin pode agendar fora do expediente (só a capacidade é validada); o portal exige dia/horário de funcionamento,
data futura e respeita a antecedência mínima para reagendar/cancelar.

## 6. Conclusão automática e receita única

- Tarefa no servidor (`ConclusaoAutomaticaWorker`, a cada 60 s) conclui `Pendente`/`Confirmado` com `dataHora <= agora`.
  Configurável em `appsettings.json` → `ConclusaoAutomatica`. **No Azure App Service, ative "Always On"**, senão o app
  dorme sem acessos e a tarefa para.
- A conclusão é um `UPDATE` condicional (relê status e horário no banco) e não conclui cancelado/reagendado/futuro.
- A receita é criada na mesma transação, com `agendamentoId` e `servicoId`; índice único em `Lancamentos.AgendamentoId`
  garante no máximo uma receita por agendamento, mesmo concluindo várias vezes.
- Admin: `CriarAgendamento` com data no passado grava `Concluido` + receita. `AlterarStatus` / `EditarAgendamento`
  para "Concluido" usam o mesmo fluxo.
- `CriarLancamento` com um `agendamentoId` que já tem receita devolve a existente (não duplica).

**Frontend:** remover a criação de receita no navegador ao concluir. Enquanto isso não sai, enviar `agendamentoId`
no lançamento evita duplicidade.

Atenção na primeira publicação: agendamentos antigos ainda `Pendente`/`Confirmado` com data passada serão concluídos
e ganharão receita. Se não quiser isso, ajuste o status deles antes de publicar.

## 7. Estoque, financeiro, vacinas, meta

- Estoque e financeiro já eram persistidos e isolados por loja; lançamento ganhou `agendamentoId` e `servicoId`.
- Vacinas admin: `GET ListarVacinas`, `GET BuscarVacinaPorId/{id}`, `POST CriarVacina`, `PUT EditarVacina`,
  `DELETE ExcluirVacina/{id}` em `/api/Vacina` (resposta com `petNome` e `clienteNome`, datas `yyyy-MM-dd`).
- Meta: `GET /api/Configuracao/ObterMeta`, `PUT /api/Configuracao/DefinirMeta` `{ "metaMensal": 120 }`
  (`null` remove). `Dashboard/Stats` → `metaMensal.{3m,6m,1a}.meta` é `null` sem meta ("Meta não definida") e
  `realizado` = agendamentos não cancelados no período.
