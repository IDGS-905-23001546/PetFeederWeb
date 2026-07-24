using Microsoft.EntityFrameworkCore;
using PawFeeder.Models;

namespace PawFeeder.Data
{
    public class PawFeederContext : DbContext
    {
        public PawFeederContext(
            DbContextOptions<PawFeederContext> options
        ) : base(options)
        {

        }


        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Dispensador> Dispensadores { get; set; }

        public DbSet<Mascota> Mascotas { get; set; }

        public DbSet<Horario> Horarios { get; set; }

        public DbSet<Opinion> Opiniones { get; set; }


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Dispensador>()
                .HasOne(d => d.Usuario)
                .WithMany(u => u.Dispensadores)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}