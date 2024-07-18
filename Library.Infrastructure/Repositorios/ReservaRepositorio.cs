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
    public class ReservaRepositorio : IReservaRepositorio
    {
        private readonly ContextoBiblioteca _contexto;

        public ReservaRepositorio(ContextoBiblioteca contexto)
        {
            _contexto = contexto;
        }

        public async Task<IEnumerable<Reserva>> ObtenerTodos()
        {
            return await _contexto.Reservas.Include(r => r.Libro).Include(r => r.Usuario).ToListAsync();
        }

        public async Task<Reserva> ObtenerPorId(int id)
        {
            return await _contexto.Reservas.Include(r => r.Libro).Include(r => r.Usuario).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Reserva> Crear(Reserva reserva)
        {
            _contexto.Reservas.Add(reserva);
            await _contexto.SaveChangesAsync();
            return reserva;
        }

        public async Task Actualizar(Reserva reserva)
        {
            _contexto.Entry(reserva).State = EntityState.Modified;
            await _contexto.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            var reserva = await _contexto.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _contexto.Reservas.Remove(reserva);
                await _contexto.SaveChangesAsync();
            }
        }
    }
}
