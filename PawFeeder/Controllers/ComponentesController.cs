using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models;

namespace PawFeeder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComponentesController : ControllerBase
    {
        private readonly PawFeederContext _context;

        public ComponentesController(PawFeederContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Componente>>> GetComponentes()
        {
            return await _context.Componentes.ToListAsync();
        }

        // ➕ NUEVO MÉTODO: Registrar un nuevo componente de materia prima
        [HttpPost]
        public async Task<ActionResult<Componente>> CrearComponente([FromBody] Componente nuevoComponente)
        {
            // Validación 1: Datos nulos o nombre en blanco
            if (nuevoComponente == null || string.IsNullOrWhiteSpace(nuevoComponente.Nombre))
            {
                return BadRequest("El nombre del componente es obligatorio.");
            }

            // Validación 2: Cero o menor a cero
            if (nuevoComponente.Stock <= 0)
            {
                return BadRequest("El stock inicial debe ser mayor a 0.");
            }

            // Limpiamos espacios en blanco
            nuevoComponente.Nombre = nuevoComponente.Nombre.Trim();
            nuevoComponente.UnidadMedida = "pza";

            _context.Componentes.Add(nuevoComponente);
            await _context.SaveChangesAsync();

            return Ok(nuevoComponente);
        }

        // ✏️ EDITAR COMPONENTE
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarComponente(int id, [FromBody] Componente componenteActualizado)
        {
            if (id != componenteActualizado.Id)
                return BadRequest("El ID del componente no coincide.");

            var componenteExistente = await _context.Componentes.FindAsync(id);
            if (componenteExistente == null)
                return NotFound("El componente no existe.");

            if (string.IsNullOrWhiteSpace(componenteActualizado.Nombre))
                return BadRequest("El nombre no puede estar vacío.");

            if (componenteActualizado.Stock < 0)
                return BadRequest("El stock no puede ser negativo.");

            componenteExistente.Nombre = componenteActualizado.Nombre.Trim();
            componenteExistente.Stock = componenteActualizado.Stock;

            await _context.SaveChangesAsync();
            return Ok(componenteExistente);
        }

        // 🗑️ ELIMINAR COMPONENTE
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarComponente(int id)
        {
            var componente = await _context.Componentes.FindAsync(id);
            if (componente == null)
                return NotFound("El componente a eliminar no existe.");

            _context.Componentes.Remove(componente);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Componente '{componente.Nombre}' eliminado correctamente." });
        }
    }
}