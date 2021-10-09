using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcialApp.Dominio
{
    class Producto
    {
        public int ProductoNro { 
            
            get; 
            
            set; }

        public string NombreP { get; set; }

        public double PrecioP { get; set; }

        public bool Activo { get; set; }

        public Producto(int productoNro, string nombreP, double precioP)
        {
            ProductoNro = productoNro;
            NombreP = nombreP;
            PrecioP = precioP;
            Activo = true;

        }
    }

}
