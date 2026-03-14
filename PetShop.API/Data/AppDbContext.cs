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
        public DbSet<PetModel> PetsModelo { get; set; }    
        public DbSet<ServicoModel> Servicos { get; set; }
        public DbSet<AgendamentoModel> Agendamentos { get; set; }
        public DbSet<AdministradorModel> Administradores { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
            // 🔑 Configuração do campo Preco
             modelBuilder.Entity<ServicoModel>() 
                .Property(s => s.Preco)
                .HasColumnType("decimal(10,2)");
        }


        }
}
