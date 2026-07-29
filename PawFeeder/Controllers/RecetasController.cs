using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models;

namespace PawFeeder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecetasController : ControllerBase
    {
        private readonly PawFeederContext _context;

        public RecetasController(PawFeederContext context)
        {
            _context = context;
        }

        [HttpGet("producto/{productoId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetByProducto(int productoId)
        {
            var recetas = await _context.RecetasProducto
                .Include(r => r.Componente)
                .Where(r => r.ProductoId == productoId)
                .ToListAsync();

            return Ok(recetas.Select(r => new
            {
                r.Id,
                r.ProductoId,
                r.ComponenteId,
                ComponenteNombre = r.Componente?.Nombre ?? "",
                r.CantidadRequerida,
                r.Dispensador
            }));
        }

        [HttpGet("productos-con-receta")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductosConReceta()
        {
            var productos = await _context.InventarioProductos.ToListAsync();
            var recetaCounts = await _context.RecetasProducto
                .GroupBy(r => r.ProductoId)
                .Select(g => new { ProductoId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ProductoId, x => x.Count);

            return Ok(productos.Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Stock,
                p.Estado,
                ComponentesCount = recetaCounts.GetValueOrDefault(p.Id, 0)
            }));
        }

        [HttpPost]
        public async Task<ActionResult<RecetaProducto>> Create([FromBody] RecetaRequest request)
        {
            if (request.CantidadRequerida <= 0)
                return BadRequest("La cantidad requerida debe ser mayor a 0.");

            var existe = await _context.RecetasProducto
                .AnyAsync(r => r.ProductoId == request.ProductoId && r.ComponenteId == request.ComponenteId);
            if (existe)
                return BadRequest("Ese componente ya está registrado en la receta de este producto.");

            var receta = new RecetaProducto
            {
                ProductoId = request.ProductoId,
                ComponenteId = request.ComponenteId,
                CantidadRequerida = request.CantidadRequerida,
                Dispensador = request.Dispensador
            };

            _context.RecetasProducto.Add(receta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByProducto), new { productoId = receta.ProductoId }, receta);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RecetaRequest request)
        {
            var receta = await _context.RecetasProducto.FindAsync(id);
            if (receta == null) return NotFound();

            if (request.CantidadRequerida <= 0)
                return BadRequest("La cantidad requerida debe ser mayor a 0.");

            receta.CantidadRequerida = request.CantidadRequerida;
            receta.Dispensador = request.Dispensador;

            await _context.SaveChangesAsync();
            return Ok(receta);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var receta = await _context.RecetasProducto.FindAsync(id);
            if (receta == null) return NotFound();

            _context.RecetasProducto.Remove(receta);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class RecetaRequest
    {
        public int ProductoId { get; set; }
        public int ComponenteId { get; set; }
        public int CantidadRequerida { get; set; }
        public string? Dispensador { get; set; }
    }
}
