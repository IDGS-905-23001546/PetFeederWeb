using System.ComponentModel.DataAnnotations;

namespace PawFeeder.Models
{
    public class Dispensador
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public int UsuarioId { get; set; }


        [Required]
        public string Nombre { get; set; } = string.Empty;


        [Required]
        public string CodigoUnico { get; set; } = string.Empty;


        public string FirmwareVersion { get; set; } = "v1.0.0";


        public string Estado { get; set; } = "offline";


        // Cambiar de int a byte
        public byte BateriaPercent { get; set; }


        // Cambiar de int a byte
        public byte NivelTolvaPct { get; set; }


        public string? SsidWifi { get; set; }


        public bool Activo { get; set; } = true;


        public DateTime? LastPingAt { get; set; }


        public DateTime CreatedAt { get; set; }


        public DateTime UpdatedAt { get; set; }



        public Usuario? Usuario { get; set; }
    }
}