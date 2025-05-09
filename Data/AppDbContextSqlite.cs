﻿using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Data;

public class AppDbContextSqlite : DbContext
{
    public AppDbContextSqlite(DbContextOptions<AppDbContextSqlite> options) : base(options) { }

    public DbSet<Alphadigi> Alphadigi { get; set; }
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
        modelBuilder.Entity<Alphadigi>()
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

    }
}
