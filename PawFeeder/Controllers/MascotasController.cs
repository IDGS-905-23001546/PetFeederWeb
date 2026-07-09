using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models; // <-- IMPORTANTE: Esta línea debe estar aquí arriba

namespace PawFeeder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MascotasController : ControllerBase
    {
        // El resto del código de tu controlador se queda igual...
        private readonly PawFeederContext _context;

        // Inyectamos el contexto de la base de datos que configuramos
        public MascotasController(PawFeederContext context)
        {
            _context = context;
        }

        // 1. GET: api/Mascotas/usuario/1
        // Obtiene todas las mascotas vinculadas a un usuario específico
        [HttpGet("usuario/{usuarioId:int}")]
        public async Task<IActionResult> GetMascotasPorUsuario(int usuarioId)
        {
            var mascotas = await _context.Mascotas
                .Where(m => m.UsuarioId == usuarioId)
                .ToListAsync();

            return Ok(mascotas);
        }

        // 2. POST: api/Mascotas
        // Guarda una nueva mascota en la base de datos
        [HttpPost]
        public async Task<IActionResult> RegistrarMascota([FromBody] Mascota modelo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Forzamos las fechas de creación para mantener el estándar de tu BD
            // Nota: En tu modelo 'MascotaModel' actual asegúrate de incluir las propiedades necesarias
            _context.Mascotas.Add(modelo);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "¡Mascota registrada con éxito!", id = modelo.Id });
        }

        // 3. DELETE: api/Mascotas/5
        // Eliminación física (o puedes cambiar el campo 'Activa' a false para borrado lógico)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarMascota(int id)
        {
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null)
                return NotFound(new { mensaje = "Mascota no encontrada." });

            _context.Mascotas.Remove(mascota);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Mascota eliminada correctamente de la base de datos." });
        }
    }
}