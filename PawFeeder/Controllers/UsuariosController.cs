using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models;

namespace PawFeeder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {

        private readonly PawFeederContext _context;


        public UsuariosController(PawFeederContext context)
        {
            _context = context;
        }



        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .ToListAsync();

            return Ok(usuarios);
        }



        // GET: api/usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {

            var usuario = await _context.Usuarios
                .FindAsync(id);


            if (usuario == null)
            {
                return NotFound();
            }


            return Ok(usuario);
        }



        // POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> CrearUsuario(
            Usuario usuario)
        {

            usuario.CreatedAt = DateTime.Now;
            usuario.UpdatedAt = DateTime.Now;


            _context.Usuarios.Add(usuario);


            await _context.SaveChangesAsync();


            return CreatedAtAction(
                nameof(GetUsuario),
                new { id = usuario.Id },
                usuario
            );
        }




        // PUT: api/usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(
            int id,
            Usuario usuario)
        {

            if (id != usuario.Id)
            {
                return BadRequest();
            }


            usuario.UpdatedAt = DateTime.Now;


            _context.Entry(usuario).State =
                EntityState.Modified;


            await _context.SaveChangesAsync();


            return NoContent();
        }




        // DELETE: api/usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(
            int id)
        {

            var usuario = await _context.Usuarios
                .FindAsync(id);


            if (usuario == null)
            {
                return NotFound();
            }


            _context.Usuarios.Remove(usuario);


            await _context.SaveChangesAsync();


            return NoContent();
        }

    }
}