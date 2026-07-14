using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models;

namespace PawFeeder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpinionesController : ControllerBase
    {
        private readonly PawFeederContext _context;

        public OpinionesController(PawFeederContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Opinion>>> GetOpiniones()
        {
            return await _context.Opiniones.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Opinion>> PostOpinion([FromBody] Opinion opinion)
        {
            _context.Opiniones.Add(opinion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOpiniones), new { id = opinion.Id }, opinion);
        }
    }
}