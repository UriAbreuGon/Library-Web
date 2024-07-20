using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Repositorios
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ContextoBiblioteca _contexto;

        public UsuarioRepositorio(ContextoBiblioteca contexto)
        {
            _contexto = contexto;
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodos()
        {
            return await _contexto.Usuarios.ToListAsync();
        }

        public async Task<Usuario> ObtenerPorId(int id)
        {
            return await _contexto.Usuarios.FindAsync(id);
        }

        public async Task<Usuario> Crear(Usuario usuario)
        {
            _contexto.Usuarios.Add(usuario);
            await _contexto.SaveChangesAsync();
            return usuario;
        }

        public async Task Actualizar(Usuario usuario)
        {
            _contexto.Entry(usuario).State = EntityState.Modified;
            await _contexto.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            var usuario = await _contexto.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _contexto.Usuarios.Remove(usuario);
                await _contexto.SaveChangesAsync();
            }
        }

        public async Task<Usuario> ObtenerPorCorreo(string correo)
        {
            return await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
        }
    }
}
