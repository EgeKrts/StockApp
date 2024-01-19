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
    public partial class FrmNewOrders : Form
    {


        private string selectedEvrakNo;
        private string selectedDate;
        private string selectedTotalPrice;

        sqlbaglantisi baglanti = new sqlbaglantisi();
        public FrmNewOrders(string evrakNo,string date,string totalPrice)
        {
            InitializeComponent();

            selectedEvrakNo = evrakNo;
            selectedDate = date;
            selectedTotalPrice = totalPrice;

            // DataGridView'nin otomatik satır oluşturmasını engelle
            dataGridView1.AllowUserToAddRows = false;

            LoadOrderDetails();

            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
        }

        private void LoadOrderDetails()
        {
            DataTable dtDetails = new DataTable();
            string query = "SELECT StockCode, StockName, UnitPrice, Amount, RowPrice FROM Tbl_Orders WHERE EvrakNo = @EvrakNo";

           
            
              
                SqlCommand command = new SqlCommand(query, baglanti.connection());
                command.Parameters.AddWithValue("@EvrakNo", selectedEvrakNo);

                SqlDataAdapter daDetails = new SqlDataAdapter(command);
                daDetails.Fill(dtDetails);

            txtEvrakNo.Text = selectedEvrakNo;
            dateTimePicker1.Text = selectedDate;

            label1.Text = selectedTotalPrice;

           
            dataGridView1.DataSource = dtDetails;
        }


        private void btnNewRow_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();

          

        }

        private void FrmNewOrders_Load(object sender, EventArgs e)
        {


        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView1.Columns["Amount"].Index)
            {
                UpdateTotalPrice();
            }

        }

        private void UpdateTotalPrice()
        {
            decimal total = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                
                decimal amount = Convert.ToDecimal(row.Cells["Amount"].Value);
                decimal unitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value);

                
                decimal rowPrice = amount * unitPrice;

                row.Cells["RowPrice"].Value = rowPrice;

               
                total += rowPrice;
            }

            label1.Text = total.ToString("C"); 
        }

        private void btnSaveOrder_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Boş satırları atla
                if (row.IsNewRow) continue;

                string stockCode = row.Cells["StockCode"].Value?.ToString();
                decimal amount,rowPrice;

                // Gerekli hücrelerin değerlerini decimal olarak alabiliyorsak işleme devam et
                if (decimal.TryParse(row.Cells["Amount"].Value?.ToString(), out amount)
                     &&
            decimal.TryParse(row.Cells["RowPrice"].Value?.ToString(), out rowPrice))
                {
                    // Tbl_Orders tablosunu güncelleyecek SQL sorgusunu oluşturun
                    string query = "UPDATE Tbl_Orders SET Amount = @Amount ,RowPrice = @RowPrice " +
                                   "WHERE EvrakNo = @EvrakNo AND StockCode = @StockCode";

                    SqlCommand command = new SqlCommand(query, baglanti.connection());

                    
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@RowPrice", rowPrice);
                    command.Parameters.AddWithValue("@EvrakNo", txtEvrakNo.Text);
                    command.Parameters.AddWithValue("@StockCode", stockCode);

                    // Komutu çalıştırın
                    command.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Sipariş Güncellendi.");
        }
    }

}

