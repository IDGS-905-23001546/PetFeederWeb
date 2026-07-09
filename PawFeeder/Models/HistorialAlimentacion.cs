using System.ComponentModel.DataAnnotations;

namespace PawFeeder.Models
{
    public class HistorialAlimentacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DispensadorId { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string HorarioEtiqueta { get; set; } = string.Empty; // Ej. Desayuno, Almuerzo

        [Required]
        public int PorcionGramos { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;
    }
}
