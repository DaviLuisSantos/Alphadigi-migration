using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using Alphadigi_migration.Models;
namespace Alphadigi_migration.Data
{
    public class AppDbContextFirebird:DbContext
    {
        public AppDbContextFirebird(DbContextOptions<AppDbContextFirebird> options) : base(options)
        {
            

        }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "Server=localhost;Database=D:\\AcessoLinear\\Dados\\BANCODEDADOS.fdb;User=SYSDBA;Password=masterkey;";
                optionsBuilder.UseFirebird(connectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
