using Microsoft.EntityFrameworkCore;
using PetShop.API.Models;

namespace PetShop.API.Data
{
    public class AppDbContext : DbContext

    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<ClienteModel> Clientes { get; set; }
        public DbSet<PetModelo> PetsModel { get; set; }    //Talvez devo mudar o nome para PetModelo
        //public DbSet<LoginModel> Logins { get; set; }


    }
}
