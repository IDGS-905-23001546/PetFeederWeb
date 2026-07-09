using System.ComponentModel.DataAnnotations;

namespace PawFeeder.Models
{
    public class Dispensador
    {
        [Key]
        public string Id { get; set; } = string.Empty; // Ej. PW-ARD-905X

        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        // Monitoreo: Nivel de comida en porcentaje (calculado por el sensor ultrasónico)
        [Range(0, 100)]
        public int NivelAlimentoPorcentaje { get; set; }

        [Required]
        public string Estado { get; set; } = "Activo"; // Activo, Inactivo, Mantenimiento

        public DateTime UltimaConexion { get; set; } = DateTime.Now;
    }
}
