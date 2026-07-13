namespace PawFeeder.Models
{
    public class CotizacionRequest
    {
        public string Correo { get; set; } = string.Empty;

        public string Contenedor { get; set; } = string.Empty;

        public string Material { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        public decimal Total { get; set; }
    }
}