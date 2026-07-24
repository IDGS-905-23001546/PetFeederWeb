using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PawFeeder.Models
{
    [Table("inventario_componentes")]
    public class Componente
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Column("stock")]
        public int Stock { get; set; }

        [Column("unidad_medida")]
        public string UnidadMedida { get; set; } = "pza";
    }
}
