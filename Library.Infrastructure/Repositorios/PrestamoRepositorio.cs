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
                _contexto.Prestamos.Remove(prestamo);
                await _contexto.SaveChangesAsync();
            }
        }
    }
}
