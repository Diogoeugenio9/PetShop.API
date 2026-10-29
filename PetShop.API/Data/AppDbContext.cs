using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PetShop.API.Models;
using PetShop.API.Utils;

namespace PetShop.API.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<ClienteModel> Clientes { get; set; }
        public DbSet<PetModelo> PetsModelo { get; set; }
        public DbSet<ServicoModel> Servicos { get; set; }
        public DbSet<AgendamentoModel> Agendamentos { get; set; }
        public DbSet<AdministradorModel> Administradores { get; set; }
        public DbSet<ProdutoModel> Produtos { get; set; }
        public DbSet<LancamentoModel> Lancamentos { get; set; }
        public DbSet<VacinaModel> Vacinas { get; set; }
        public DbSet<ConfiguracaoLojaModel> ConfiguracoesLoja { get; set; }
        public DbSet<AssinaturaModel> Assinaturas { get; set; }
        public DbSet<EventoPagamentoModel> EventosPagamento { get; set; }

        public int AdministradorIdAtual
            => _httpContextAccessor?.HttpContext?.User?.ObterAdministradorId() ?? 0;

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

            modelBuilder.Entity<ClienteModel>()
                .HasOne<AdministradorModel>()
                .WithMany()
                .HasForeignKey(c => c.AdministradorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServicoModel>()
                .HasOne<AdministradorModel>()
                .WithMany()
                .HasForeignKey(s => s.AdministradorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProdutoModel>()
                .HasOne<AdministradorModel>()
                .WithMany()
                .HasForeignKey(p => p.AdministradorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LancamentoModel>()
                .HasOne<AdministradorModel>()
                .WithMany()
                .HasForeignKey(l => l.AdministradorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PetModelo>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Pets)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AgendamentoModel>()
                .HasOne(a => a.Pet)
                .WithMany()
                .HasForeignKey(a => a.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AgendamentoModel>()
                .HasOne(a => a.Servico)
                .WithMany(s => s.Agendamentos)
                .HasForeignKey(a => a.ServicoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PetModelo>()
                .Property(p => p.Peso)
                .HasColumnType("decimal(6,2)");

            modelBuilder.Entity<LancamentoModel>()
                .HasIndex(l => l.AgendamentoId)
                .IsUnique()
                .HasFilter("[AgendamentoId] IS NOT NULL");

            modelBuilder.Entity<VacinaModel>()
                .HasOne(v => v.Pet)
                .WithMany()
                .HasForeignKey(v => v.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VacinaModel>()
                .Property(v => v.NomeVacina)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<ConfiguracaoLojaModel>()
                .HasOne<AdministradorModel>()
                .WithMany()
                .HasForeignKey(c => c.AdministradorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConfiguracaoLojaModel>()
                .HasIndex(c => c.AdministradorId)
                .IsUnique();

            modelBuilder.Entity<ConfiguracaoLojaModel>()
                .Property(c => c.DiasFuncionamento)
                .HasMaxLength(20);

            modelBuilder.Entity<AssinaturaModel>()
                .HasOne<AdministradorModel>()
                .WithMany()
                .HasForeignKey(a => a.AdministradorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssinaturaModel>()
                .Property(a => a.Valor)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<AssinaturaModel>()
                .Property(a => a.Plano)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<AssinaturaModel>()
                .Property(a => a.Status)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<AssinaturaModel>()
                .Property(a => a.CheckoutId)
                .HasMaxLength(100);

            modelBuilder.Entity<AssinaturaModel>()
                .Property(a => a.AssinaturaExternaId)
                .HasMaxLength(100);

            modelBuilder.Entity<AssinaturaModel>()
                .HasIndex(a => a.CheckoutId)
                .IsUnique()
                .HasFilter("[CheckoutId] IS NOT NULL");

            modelBuilder.Entity<EventoPagamentoModel>()
                .Property(e => e.EventoId)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<EventoPagamentoModel>()
                .Property(e => e.Evento)
                .HasMaxLength(100);

            modelBuilder.Entity<EventoPagamentoModel>()
                .HasIndex(e => e.EventoId)
                .IsUnique();

            modelBuilder.Entity<ClienteModel>()
                .HasQueryFilter(c => c.AdministradorId == AdministradorIdAtual);

            modelBuilder.Entity<ServicoModel>()
                .HasQueryFilter(s => s.AdministradorId == AdministradorIdAtual);

            modelBuilder.Entity<ProdutoModel>()
                .HasQueryFilter(p => p.AdministradorId == AdministradorIdAtual);

            modelBuilder.Entity<LancamentoModel>()
                .HasQueryFilter(l => l.AdministradorId == AdministradorIdAtual);

            modelBuilder.Entity<PetModelo>()
                .HasQueryFilter(p => p.Cliente.AdministradorId == AdministradorIdAtual);

            modelBuilder.Entity<AgendamentoModel>()
                .HasQueryFilter(a => a.Pet.Cliente.AdministradorId == AdministradorIdAtual);

            modelBuilder.Entity<VacinaModel>()
                .HasQueryFilter(v => v.Pet.Cliente.AdministradorId == AdministradorIdAtual);

            modelBuilder.Entity<ConfiguracaoLojaModel>()
                .HasQueryFilter(c => c.AdministradorId == AdministradorIdAtual);

            modelBuilder.Entity<AssinaturaModel>()
                .HasQueryFilter(a => a.AdministradorId == AdministradorIdAtual);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            PreencherLoja();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            PreencherLoja();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void PreencherLoja()
        {
            foreach (var entry in ChangeTracker.Entries<IPertenceALoja>())
            {
                if (entry.State == EntityState.Added)
                {
                    var administradorId = AdministradorIdAtual;
                    if (administradorId != 0)
                        entry.Entity.AdministradorId = administradorId;
                    else if (entry.Entity.AdministradorId == 0)
                        throw new InvalidOperationException("Não há loja definida para vincular o registro.");
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(IPertenceALoja.AdministradorId)).IsModified = false;
                }
            }
        }
    }
}
