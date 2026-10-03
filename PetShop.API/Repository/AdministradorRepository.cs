using PetShop.API.Data;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;

namespace PetShop.API.Repository
{
    public class AdministradorRepository : IAdministradorRepository
    {
        private readonly AppDbContext _context;

        public AdministradorRepository(AppDbContext context)
        {
            _context = context;
        }

        public AdministradorModel BuscarPorEmail(string email)
        {
            return _context.Administradores.FirstOrDefault(a => a.Email.ToLower() == email);
        }

        public AdministradorModel BuscarPorId(int id)
        {
            return _context.Administradores.FirstOrDefault(a => a.Id == id);
        }

        public void Cadastrar(AdministradorModel administrador)
        {
            _context.Administradores.Add(administrador);
            _context.SaveChanges();
        }

        public bool ExisteEmail(string email)
        {
            return _context.Administradores.Any(a => a.Email.ToLower() == email);
        }
    }
}
