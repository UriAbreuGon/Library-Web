using Library.Domain.Entities;
using Library.Infrastructure.Datos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly ContextoBiblioteca _contexto;

        public LibrosController(ContextoBiblioteca contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Libro>>> ObtenerLibros()
        {
            return await _contexto.Libros.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Libro>> ObtenerLibro(int id)
        {
            var libro = await _contexto.Libros.FindAsync(id);

            if (libro == null)
            {
                return NotFound();
            }

            return libro;
        }

        [HttpPost]
        public async Task<ActionResult<Libro>> CrearLibro(Libro libro)
        {
            _contexto.Libros.Add(libro);
            await _contexto.SaveChangesAsync();

            return CreatedAtAction("ObtenerLibro", new { id = libro.Id }, libro);
        }

        // Otros métodos para PUT y DELETE
    }
}

