using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Interfaces
{
    public interface IPrestamoRepositorio
    {
        Task<IEnumerable<Prestamo>> ObtenerTodos();
        Task<Prestamo> ObtenerPorId(int id);
        Task<Prestamo> Crear(Prestamo prestamo);
        Task Actualizar(Prestamo prestamo);
        Task Eliminar(int id);
        Task<IEnumerable<Prestamo>> ObtenerHistorialPorUsuario(int usuarioId);
    }
}
