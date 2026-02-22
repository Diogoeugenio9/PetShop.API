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
        public DbSet<PetModel> Pets { get; set; }
        //public DbSet<LoginModel> Logins { get; set; }


    }
}
