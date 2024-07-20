using Library.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Library.Infrastructure.Datos;

namespace Library.Infrastructure.Repositorios
{
    public class LibroRepositorio : ILibroRepositorio
    {
        private readonly ContextoBiblioteca _contexto;

        public LibroRepositorio(ContextoBiblioteca contexto)
        {
            _contexto = contexto;
        }

        public async Task<IEnumerable<Libro>> ObtenerTodos()
        {
            return await _contexto.Libros.ToListAsync();
        }

        public async Task<Libro> ObtenerPorId(int id)
        {
            return await _contexto.Libros.FindAsync(id);
        }

        public async Task<IEnumerable<Libro>> BuscarLibros(string? titulo, string? autor, string? genero, int? año)
        {
            var query = _contexto.Libros.AsQueryable();

            if (!string.IsNullOrEmpty(titulo))
            {
                query = query.Where(l => l.Titulo.Contains(titulo));
            }

            if (!string.IsNullOrEmpty(autor))
            {
                query = query.Where(l => l.Autor.Contains(autor));
            }

            if (!string.IsNullOrEmpty(genero))
            {
                query = query.Where(l => l.Genero.Contains(genero));
            }

            if (año.HasValue)
            {
                query = query.Where(l => l.Año == año.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Libro> Crear(Libro libro)
        {
            _contexto.Libros.Add(libro);
            await _contexto.SaveChangesAsync();
            return libro;
        }

        public async Task Actualizar(Libro libro)
        {
            _contexto.Entry(libro).State = EntityState.Modified;
            await _contexto.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            var libro = await _contexto.Libros.FindAsync(id);
            if (libro != null)
            {
                _contexto.Libros.Remove(libro);
                await _contexto.SaveChangesAsync();
            }
        }
    }
}
