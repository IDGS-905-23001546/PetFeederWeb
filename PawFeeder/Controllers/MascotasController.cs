using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models; 

namespace PawFeeder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MascotasController : ControllerBase
    {
        private readonly PawFeederContext _context;

        public MascotasController(PawFeederContext context)
        {
            _context = context;
        }

        // 1. GET:
        [HttpGet("usuario/{usuarioId:int}")]
        public async Task<IActionResult> GetMascotasPorUsuario(int usuarioId)
        {
            var mascotas = await _context.Mascotas
                .Where(m => m.UsuarioId == usuarioId)
                .ToListAsync();

            return Ok(mascotas);
        }

        // 2. POST:
        [HttpPost]
        public async Task<ActionResult<Mascota>> PostMascota([FromBody] Mascota mascota) // <-- IMPORTANTE EL [FromBody]
        {
            _context.Mascotas.Add(mascota);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMascota", new { id = mascota.Id }, mascota);
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMascota(int id, [FromBody] Mascota mascota)
        {
            if (id <= 0)
            {
                return BadRequest("ID de mascota inválido.");
            }

            mascota.Id = id;

            _context.Entry(mascota).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Mascotas.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); 
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMascota(int id)
        {
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null)
            {
                return NotFound();
            }

            _context.Mascotas.Remove(mascota);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}