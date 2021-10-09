
using ParcialApp.Dominio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParcialApp.Presentacion
{
    public partial class Frm_Alta : Form
    {
        private Dominio.Factura nuevo;

        
        public Frm_Alta()
        {
            InitializeComponent();
            nuevo = new Factura();

        }




        private void btnAceptar_Click(object sender, EventArgs e)
        {

            if (dgvDetalles.Rows.Count == 0)
            {
                MessageBox.Show("Debe Ingresar un detalle al menos...", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboProducto.Focus();
                return;
            }

            GuardarPresupuesto();
        }

        private void GuardarPresupuesto()
        

        {

            //nuevo.FechaFac = Convert.ToDateTime(dtpFecha.Text);
            nuevo.Cliente = txtCliente.Text;
            nuevo.FormaPago = 1;
            nuevo.NumFac = ProximoPresupuesto();
            //nuevo.Descuento = double.Parse(lblDescuento.Text);
            nuevo.TotalFac = nuevo.CalcularTotal();

            if (nuevo.Confirmar())
            {
                MessageBox.Show("El presupuesto se grabo correctamente!!!!", "Notificacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dgvDetalles.ClearSelection();
                //this.Dispose();
                return;
            }
            else
            {
                MessageBox.Show("El presupuesto NO se grabo!!!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }
    

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Está seguro que desea cancelar?", "Salir", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Dispose();

            }
            else
            {
                return;
            }
        }

        private void Frm_Alta_Presupuesto_Load(object sender, EventArgs e)
        {
            CargarCombo();
            lblNro.Text += ProximoPresupuesto();
        }

        private int ProximoPresupuesto()
        {
            SqlConnection conexion = new SqlConnection();
            conexion.ConnectionString = @"Data Source=PC-PC\SQLDEVELOPER;Initial Catalog=db_facturas;Integrated Security=True";
            conexion.Open();
            SqlCommand comando = new SqlCommand();
            comando.Connection = conexion;
            comando.CommandType = CommandType.StoredProcedure;
            comando.CommandText = "SP_PROXIMO_ID";

            SqlParameter param = new SqlParameter("@next", SqlDbType.Int);
            param.Direction = ParameterDirection.Output;
            comando.Parameters.Add(param);

            comando.ExecuteNonQuery();
            conexion.Close();
            return (int)param.Value;


        }

        private void CargarCombo()
        {
            //completar...

            SqlConnection conexion = new SqlConnection();
            conexion.ConnectionString = @"Data Source=PC-PC\SQLDEVELOPER;Initial Catalog=db_facturas;Integrated Security=True";
            conexion.Open();
            SqlCommand comando = new SqlCommand();
            comando.Connection = conexion;
            comando.CommandType = CommandType.StoredProcedure;
            comando.CommandText = "SP_CONSULTAR_PRODUCTOS";
            DataTable tabla = new DataTable();
            tabla.Load(comando.ExecuteReader());
            conexion.Close();

            cboProducto.DataSource = tabla;
            cboProducto.DisplayMember = "n_producto";
            cboProducto.ValueMember = "id_producto";
        }
        double total = 0;
       
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cboProducto.Text.Equals(String.Empty))
            {
                MessageBox.Show("Debe seleccionar un producto...", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (txtCliente.Text.Equals(String.Empty))
            {
                MessageBox.Show("Debe introducir un cliente...", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (cboForma.Text.Equals(String.Empty))
            {
                MessageBox.Show("Debe seleccionar una forma de pago...", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            
            if (string.IsNullOrEmpty(nudCantidad.Text) || !int.TryParse(nudCantidad.Text, out _))

            {
                MessageBox.Show("Debe ingresar una cantidad valida...", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            foreach (DataGridViewRow row in dgvDetalles.Rows)
            {
                if (row.Cells["producto"].Value.ToString().Equals(cboProducto.Text))
                {
                    MessageBox.Show("Este producto ya esta presupuestado...", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            DataRowView item = (DataRowView)cboProducto.SelectedItem;
            int prod = Convert.ToInt32(item.Row.ItemArray[0]);
            string nom = Convert.ToString(item.Row.ItemArray[1]);
            double pre = Convert.ToDouble(item.Row.ItemArray[2]);
            int cant = Convert.ToInt32(nudCantidad.Text);
            double subtotal = cant * pre;

            Producto p = new Producto(prod, nom, pre);
            DetalleFactura detalle = new DetalleFactura(p, cant);

            nuevo.AgregarDetalle(detalle);

            dgvDetalles.Rows.Add(new object[] { prod, nom, pre, cant, subtotal });

            
            total = nuevo.CalcularTotal();
            lblTotal.Text = "Total " + total.ToString();
            lblSubtotal.Text = "Subtotal " + total.ToString();
            lblDescuento.Text = "Descuento 0";
        }

        private bool ExisteProductoEnGrilla(string text)
        {
            foreach (DataGridViewRow fila in dgvDetalles.Rows)
            {
                if (fila.Cells["producto"].Value.Equals(text))
                    return true;
            }
            return false;
        }

       



        private void dgvDetalles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDetalles.CurrentCell.ColumnIndex == 5)
            {
                nuevo.QuitarDetalle(dgvDetalles.CurrentRow.Index);
                dgvDetalles.Rows.Remove(dgvDetalles.CurrentRow);

                total = nuevo.CalcularTotal();
                lblTotal.Text = "Total " + total.ToString();
                lblSubtotal.Text = "Subtotal " + total.ToString();
                lblDescuento.Text = "Descuento 0";
            }
        }
    }
}
