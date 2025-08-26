using Microsoft.EntityFrameworkCore;
using Alphadigi_migration.Models;
using Alphadigi_migration.Domain.Entities;

namespace Alphadigi_migration.Infrastructure.Data;

public class AppDbContextFirebird : DbContext
{
    public AppDbContextFirebird(DbContextOptions<AppDbContextFirebird> options) : base(options)
    {
    }

    public DbSet<Veiculo> Veiculo { get; set; }
    public DbSet<Camera> Camera { get; set; }
    public DbSet<Area> Area { get; set; }
    public DbSet<Unidade> Unidade { get; set; }
    public DbSet<Acesso> Acesso { get; set; }
    public DbSet<Condominio> Condominio { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Camera>()
            .HasOne(c => c.Area)
            .WithMany()
            .HasForeignKey(c => c.IdArea);

        modelBuilder.Entity<Veiculo>()
         .HasOne(v => v.UnidadeNavigation)
         .WithMany()
         .HasForeignKey(v => v.Unidade)
         .HasPrincipalKey(u => u.Nome);

        modelBuilder.Entity<Veiculo>()
            .HasOne(v => v.Rota)
            .WithMany()
            .HasForeignKey(v => v.IdRota);

        modelBuilder.Entity<Camera>()
       .Property(c => c.FotoEvento)
       .HasColumnName("FOTO_EVENTO")
       .HasConversion(
            v => v.HasValue ? (v.Value ? 1 : 0) : 0,
            v => v == 1
        );

    }
}