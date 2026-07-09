using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PawFeeder.Models
{
    public class Mascota
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")] // Mapea directo a tu columna con guion bajo
        public int UsuarioId { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("raza")]
        public string Raza { get; set; } = string.Empty;

        [Column("edad_anos")] // Mapea a edad_anos
        public byte EdadAnos { get; set; }

        [Column("peso_kg")] // Mapea a peso_kg
        public decimal PesoKg { get; set; }

        [Column("tamano")]
        public string Tamano { get; set; } = "mediano";

        [Column("activa")]
        public bool Activa { get; set; } = true;

        [Column("foto_uri")] // Mapea a foto_uri
        public string? FotoUri { get; set; }
    }
}