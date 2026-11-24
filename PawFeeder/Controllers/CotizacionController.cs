using Microsoft.AspNetCore.Mvc;
using PawFeeder.Models;
using PawFeeder.Services;

namespace PawFeeder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotizacionController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly ILogger<CotizacionController> _logger;

        public CotizacionController(EmailService emailService, ILogger<CotizacionController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarCotizacion([FromBody] CotizacionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Correo))
            {
                return BadRequest(new { mensaje = "El correo es obligatorio." });
            }

            string mensaje = $@"
                <div style='font-family:Arial,sans-serif;max-width:560px;margin:0 auto;padding:24px'>
                  <h2 style='color:#2C5F8A;margin-bottom:8px'>Cotización PawFeeder</h2>
                  <p>Gracias por solicitar una cotización.</p>
                  <table style='border-collapse:collapse;width:100%'>
                    <tr><td style='padding:8px 0;color:#5F6E7D'><b>Modelo</b></td><td style='padding:8px 0'>PawFeeder grande · acero inoxidable</td></tr>
                    <tr><td style='padding:8px 0;color:#5F6E7D'><b>Contenedor</b></td><td style='padding:8px 0'>{System.Net.WebUtility.HtmlEncode(request.Contenedor)} (3.0 kg)</td></tr>
                    <tr><td style='padding:8px 0;color:#5F6E7D'><b>Material</b></td><td style='padding:8px 0'>{System.Net.WebUtility.HtmlEncode(request.Material)}</td></tr>
                    <tr><td style='padding:8px 0;color:#5F6E7D'><b>Cantidad</b></td><td style='padding:8px 0'>{request.Cantidad}</td></tr>
                    <tr><td style='padding:12px 0;color:#1E3A5C;font-size:18px'><b>Total estimado</b></td><td style='padding:12px 0;font-size:18px;color:#2C5F8A'><b>$ {request.Total} MXN</b></td></tr>
                  </table>
                  <p style='color:#5F6E7D;font-size:14px;margin-top:20px'>Gracias por confiar en <b>PawFeeder</b>.</p>
                </div>";

            try
            {
                await _emailService.EnviarCorreoAsync(
                    request.Correo,
                    "Cotización PawFeeder",
                    mensaje);

                return Ok(new
                {
                    mensaje = "Correo enviado correctamente."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando cotización a {Correo}", request.Correo);

                return BadRequest(new
                {
                    mensaje = "No se pudo enviar el correo. Intenta de nuevo."
                });
            }
        }
    }
}