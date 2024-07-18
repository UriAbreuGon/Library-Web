using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrestamosController : ControllerBase
    {
        private readonly IPrestamoRepositorio _prestamoRepositorio;

        public PrestamosController(IPrestamoRepositorio prestamoRepositorio)
        {
            _prestamoRepositorio = prestamoRepositorio;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prestamo>>> ObtenerPrestamos()
        {
            var prestamos = await _prestamoRepositorio.ObtenerTodos();
            return Ok(prestamos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Prestamo>> ObtenerPrestamo(int id)
        {
            var prestamo = await _prestamoRepositorio.ObtenerPorId(id);

            if (prestamo == null)
            {
                return NotFound();
            }

            return Ok(prestamo);
        }

        [HttpPost]
        public async Task<ActionResult<Prestamo>> CrearPrestamo(Prestamo prestamo)
        {
            var nuevoPrestamo = await _prestamoRepositorio.Crear(prestamo);
            return CreatedAtAction("ObtenerPrestamo", new { id = nuevoPrestamo.Id }, nuevoPrestamo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPrestamo(int id, Prestamo prestamo)
        {
            if (id != prestamo.Id)
            {
                return BadRequest();
            }

            await _prestamoRepositorio.Actualizar(prestamo);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPrestamo(int id)
        {
            await _prestamoRepositorio.Eliminar(id);
            return NoContent();
        }
    }
}
