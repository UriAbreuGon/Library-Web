using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entities
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string Genero { get; set; }
        public int Año { get; set; }
        public string Descripcion { get; set; }
        public int Paginas { get; set; }
        public bool EstaDisponible { get; set; }
        public string Ubicacion { get; set; }
    }
}
