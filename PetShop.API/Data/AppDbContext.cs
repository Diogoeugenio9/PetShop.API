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
        public DbSet<PetModelo> PetsModelo { get; set; }
        public DbSet<ServicoModel> Servicos { get; set; }
        public DbSet<AgendamentoModel> Agendamentos { get; set; }
        public DbSet<AdministradorModel> Administradores { get; set; }
        public DbSet<ProdutoModel> Produtos { get; set; }
        public DbSet<LancamentoModel> Lancamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ServicoModel>()
                .Property(s => s.Preco)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<ProdutoModel>()
                .Property(p => p.PrecoUnitario)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<LancamentoModel>()
                .Property(l => l.Valor)
                .HasColumnType("decimal(10,2)");
        }
    }
}