using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models;

namespace PawFeeder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly PawFeederContext _context;

        public UsuariosController(PawFeederContext context)
        {
            _context = context;
        }

        private static UsuarioDto ToDto(Usuario u) => new UsuarioDto
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Email = u.Email,
            Telefono = u.Telefono,
            Verificado = u.Verificado,
            Activo = u.Activo,
            Rol = u.Rol
        };

        // POST: api/usuarios/login
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDto>> Login([FromBody] LoginRequest request)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null)
                return Unauthorized(new { mensaje = "Credenciales inv\u00e1lidas" });

            if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
                return Unauthorized(new { mensaje = "Credenciales inv\u00e1lidas" });

            return Ok(ToDto(usuario));
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return Ok(usuarios.Select(ToDto));
        }

        // GET: api/usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            return Ok(ToDto(usuario));
        }

        // POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult<UsuarioDto>> CrearUsuario([FromBody] Usuario usuario)
        {
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
            usuario.Rol = string.IsNullOrWhiteSpace(usuario.Rol) ? "cliente" : usuario.Rol;
            usuario.Verificado = true;
            usuario.Activo = true;
            usuario.CreatedAt = DateTime.Now;
            usuario.UpdatedAt = DateTime.Now;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, ToDto(usuario));
        }

        // PUT: api/usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.Id)
                return BadRequest();

            usuario.UpdatedAt = DateTime.Now;
            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/usuarios/{id}/estado
        [HttpPut("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] EstadoUsuarioRequest request)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.Activo = request.Activo;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Estado actualizado" });
        }
    }
}
