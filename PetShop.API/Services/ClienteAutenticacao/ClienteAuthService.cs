using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetShop.API.Data;
using PetShop.API.Dto.ClienteAutenticacao;
using PetShop.API.Models;
using PetShop.API.Utils;

namespace PetShop.API.Services.ClienteAutenticacao
{
    public class ClienteAuthService : IClienteAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public ClienteAuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ClienteAutenticadoDto> Login(ClienteLoginDto dto)
        {
            var email = NormalizarEmail(dto.Email);

            var contas = await _context.Clientes
                .IgnoreQueryFilters()
                .Where(c => c.Email == email && c.SenhaHash != null && !c.Excluido)
                .ToListAsync();

            var cliente = contas.FirstOrDefault(c => BCrypt.Net.BCrypt.Verify(dto.Senha ?? string.Empty, c.SenhaHash));
            if (cliente == null)
                return null;

            if (!cliente.Ativo)
                throw new RegraDeNegocioException("Sua conta está inativa. Fale com o petshop.");

            return Montar(cliente);
        }

        public async Task<ClienteAutenticadoDto> Registrar(ClienteRegistroDto dto)
        {
            var email = NormalizarEmail(dto.Email);
            var cpf = SomenteDigitos(dto.Cpf);

            if (!string.IsNullOrEmpty(cpf) && cpf.Length != 11)
                throw new RegraDeNegocioException("CPF inválido. Informe os 11 dígitos.");

            var emailEmUso = await _context.Clientes
                .IgnoreQueryFilters()
                .AnyAsync(c => c.Email == email && c.SenhaHash != null && !c.Excluido);

            if (emailEmUso)
                throw new ConflitoException("Já existe uma conta com este e-mail.");

            var temCpf = !string.IsNullOrEmpty(cpf);
            var cpfFormatado = temCpf ? FormatarCpf(cpf) : null;

            var existentes = await _context.Clientes
                .IgnoreQueryFilters()
                .Where(c => !c.Excluido
                            && (c.Email == email || (temCpf && (c.Cpf == cpf || c.Cpf == cpfFormatado))))
                .ToListAsync();

            var petshopId = await DefinirPetshop(dto.PetshopId, existentes);

            var cliente = existentes.FirstOrDefault(c => c.AdministradorId == petshopId);
            if (cliente != null && cliente.SenhaHash != null)
                throw new ConflitoException("Este CPF já possui conta no portal. Faça login.");

            if (cliente == null)
            {
                cliente = new ClienteModel
                {
                    AdministradorId = petshopId,
                    DataCadastro = DataHoraBrasil.Agora,
                    Ativo = true
                };
                _context.Clientes.Add(cliente);
            }

            cliente.Nome = dto.Nome?.Trim();
            cliente.Sobrenome = dto.Sobrenome?.Trim();
            cliente.Email = email;
            if (!string.IsNullOrEmpty(cpf))
                cliente.Cpf = cpf;
            if (!string.IsNullOrWhiteSpace(dto.Telefone))
                cliente.Telefone = dto.Telefone.Trim();
            cliente.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);

            await _context.SaveChangesAsync();
            return Montar(cliente);
        }

        public async Task<List<PetshopPublicoDto>> ListarPetshops()
        {
            return await _context.Administradores
                .AsNoTracking()
                .OrderBy(a => a.NomeLoja)
                .Select(a => new PetshopPublicoDto
                {
                    Id = a.Id,
                    NomeLoja = a.NomeLoja,
                    Cidade = a.Cidade,
                    Estado = a.Estado
                })
                .ToListAsync();
        }

        private async Task<int> DefinirPetshop(int? petshopInformado, List<ClienteModel> existentes)
        {
            if (petshopInformado.HasValue && petshopInformado.Value > 0)
            {
                var existe = await _context.Administradores.AnyAsync(a => a.Id == petshopInformado.Value);
                if (!existe)
                    throw new RegraDeNegocioException("Petshop não encontrado.");
                return petshopInformado.Value;
            }

            var lojasDoCadastro = existentes
                .Where(c => c.SenhaHash == null)
                .Select(c => c.AdministradorId)
                .Distinct()
                .ToList();

            if (lojasDoCadastro.Count == 1)
                return lojasDoCadastro[0];

            if (lojasDoCadastro.Count > 1)
                throw new RegraDeNegocioException("Encontramos seu cadastro em mais de um petshop. Informe o petshopId.");

            var lojas = await _context.Administradores.Select(a => a.Id).Take(2).ToListAsync();
            if (lojas.Count == 1)
                return lojas[0];

            if (lojas.Count == 0)
                throw new RegraDeNegocioException("Nenhum petshop cadastrado.");

            throw new RegraDeNegocioException("Informe o petshop (petshopId) em que deseja se cadastrar.");
        }

        private ClienteAutenticadoDto Montar(ClienteModel cliente) => new ClienteAutenticadoDto
        {
            Token = GerarToken(cliente),
            Cliente = new ClienteResumoDto
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Sobrenome = cliente.Sobrenome,
                Email = cliente.Email
            }
        };

        private string GerarToken(ClienteModel cliente)
        {
            // Sem ClaimTypes.NameIdentifier: esse claim identifica administradores.
            var claims = new[]
            {
                new Claim(UsuarioClaims.ClienteId, cliente.Id.ToString()),
                new Claim(UsuarioClaims.PetshopId, cliente.AdministradorId.ToString()),
                new Claim(ClaimTypes.Email, cliente.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, $"{cliente.Nome} {cliente.Sobrenome}".Trim()),
                new Claim(ClaimTypes.Role, Politicas.Cliente)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string NormalizarEmail(string email)
            => (email ?? string.Empty).Trim().ToLowerInvariant();

        private static string SomenteDigitos(string valor)
            => new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());

        private static string FormatarCpf(string cpf)
            => cpf.Length == 11 ? $"{cpf[..3]}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}" : cpf;
    }
}
