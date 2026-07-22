using System.ComponentModel.DataAnnotations;

namespace PawFeeder.Models
{
    public class Usuario
    {

        [Key]
        public int Id { get; set; }


        [Required]
        public string Nombre { get; set; } = string.Empty;


        [Required]
        public string Email { get; set; } = string.Empty;


        public string? Telefono { get; set; }



        public string PasswordHash { get; set; } = string.Empty;



        public bool Verificado { get; set; }


        public bool Activo { get; set; }



        public DateTime CreatedAt { get; set; }


        public DateTime UpdatedAt { get; set; }



        // Relaciones

        public ICollection<Dispensador> Dispensadores { get; set; }
            = new List<Dispensador>();


        public ICollection<Mascota> Mascotas { get; set; }
            = new List<Mascota>();

    }
}