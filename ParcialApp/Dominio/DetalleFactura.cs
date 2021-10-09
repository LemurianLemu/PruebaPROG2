using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcialApp.Dominio
{
    class DetalleFactura
    {

        public Producto Producto { get; set; }

        public int Cantidad { get; set; }

        public DetalleFactura(Producto oProducto, int cantidad )
        {
            Producto = oProducto;
            Cantidad = cantidad;
        }

        public double CalcularSubTotal() {

            return Producto.PrecioP * Cantidad;
        }

      
    }
}
