using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models;

namespace PawFeeder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DispensadoresInventarioController : ControllerBase
    {
        private readonly PawFeederContext _context;

        public DispensadoresInventarioController(PawFeederContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            var items = await _context.DispensadoresInventario
                .Include(d => d.Producto)
                .OrderByDescending(d => d.CreadoEn)
                .Select(d => new
                {
                    d.Id,
                    d.ProductoId,
                    ProductoNombre = d.Producto != null ? d.Producto.Nombre : "N/A",
                    d.CodigoUnico,
                    d.Estado,
                    d.CreadoEn
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("conteo-terminados")]
        public async Task<ActionResult<object>> ConteoTerminados()
        {
            var total = await _context.DispensadoresInventario
                .CountAsync(d => d.Estado == "Terminado");

            return Ok(new { total });
        }

        [HttpPut("{id}/estado")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] EstadoRequest request)
        {
            var item = await _context.DispensadoresInventario.FindAsync(id);
            if (item == null)
                return NotFound("Dispensador no encontrado.");

            item.Estado = request.Estado;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Estado actualizado a '{request.Estado}'." });
        }
    }
}
