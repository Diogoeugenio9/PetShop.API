using Microsoft.IdentityModel.Tokens;
using PetShop.API.Dto.LoginDto;
using PetShop.API.Dto.RegistroDto;
using PetShop.API.Models;
using PetShop.API.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PetShop.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAdministradorRepository _repo;
        private readonly IConfiguration _configuration;

        public AuthService(IAdministradorRepository repo, IConfiguration configuration)
        {
            _repo = repo;
            _configuration = configuration;
        }

        public string Login(LoginDto dto)
        {
            var admin = _repo.BuscarPorEmail(dto.Email);
            if (admin == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, admin.SenhaHash))
                return null;

            return GerarToken(admin);
        }

        public string Registrar(RegistroDto dto)
        {
            if (_repo.ExisteEmail(dto.Email))
                return null;

            var admin = new AdministradorModel
            {
                NomeProprietario = dto.NomeProprietario,
                NomeLoja = dto.NomeLoja,
                Telefone = dto.Telefone,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                Email = dto.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha)
            };

            _repo.Cadastrar(admin);
            return GerarToken(admin);
        }

        public string GerarToken(AdministradorModel admin)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Email, admin.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
