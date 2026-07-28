using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models;

namespace PawFeeder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly PawFeederContext _context;

        public ProductosController(PawFeederContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoTerminado>>> GetProductos()
        {
            return await _context.InventarioProductos.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<ProductoTerminado>> CrearProducto([FromBody] ProductoTerminado producto)
        {
            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return BadRequest("El nombre del producto es obligatorio.");

            var existe = await _context.InventarioProductos
                .AnyAsync(p => p.Nombre.ToLower() == producto.Nombre.ToLower());

            if (existe)
                return BadRequest($"Ya existe un producto con el nombre '{producto.Nombre}'.");

            producto.Stock = 0;
            producto.Estado = "En proceso";
            _context.InventarioProductos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductos), new { id = producto.Id }, producto);
        }

        [HttpPut("{id}/estado")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] EstadoRequest request)
        {
            var producto = await _context.InventarioProductos.FindAsync(id);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            producto.Estado = request.Estado;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Estado actualizado a '{request.Estado}'." });
        }
    }

    public class EstadoRequest
    {
        public string Estado { get; set; } = string.Empty;
    }
}