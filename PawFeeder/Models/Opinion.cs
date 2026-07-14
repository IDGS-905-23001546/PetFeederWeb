using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PawFeeder.Models
{
    [Table("opiniones")]
    public class Opinion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nombre_usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("detalles_mascota")]
        public string DetallesMascota { get; set; } = string.Empty;

        [Required]
        [Column("calificacion")]
        public int Calificacion { get; set; }

        [Required]
        [Column("comentario")]
        public string Comentario { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("fecha")]
        public string Fecha { get; set; } = string.Empty;
    }
}