using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models;


namespace PawFeeder.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class DispensadoresController : ControllerBase
    {


        private readonly PawFeederContext _context;


        public DispensadoresController(PawFeederContext context)
        {
            _context = context;
        }



        // GET api/Dispensadores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dispensador>>> GetDispensadores()
        {

            var dispensadores = await _context.Dispensadores
                .ToListAsync();


            return Ok(dispensadores);

        }



        // GET api/Dispensadores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Dispensador>> GetDispensador(int id)
        {

            var dispensador = await _context.Dispensadores
                .FindAsync(id);


            if (dispensador == null)
            {
                return NotFound();
            }


            return Ok(dispensador);

        }



        // POST api/Dispensadores
        [HttpPost]
        public async Task<ActionResult<Dispensador>> CrearDispensador(
            Dispensador dispensador)
        {


            dispensador.CreatedAt = DateTime.Now;
            dispensador.UpdatedAt = DateTime.Now;


            _context.Dispensadores.Add(dispensador);


            await _context.SaveChangesAsync();


            return Ok(dispensador);

        }



    }

}