using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models;

namespace PawFeeder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProduccionController : ControllerBase
    {
        private readonly PawFeederContext _context;

        public ProduccionController(PawFeederContext context)
        {
            _context = context;
        }

        [HttpPost("fabricar-dispensador/{productoId}")]
        public async Task<IActionResult> FabricarDispensador(int productoId, [FromQuery] int cantidadAFabricar = 1)
        {
            // Validación de cantidad válida
            if (cantidadAFabricar <= 0)
            {
                return BadRequest("La cantidad a fabricar debe ser mayor a 0.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var producto = await _context.InventarioProductos.FindAsync(productoId);
                if (producto == null)
                    return NotFound("El producto especificado no existe.");

                var receta = await _context.RecetasProducto
                    .Include(r => r.Componente)
                    .Where(r => r.ProductoId == productoId)
                    .ToListAsync();

                if (!receta.Any())
                    return BadRequest("El producto no tiene una receta de componentes registrada.");

                // Validación de Stock suficiente antes de descontar
                foreach (var item in receta)
                {
                    int requeridosTotal = item.CantidadRequerida * cantidadAFabricar;
                    if (item.Componente.Stock < requeridosTotal)
                    {
                        return BadRequest($"Stock insuficiente de '{item.Componente.Nombre}'. Se necesitan {requeridosTotal} pzas y solo hay {item.Componente.Stock} en existencia.");
                    }
                }

                // Descuento e incremento
                foreach (var item in receta)
                {
                    item.Componente.Stock -= (item.CantidadRequerida * cantidadAFabricar);
                }

                producto.Stock += cantidadAFabricar;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = $"¡Ensamblaje exitoso! Se produjeron {cantidadAFabricar} dispensador(es)." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error interno durante el proceso de producción: {ex.Message}");
            }
        }
    }
    }
