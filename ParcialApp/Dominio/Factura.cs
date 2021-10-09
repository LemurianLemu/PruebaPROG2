using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcialApp.Dominio
{
    class Factura
    {
        public int NumFac { get; set; }
        public DateTime FechaFac { get; set; }
        public string Cliente { get; set; }
        public int FormaPago { get; set; }
        public DateTime FechaBaja { get; set; }
        public double TotalFac { get; set; }

        public List<DetalleFactura> Detalles { get; set; }

        public Factura()
        {
            Detalles = new List<DetalleFactura>();
        }

        public double CalcularTotal()
        {
            double total = 0;

            foreach (DetalleFactura item in Detalles)
            {
                total += item.CalcularSubTotal();
            }

            
            return total;
        
        }

        public void AgregarDetalle(DetalleFactura detalle)
        {
            Detalles.Add(detalle);

        }

        public void QuitarDetalle(int indice)
        {
            Detalles.RemoveAt(indice);
        }

        public bool Confirmar() {



            bool estado = true;
            SqlConnection conexion = new SqlConnection();
            SqlTransaction transaccion = null;
            try
            {

                conexion.ConnectionString = @"Data Source=PC-PC\SQLDEVELOPER;Initial Catalog=db_facturas;Integrated Security=True";
                conexion.Open();
                transaccion = conexion.BeginTransaction();
                SqlCommand comando = new SqlCommand();
                comando.Connection = conexion;
                comando.Transaction = transaccion;
                comando.CommandType = CommandType.StoredProcedure;
                //SP MAESTRO
                comando.CommandText = "SP_INSERTAR_FACTURA";
                comando.Parameters.AddWithValue("@cliente", this.Cliente);
                comando.Parameters.AddWithValue("@forma", this.FormaPago);
                comando.Parameters.AddWithValue("@total", this.TotalFac);
                comando.Parameters.AddWithValue("@nro", this.NumFac);


                comando.ExecuteNonQuery();


                //SP DETALLE

                int DetalleNro = 1;
                foreach (DetalleFactura item in this.Detalles)
                {
                    SqlCommand comando2 = new SqlCommand();
                    comando2.Connection = conexion;
                    comando2.CommandType = CommandType.StoredProcedure;
                    comando2.Transaction = transaccion;

                    comando2.CommandText = "SP_INSERTAR_DETALLES";
                    comando2.Parameters.AddWithValue("@nro", this.NumFac);
                    comando2.Parameters.AddWithValue("@detalle", DetalleNro);
                    comando2.Parameters.AddWithValue("@id_producto", item.Producto.ProductoNro);
                    comando2.Parameters.AddWithValue("@cantidad", item.Cantidad);
                    comando2.ExecuteNonQuery();
                    DetalleNro++;
                }
                transaccion.Commit();


            }
            catch (Exception)
            {
                transaccion.Rollback();
                estado = false;
            }

            finally
            {
                if (conexion.State == ConnectionState.Open)
                    conexion.Close();
            }

            return estado;

        }

    }
}
