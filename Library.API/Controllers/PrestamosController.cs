using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrestamosController : ControllerBase
    {
        private readonly IPrestamoRepositorio _prestamoRepositorio;
        private readonly IEmailService _emailService;
        private readonly ILibroRepositorio _libroRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public PrestamosController(
            IPrestamoRepositorio prestamoRepositorio,
            IEmailService emailService,
            ILibroRepositorio libroRepositorio,
            IUsuarioRepositorio usuarioRepositorio)
        {
            _prestamoRepositorio = prestamoRepositorio;
            _emailService = emailService;
            _libroRepositorio = libroRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Prestamo>> CrearPrestamo(Prestamo prestamo)
        {
            var libro = await _libroRepositorio.ObtenerPorId(prestamo.LibroId);
            var usuario = await _usuarioRepositorio.ObtenerPorId(prestamo.UsuarioId);

            if (libro == null || usuario == null)
            {
                return BadRequest("Libro o Usuario no encontrado.");
            }

            if (!libro.EstaDisponible)
            {
                return BadRequest("El libro no está disponible para préstamo.");
            }

            prestamo.Libro = libro;
            prestamo.Usuario = usuario;

            var nuevoPrestamo = await _prestamoRepositorio.Crear(prestamo);

            // Enviar notificación por correo electrónico
            if (!string.IsNullOrEmpty(usuario.Correo))
            {
                var mensaje = $"Tu préstamo del libro '{libro.Titulo}' ha sido registrado con éxito.";
                await _emailService.EnviarCorreoAsync(usuario.Correo, "Préstamo Registrado", mensaje);
            }

            return CreatedAtAction(nameof(ObtenerPrestamo), new { id = nuevoPrestamo.Id }, nuevoPrestamo);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Prestamo>> ObtenerPrestamo(int id)
        {
            var prestamo = await _prestamoRepositorio.ObtenerPorId(id);

            if (prestamo == null)
            {
                return NotFound();
            }

            return Ok(prestamo);
        }

        [HttpPut("{id}")]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> EliminarPrestamo(int id)
        {
            await _prestamoRepositorio.Eliminar(id);
            return NoContent();
        }

        [HttpGet("historial/{usuarioId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Prestamo>>> ObtenerHistorialPorUsuario(int usuarioId)
        {
            var historial = await _prestamoRepositorio.ObtenerHistorialPorUsuario(usuarioId);
            return Ok(historial);
        }
    }
}
