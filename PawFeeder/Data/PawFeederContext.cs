using Microsoft.EntityFrameworkCore;
using PawFeeder.Models; // <-- Asegúrate de que apunte a tu carpeta de modelos real

namespace PawFeeder.Data
{
    public class PawFeederContext : DbContext
    {
        public PawFeederContext(DbContextOptions<PawFeederContext> options) : base(options)
        {
        }

        // Apuntamos a las clases reales de tu carpeta Models
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Dispensador> Dispensadores { get; set; }
        public DbSet<HistorialAlimentacion> HistorialAlimentaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo exacto a tus tablas en minúsculas de SQL Server
            modelBuilder.Entity<Mascota>().ToTable("mascotas");
            modelBuilder.Entity<Dispensador>().ToTable("dispensadores");
            modelBuilder.Entity<HistorialAlimentacion>().ToTable("dispensaciones");
        }
    }
}