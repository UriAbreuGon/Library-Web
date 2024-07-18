using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly IReservaRepositorio _reservaRepositorio;

        public ReservasController(IReservaRepositorio reservaRepositorio)
        {
            _reservaRepositorio = reservaRepositorio;
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
