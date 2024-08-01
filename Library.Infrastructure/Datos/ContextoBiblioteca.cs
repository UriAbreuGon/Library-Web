using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Datos
{
    public class ContextoBiblioteca : DbContext
    {
        public ContextoBiblioteca(DbContextOptions<ContextoBiblioteca> opciones) : base(opciones) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=MSI\\SQLEXPRESS;Database=BibliotecaDb;Trusted_Connection=True;MultipleActiveResultSets=true",
                    b => b.MigrationsAssembly("Library.Infrastructure"));
            }
        }

        public DbSet<Libro> Libros { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuraciones adicionales de las entidades
        }
    }

}
