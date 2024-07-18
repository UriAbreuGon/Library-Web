using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly ILibroRepositorio _libroRepositorio;

        public LibrosController(ILibroRepositorio libroRepositorio)
        {
            _libroRepositorio = libroRepositorio;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Libro>>> ObtenerLibros()
        {
            var libros = await _libroRepositorio.ObtenerTodos();
            return Ok(libros);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Libro>> ObtenerLibro(int id)
        {
            var libro = await _libroRepositorio.ObtenerPorId(id);

            if (libro == null)
            {
                return NotFound();
            }

            return Ok(libro);
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Libro>>> BuscarLibros([FromQuery] string? titulo, [FromQuery] string? autor, [FromQuery] string? genero, [FromQuery] int? año)
        {
            var libros = await _libroRepositorio.BuscarLibros(titulo, autor, genero, año);
            return Ok(libros);
        }


        [HttpPost]
        public async Task<ActionResult<Libro>> CrearLibro(Libro libro)
        {
            var nuevoLibro = await _libroRepositorio.Crear(libro);
            return CreatedAtAction("ObtenerLibro", new { id = nuevoLibro.Id }, nuevoLibro);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarLibro(int id, Libro libro)
        {
            if (id != libro.Id)
            {
                return BadRequest();
            }

            await _libroRepositorio.Actualizar(libro);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarLibro(int id)
        {
            await _libroRepositorio.Eliminar(id);
            return NoContent();
        }
    }
}
