using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Data;

public class AppDbContextSqlite : DbContext
{
    public AppDbContextSqlite(DbContextOptions<AppDbContextSqlite> options) : base(options) { }

    public DbSet<Alphadigi> Alphadigi { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=database.db");
        }
    }
}
