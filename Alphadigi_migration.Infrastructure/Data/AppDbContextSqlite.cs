using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Infrastructure.Data;


public class AppDbContextSqlite : DbContext
{
    public AppDbContextSqlite(DbContextOptions<AppDbContextSqlite> options) : base(options) { }

    public DbSet<Domain.EntitiesNew.Alphadigi> Alphadigi { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<PlacaLida> PlacaLida { get; set; }
    public DbSet<MensagemDisplay> MensagemDisplay { get; set; }
    public DbSet<Condominio> Condominio { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=database.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity < Alphadigi_migration.Domain.EntitiesNew.Alphadigi> ()
            .HasOne(a => a.Area)
            .WithMany()
            .HasForeignKey(a => a.AreaId);

        modelBuilder.Entity<PlacaLida>()
            .HasOne(a => a.Alphadigi)
            .WithMany()
            .HasForeignKey(a => a.AlphadigiId);

        modelBuilder.Entity<PlacaLida>()
            .HasOne(a => a.Area)
            .WithMany()
            .HasForeignKey(a => a.AreaId);

        modelBuilder.Entity<MensagemDisplay>()
            .HasOne(a => a.Alphadigi)
            .WithMany()
            .HasForeignKey(a => a.AlphadigiId);

        modelBuilder.Entity<Condominio>(entity =>
        {
            entity.OwnsOne(e => e.Cnpj, cnpj =>
            {
                cnpj.Property(c => c.Numero)
                .HasColumnName("CNPJ")
                .HasMaxLength(14);
            });
        });

        //modelBuilder.Entity<PlacaLida>(entity =>
        //{
        //    entity.OwnsOne(e => e.Placa, placa =>
        //    {

        //        placa.Property(p => p.Numero)
        //        .HasColumnName("PLACA")
        //        .IsRequired()
        //        .HasMaxLength(10);

        //    });
        //});

        modelBuilder.Entity<PlacaLida>(entity =>
        {
            entity.Property(e => e.Placa)
                .HasColumnName("PLACA")
                .IsRequired()
                .HasMaxLength(10);

            entity.Ignore(e => e.Placa); // Ignorar a propriedade do domínio
        });

        modelBuilder.Entity<MensagemDisplay>(entity =>
        {
            entity.OwnsOne(e => e.Placa, placa =>
            {
                placa.Property(p => p.Numero)
                .HasColumnName("PLACA")
                .HasMaxLength(10);
            });
        });

    }
}
