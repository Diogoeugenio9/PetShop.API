# API PetShop

API REST para gestão de pet shops, com área administrativa e portal do cliente. Projeto de TCC do curso de Sistemas de Informação (UNIP), publicado no Azure App Service.

## Funcionalidades

- **Várias lojas no mesmo sistema:** cada administrador vê apenas os dados da própria loja
- **Dois tipos de acesso com JWT:** administrador (gestão da loja) e cliente (portal)
- **Portal do cliente:** cadastro e login, perfil, pets, agendamentos online e carteira de vacinação
- **Agendamento sem conflito:** horários disponíveis calculados pelo expediente, intervalo e capacidade da loja; duas reservas simultâneas para a mesma vaga não são gravadas
- **Conclusão automática:** uma tarefa em segundo plano conclui os atendimentos cujo horário já passou e gera a receita no financeiro, sem duplicidade
- **Gestão:** clientes, pets, serviços, vacinas, estoque, lançamentos financeiros, metas e dashboard

## Tecnologias

C# · ASP.NET Core (.NET 8) · Entity Framework Core · SQL Server · AutoMapper · JWT · Docker · Azure App Service

## Arquitetura

```
Controllers  →  Services  →  Repositories  →  EF Core (Code First + Migrations)  →  SQL Server
```

- Isolamento por loja com filtros globais do EF Core, a partir do usuário do token
- Exclusão lógica para clientes, pets, serviços e produtos
- Erros padronizados no formato `{ "message": "..." }`

## Como rodar

1. Configure a connection string e a chave JWT com User Secrets:
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<sua connection string>" --project PetShop.API
   dotnet user-secrets set "Jwt:Key" "<chave com pelo menos 32 caracteres>" --project PetShop.API
   ```
2. Crie o banco:
   ```bash
   dotnet ef database update --project PetShop.API
   ```
3. Rode a API e acesse o Swagger em `/swagger`:
   ```bash
   dotnet run --project PetShop.API
   ```

## Principais rotas

| Área | Exemplos |
| --- | --- |
| Autenticação | `POST /api/admin/autenticacao/login`, `POST /api/cliente/autenticacao/registrar` |
| Portal do cliente | `GET /api/Pet/MeusPets`, `GET /api/Agendamento/HorariosDisponiveis`, `POST /api/Agendamento/CriarMeuAgendamento` |
| Administração | `/api/Cliente`, `/api/Pet`, `/api/Servico`, `/api/Agendamento`, `/api/Vacina`, `/api/Produto`, `/api/Lancamento` |
| Indicadores | `GET /api/Dashboard/Stats`, `/api/Configuracao` |

A lista completa de rotas está no Swagger e em [ALTERACOES_API.md](ALTERACOES_API.md).

## Autor

Diogo Eugênio · [LinkedIn](https://www.linkedin.com/in/diogo-eugenio-dev)
