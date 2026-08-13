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
                <h2>Cotización PawFeeder</h2>

                <p>Gracias por solicitar una cotización.</p>

                <table style='border-collapse:collapse;'>

                    <tr>
                        <td><b>Contenedor:</b></td>
                        <td>{request.Contenedor}</td>
                    </tr>

                    <tr>
                        <td><b>Material:</b></td>
                        <td>{request.Material}</td>
                    </tr>

                    <tr>
                        <td><b>Cantidad:</b></td>
                        <td>{request.Cantidad}</td>
                    </tr>

                    <tr>
                        <td><b>Total:</b></td>
                        <td>$ {request.Total} MXN</td>
                    </tr>

                </table>

                <br>

                <p>Gracias por confiar en <b>PawFeeder</b>.</p>
            ";

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