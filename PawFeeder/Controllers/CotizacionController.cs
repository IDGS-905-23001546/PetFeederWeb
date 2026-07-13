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

        public CotizacionController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarCotizacion([FromBody] CotizacionRequest request)
        {
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

            await _emailService.EnviarCorreoAsync(
                request.Correo,
                "Cotización PawFeeder",
                mensaje);

            return Ok(new
            {
                mensaje = "Correo enviado correctamente."
            });
        }
    }
}