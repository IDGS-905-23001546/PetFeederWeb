using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PawFeeder.Models
{
    [Table("horarios")]
    public class Horario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("mascota_id")]
        public int? MascotaId { get; set; } // Opcional (puede ser NULL)

        [Column("dispensador_id")]
        public int? DispensadorId { get; set; } // Opcional (puede ser NULL)

        [Required]
        [StringLength(50)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("icono")]
        public string Icono { get; set; } = "sun";

        [Required]
        [StringLength(10)]
        [Column("hora")]
        public string Hora { get; set; } = string.Empty; // Ej: "07:30 AM"

        [Required]
        [Column("lunes")]
        public bool Lunes { get; set; }

        [Required]
        [Column("martes")]
        public bool Martes { get; set; }

        [Required]
        [Column("miercoles")]
        public bool Miercoles { get; set; }

        [Required]
        [Column("jueves")]
        public bool Jueves { get; set; }

        [Required]
        [Column("viernes")]
        public bool Viernes { get; set; }

        [Required]
        [Column("sabado")]
        public bool Sabado { get; set; }

        [Required]
        [Column("domingo")]
        public bool Domingo { get; set; }

        [Required]
        [Column("porcion_gramos")]
        public decimal PorcionGramos { get; set; } = 100.0m;

        [Required]
        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}