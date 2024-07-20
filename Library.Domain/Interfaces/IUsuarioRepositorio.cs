using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Domain.Entities;

namespace Library.Domain.Interfaces
{
    public interface IUsuarioRepositorio
    {
        Task<IEnumerable<Usuario>> ObtenerTodos();
        Task<Usuario> ObtenerPorId(int id);
        Task<Usuario> Crear(Usuario usuario);
        Task Actualizar(Usuario usuario);
        Task Eliminar(int id);
        Task<Usuario> ObtenerPorCorreo(string correo);
    }
}
