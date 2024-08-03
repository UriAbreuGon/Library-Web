using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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
        private readonly ILogger<PrestamosController> _logger;

        public PrestamosController(
            IPrestamoRepositorio prestamoRepositorio,
            IEmailService emailService,
            ILibroRepositorio libroRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            ILogger<PrestamosController> logger)
        {
            _prestamoRepositorio = prestamoRepositorio;
            _emailService = emailService;
            _libroRepositorio = libroRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Prestamo>> CrearPrestamo([FromBody] CrearPrestamoRequest request)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogError("Datos de préstamo no proporcionados.");
                    return BadRequest("Datos de préstamo no proporcionados.");
                }

                var libro = await _libroRepositorio.ObtenerPorId(request.LibroId);
                if (libro == null)
                {
                    _logger.LogError($"Libro con ID {request.LibroId} no encontrado.");
                    return BadRequest("Libro no encontrado.");
                }

                if (!libro.EstaDisponible)
                {
                    _logger.LogError($"El libro con ID {request.LibroId} no está disponible para préstamo.");
                    return BadRequest("El libro no está disponible para préstamo.");
                }

                // Obtener el usuarioId del token JWT
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var usuario = await _usuarioRepositorio.ObtenerPorId(usuarioId);
                if (usuario == null)
                {
                    _logger.LogError($"Usuario con ID {usuarioId} no encontrado.");
                    return BadRequest("Usuario no encontrado.");
                }

                var prestamo = new Prestamo
                {
                    LibroId = request.LibroId,
                    UsuarioId = usuarioId,
                    FechaPrestamo = DateTime.Now,
                    FechaDevolucion = DateTime.Now.AddMonths(1),
                    Libro = libro,
                    Usuario = usuario
                };

                var nuevoPrestamo = await _prestamoRepositorio.Crear(prestamo);

                // Enviar notificación por correo electrónico
                if (!string.IsNullOrEmpty(usuario.Correo))
                {
                    var mensaje = $"Tu préstamo del libro '{libro.Titulo}' ha sido registrado con éxito.";
                    await _emailService.EnviarCorreoAsync(usuario.Correo, "Préstamo Registrado", mensaje);
                }

                return CreatedAtAction(nameof(ObtenerPrestamo), new { id = nuevoPrestamo.Id }, nuevoPrestamo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el préstamo.");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        public class CrearPrestamoRequest
        {
            public int LibroId { get; set; }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Prestamo>> ObtenerPrestamo(int id)
        {
            try
            {
                var prestamo = await _prestamoRepositorio.ObtenerPorId(id);
                if (prestamo == null)
                {
                    return NotFound();
                }
                return Ok(prestamo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el préstamo.");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> ActualizarPrestamo(int id, Prestamo prestamo)
        {
            try
            {
                if (id != prestamo.Id)
                {
                    return BadRequest();
                }

                await _prestamoRepositorio.Actualizar(prestamo);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el préstamo.");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> EliminarPrestamo(int id)
        {
            try
            {
                var prestamo = await _prestamoRepositorio.ObtenerPorId(id);
                if (prestamo == null)
                {
                    return NotFound();
                }

                await _prestamoRepositorio.Eliminar(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el préstamo.");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("historial/{usuarioId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Prestamo>>> ObtenerHistorialPorUsuario(int usuarioId)
        {
            try
            {
                var historial = await _prestamoRepositorio.ObtenerHistorialPorUsuario(usuarioId);
                return Ok(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el historial de préstamos.");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}

