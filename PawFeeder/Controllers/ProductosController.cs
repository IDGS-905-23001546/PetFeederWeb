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
    }
}