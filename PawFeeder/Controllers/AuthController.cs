using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models;
using PawFeeder.Services;

namespace PawFeeder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, CodigoVerificacion> Codigos = new();

        private readonly PawFeederContext _context;
        private readonly EmailService _emailService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(PawFeederContext context, EmailService emailService, ILogger<AuthController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegistroRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { mensaje = "Nombre, correo y contraseña son obligatorios." });
            }

            var email = request.Email.Trim().ToLowerInvariant();

            var existe = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == email);
            if (existe)
            {
                return BadRequest(new { mensaje = "Este correo ya está registrado." });
            }

            var usuario = new Usuario
            {
                Nombre = request.Nombre.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Rol = "cliente",
                Verificado = false,
                Activo = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var codigo = GenerarCodigo();
            GuardarCodigo(email, codigo);

            try
            {
                await EnviarCodigoAsync(email, usuario.Nombre, codigo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo enviar código de verificación a {Email}", email);
                return Ok(new
                {
                    exito = true,
                    mensaje = "Cuenta creada, pero no se pudo enviar el correo de verificación. Usa 'Reenviar código' o contacta soporte.",
                    codigoPrueba = codigo
                });
            }

            return Ok(new
            {
                exito = true,
                mensaje = "Te enviamos un código de verificación a tu correo.",
                codigoPrueba = codigo
            });
        }

        [HttpPost("verificar")]
        public async Task<IActionResult> Verificar([FromBody] VerificarRequest request)
        {
            var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
            var codigo = (request.Codigo ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(codigo))
            {
                return BadRequest(new { mensaje = "Correo y código son obligatorios." });
            }

            if (!Codigos.TryGetValue(email, out var guardado))
            {
                return BadRequest(new { mensaje = "No hay un código activo para este correo. Solicita uno nuevo." });
            }

            if (guardado.ExpiraEn < DateTime.UtcNow)
            {
                Codigos.TryRemove(email, out _);
                return BadRequest(new { mensaje = "El código expiró. Solicita uno nuevo." });
            }

            if (!string.Equals(guardado.Codigo, codigo, StringComparison.Ordinal))
            {
                return BadRequest(new { mensaje = "El código es incorrecto." });
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado." });
            }

            usuario.Verificado = true;
            usuario.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            Codigos.TryRemove(email, out _);

            return Ok(new { exito = true, mensaje = "Correo verificado. Ya puedes iniciar sesión." });
        }

        [HttpPost("reenviar")]
        public async Task<IActionResult> Reenviar([FromBody] ReenviarRequest request)
        {
            var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { mensaje = "El correo es obligatorio." });
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
            if (usuario == null)
            {
                return NotFound(new { mensaje = "No existe una cuenta con ese correo." });
            }

            if (usuario.Verificado)
            {
                return Ok(new { exito = true, mensaje = "Esta cuenta ya está verificada. Puedes iniciar sesión." });
            }

            var codigo = GenerarCodigo();
            GuardarCodigo(email, codigo);

            try
            {
                await EnviarCodigoAsync(email, usuario.Nombre, codigo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo reenviar código a {Email}", email);
                return BadRequest(new
                {
                    mensaje = "No se pudo enviar el correo. Intenta de nuevo.",
                    codigoPrueba = codigo
                });
            }

            return Ok(new
            {
                exito = true,
                mensaje = "Te enviamos un nuevo código de verificación.",
                codigoPrueba = codigo
            });
        }

        private static string GenerarCodigo()
        {
            return Random.Shared.Next(100000, 999999).ToString();
        }

        private static void GuardarCodigo(string email, string codigo)
        {
            Codigos[email] = new CodigoVerificacion(codigo, DateTime.UtcNow.AddMinutes(15));
        }

        private async Task EnviarCodigoAsync(string email, string nombre, string codigo)
        {
            var mensaje = $@"
                <div style='font-family:Arial,sans-serif;max-width:520px;margin:0 auto;padding:24px'>
                  <h2 style='color:#2C5F8A;margin-bottom:8px'>Verifica tu cuenta PawFeeder</h2>
                  <p>Hola {System.Net.WebUtility.HtmlEncode(nombre)},</p>
                  <p>Tu código de verificación es:</p>
                  <div style='font-size:32px;font-weight:bold;letter-spacing:6px;color:#1E3A5C;background:#F5F0E8;padding:16px 24px;border-radius:12px;text-align:center;margin:20px 0'>
                    {codigo}
                  </div>
                  <p style='color:#5F6E7D;font-size:14px'>Este código expira en 15 minutos. Si no solicitaste esta cuenta, ignora este correo.</p>
                </div>";

            await _emailService.EnviarCorreoAsync(email, "Código de verificación PawFeeder", mensaje);
        }

        private sealed record CodigoVerificacion(string Codigo, DateTime ExpiraEn);
    }

    public class RegistroRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class VerificarRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
    }

    public class ReenviarRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
