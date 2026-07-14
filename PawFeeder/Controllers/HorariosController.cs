using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PawFeeder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HorariosController : ControllerBase
    {
        private readonly PawFeederContext _context;

        public HorariosController(PawFeederContext context)
        {
            _context = context;
        }

        // 1. GET: api/Horarios/Usuario/1
        // Obtiene todos los horarios programados por un usuario específico
        [HttpGet("Usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Horario>>> GetHorariosPorUsuario(int usuarioId)
        {
            var horarios = await _context.Horarios
                .Where(h => h.UsuarioId == usuarioId && h.Activo == true)
                .OrderBy(h => h.Hora)
                .ToListAsync();

            return Ok(horarios);
        }

        // 2. GET: api/Horarios/5
        // Obtiene un horario específico por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Horario>> GetHorario(int id)
        {
            var horario = await _context.Horarios.FindAsync(id);

            if (horario == null)
            {
                return NotFound(new { mensaje = "Horario no encontrado." });
            }

            return Ok(horario);
        }

        // POST: api
        [HttpPost]
        public async Task<ActionResult<Horario>> PostHorario([FromBody] Horario horario)
        {
            if (horario == null)
            {
                return BadRequest(new { mensaje = "Los datos del horario son inválidos." });
            }

            horario.CreatedAt = DateTime.Now;
            horario.UpdatedAt = DateTime.Now;

            _context.Horarios.Add(horario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHorario), new { id = horario.Id }, horario);
        }

        // 4. PUT: api/Horarios/5
        // Actualiza un horario existente (¡Ruta blindada para Angular!)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHorario(int id, [FromBody] Horario horario)
        {
            if (id <= 0)
            {
                return BadRequest(new { mensaje = "ID de horario inválido." });
            }

            // Sincronizamos el ID del objeto con el de la URL
            horario.Id = id;
            horario.UpdatedAt = DateTime.Now;

            _context.Entry(horario).State = EntityState.Modified;

            // Evitamos que Entity Framework intente modificar la fecha de creación original
            _context.Entry(horario).Property(x => x.CreatedAt).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HorarioExists(id))
                {
                    return NotFound(new { mensaje = "El horario que intentas actualizar ya no existe." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Retorna 204 Éxito sin contenido
        }


        // 5. DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHorario(int id)
        {
            var horario = await _context.Horarios.FindAsync(id);
            if (horario == null) return NotFound();

            // En lugar de usar _context.Horarios.Remove(horario), cambiamos su estado:
            horario.Activo = false;
            horario.UpdatedAt = DateTime.Now;

            _context.Entry(horario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Horario desactivado lógicamente con éxito" });
        }

        private bool HorarioExists(int id)
        {
            return _context.Horarios.Any(e => e.Id == id);
        }
    }
}