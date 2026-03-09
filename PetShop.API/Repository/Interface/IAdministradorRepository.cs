using PetShop.API.Models;

namespace PetShop.API.Repository
{
    public interface IAdministradorRepository
    {
        AdministradorModel BuscarPorEmail(string email);
        AdministradorModel BuscarPorId(int id);
        void Cadastrar(AdministradorModel administrador);
        bool ExisteEmail(string email);
    }
}
