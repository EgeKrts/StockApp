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

namespace Proje_Hastane
{
    public partial class FrmOrderList : Form
    {
        public FrmOrderList()
        {
            InitializeComponent();
        }

        sqlbaglantisi baglanti = new sqlbaglantisi();

        private void btnNewOrder_Click(object sender, EventArgs e)
        {
            
            FrmNewRecord frmNewRecord = new FrmNewRecord();
            frmNewRecord.Show();
        }

        private void btnStockList_Click(object sender, EventArgs e)
        {
            FrmStockList frmStockList = new FrmStockList();
            frmStockList.Show();
        }

        private void FrmOrderList_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string query = "SELECT EvrakNo, MAX(Date) AS Date, SUM(UnitPrice * Amount) AS TotalRowPrice " +
                           "FROM Tbl_Orders " +
                           "GROUP BY EvrakNo, Date";

            SqlDataAdapter da = new SqlDataAdapter(query, baglanti.connection());
            da.Fill(dt);
            dataGridView1.DataSource = dt;



        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int selectedRowIndex = e.RowIndex;

            
            DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];
            string evrakNo = selectedRow.Cells["EvrakNo"].Value.ToString(); 
            string date = selectedRow.Cells["Date"].Value.ToString(); 
            string totalPrice = selectedRow.Cells["TotalRowPrice"].Value.ToString(); 

            // FrmOrderDetails formunu açmak ve verileri yüklemek için
            FrmNewOrders frmOrderDetails = new FrmNewOrders(evrakNo,date,totalPrice);
            frmOrderDetails.ShowDialog();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // DataGridView'i güncelle
            DataTable dt = new DataTable();
            string query = "SELECT EvrakNo, MAX(Date) AS Date, SUM(UnitPrice * Amount) AS TotalRowPrice " +
                           "FROM Tbl_Orders " +
                           "GROUP BY EvrakNo, Date";

            SqlDataAdapter da = new SqlDataAdapter(query, baglanti.connection());
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }
    }
}
