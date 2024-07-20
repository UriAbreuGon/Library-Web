using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Interfaces
{
    public interface IReservaRepositorio
    {
        Task<IEnumerable<Reserva>> ObtenerTodos();
        Task<Reserva> ObtenerPorId(int id);
        Task<Reserva> Crear(Reserva reserva);
        Task Actualizar(Reserva reserva);
        Task Eliminar(int id);
    }
}
