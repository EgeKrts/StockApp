using System;
using System.Collections;
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
    public partial class FrmNewRecord : Form
    {
        public FrmNewRecord()
        {
            InitializeComponent();
        }


        sqlbaglantisi baglanti = new sqlbaglantisi();

        private void FrmNewRecord_Load(object sender, EventArgs e)
        {

        }

        private void btnNewRow_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();
        }

      

        private void btnSaveOrder_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Boş satırları atla
                if (row.IsNewRow) continue;

                string stockCode = row.Cells["StockCode"].Value?.ToString();
                string stockName = row.Cells["StockName"].Value?.ToString();
                decimal unitPrice, amount, rowPrice;

                // Gerekli hücrelerin değerlerini decimal olarak alabiliyorsak işleme devam et
                if (decimal.TryParse(row.Cells["UnitPrice"].Value?.ToString(), out unitPrice) &&
                    decimal.TryParse(row.Cells["Amount"].Value?.ToString(), out amount) &&
                    decimal.TryParse(row.Cells["RowPrice"].Value?.ToString(), out rowPrice))
                {
                    // Tbl_Orders tablosuna ekleme yapacak SQL sorgusunu oluşturun
                    string query = "INSERT INTO Tbl_Orders (StockCode, StockName, UnitPrice, Amount, RowPrice, EvrakNo, Date) " +
                                   "VALUES (@StockCode, @StockName, @UnitPrice, @Amount, @RowPrice, @EvrakNo, @Date)";

                    SqlCommand command = new SqlCommand(query, baglanti.connection());

                  
                    command.Parameters.AddWithValue("@StockCode", stockCode);
                    command.Parameters.AddWithValue("@StockName", stockName);
                    command.Parameters.AddWithValue("@UnitPrice", unitPrice);
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@RowPrice", rowPrice);
                    command.Parameters.AddWithValue("@EvrakNo", txtEvrakNo.Text);
                    command.Parameters.AddWithValue("@Date", dateTimePicker1.Text);

                    // Komutu çalıştırın
                    command.ExecuteNonQuery();

                    MessageBox.Show("Sipariş eklendi.");
                }
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Eğer değişiklik yapılan hücre "StockCode" sütunu ise
            if (e.ColumnIndex == dataGridView1.Columns[1].Index && e.RowIndex != -1)
            {
                string stockCode = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["StockCode"].Value);

                
                    string query = "SELECT Name, UnitPrice FROM Tbl_Books WHERE Code = @StockCode";
                    SqlCommand command = new SqlCommand(query, baglanti.connection());
                    command.Parameters.AddWithValue("@StockCode", stockCode);

                    
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        // Okunan verileri DataGridView hücrelerine yerleştir
                        dataGridView1.Rows[e.RowIndex].Cells["StockName"].Value = reader["Name"].ToString();
                        dataGridView1.Rows[e.RowIndex].Cells["UnitPrice"].Value = reader["UnitPrice"].ToString();
                    }

                UpdateTotalPrice();

                reader.Close();
                
            }

            
            if ((e.ColumnIndex == dataGridView1.Columns[1].Index ||
                e.ColumnIndex == dataGridView1.Columns[2].Index ||
                e.ColumnIndex == dataGridView1.Columns[3].Index) &&
                e.RowIndex != -1)
            {
                
                string stockCode = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells[1].Value);
                decimal unitPrice, amount, rowPrice;

                // Hücredeki değerleri decimal olarak alabiliyorsak işleme devam et
                if (decimal.TryParse(Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells[2].Value), out unitPrice) &&
                    decimal.TryParse(Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells[3].Value), out amount))
                {
                    // RowPrice hesaplaması
                    rowPrice = unitPrice * amount;

                    // RowPrice hücresine değeri yerleştir
                    dataGridView1.Rows[e.RowIndex].Cells[4].Value = rowPrice.ToString();


                    UpdateTotalPrice();
                }
                else
                {
                    
                }
            }
        }


        private void UpdateTotalPrice()
        {
            decimal totalPrice = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Boş satırları atla
                if (row.IsNewRow) continue;

                decimal rowPrice;

                // Gerekli hücrelerin değerlerini decimal olarak alabiliyorsak işleme devam et
                if (decimal.TryParse(row.Cells["RowPrice"].Value?.ToString(), out rowPrice))
                {
                    // Güncel satırın değerini totalPrice'a ekle
                    totalPrice += rowPrice;
                }
            }

            
            lblPrice.Text = totalPrice.ToString();
        }
    }
}



