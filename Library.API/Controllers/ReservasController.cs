using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservasController : ControllerBase
    {
        private readonly IReservaRepositorio _reservaRepositorio;
        private readonly IEmailService _emailService;

        public ReservasController(IReservaRepositorio reservaRepositorio, IEmailService emailService)
        {
            _reservaRepositorio = reservaRepositorio;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> ObtenerReservas()
        {
            var reservas = await _reservaRepositorio.ObtenerTodos();
            return Ok(reservas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reserva>> ObtenerReserva(int id)
        {
            var reserva = await _reservaRepositorio.ObtenerPorId(id);

            if (reserva == null)
            {
                return NotFound();
            }

            return Ok(reserva);
        }

        [HttpPost]
        public async Task<ActionResult<Reserva>> CrearReserva(Reserva reserva)
        {
            var nuevaReserva = await _reservaRepositorio.Crear(reserva);

            // Enviar notificación por correo electrónico
            var usuario = nuevaReserva.Usuario;
            var libro = nuevaReserva.Libro;
            var mensaje = $"Hola {usuario.NombreUsuario},\n\nTu reserva para el libro \"{libro.Titulo}\" ha sido confirmada.\n\nGracias.";
            await _emailService.EnviarCorreoAsync(usuario.Correo, "Confirmación de Reserva", mensaje);

            return CreatedAtAction("ObtenerReserva", new { id = nuevaReserva.Id }, nuevaReserva);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarReserva(int id, Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return BadRequest();
            }

            await _reservaRepositorio.Actualizar(reserva);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarReserva(int id)
        {
            await _reservaRepositorio.Eliminar(id);
            return NoContent();
        }
    }
}
