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
    public class PrestamoRepositorio : IPrestamoRepositorio
    {
        private readonly ContextoBiblioteca _contexto;

        public PrestamoRepositorio(ContextoBiblioteca contexto)
        {
            _contexto = contexto;
        }

        public async Task<IEnumerable<Prestamo>> ObtenerTodos()
        {
            return await _contexto.Prestamos.Include(p => p.Libro).Include(p => p.Usuario).ToListAsync();
        }

        public async Task<Prestamo> ObtenerPorId(int id)
        {
            return await _contexto.Prestamos.Include(p => p.Libro).Include(p => p.Usuario).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Prestamo> Crear(Prestamo prestamo)
        {
            _contexto.Prestamos.Add(prestamo);
            prestamo.Libro.EstaDisponible = false; // Marcar el libro como no disponible
            await _contexto.SaveChangesAsync();
            return prestamo;
        }

        public async Task Actualizar(Prestamo prestamo)
        {
            _contexto.Entry(prestamo).State = EntityState.Modified;
            await _contexto.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            var prestamo = await _contexto.Prestamos.FindAsync(id);
            if (prestamo != null)
            {
                prestamo.Libro.EstaDisponible = true; // Marcar el libro como disponible
                _contexto.Prestamos.Remove(prestamo);
                await _contexto.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Prestamo>> ObtenerHistorialPorUsuario(int usuarioId)
        {
            return await _contexto.Prestamos
                .Include(p => p.Libro)
                .Include(p => p.Usuario)
                .Where(p => p.UsuarioId == usuarioId)
                .ToListAsync();
        }
    }
}
