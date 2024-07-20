using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Interfaces
{
    public interface ILibroRepositorio
    {
        Task<IEnumerable<Libro>> ObtenerTodos();
        Task<Libro> ObtenerPorId(int id);
        Task<IEnumerable<Libro>> BuscarLibros(string titulo, string autor, string genero, int? año);
        Task<Libro> Crear(Libro libro);
        Task Actualizar(Libro libro);
        Task Eliminar(int id);
    }
}
