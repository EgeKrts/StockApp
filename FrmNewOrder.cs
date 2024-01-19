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
    public partial class FrmNewOrder : Form
    {
        public FrmNewOrder()
        {
            InitializeComponent();

            dataGridView1.AllowUserToAddRows = false;
        }

        sqlbaglantisi baglanti = new sqlbaglantisi();

        private void FrmNewOrder_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'stockProjectDBDataSet1.Tbl_Orders' table. You can move, or remove it, as needed.
           
            // TODO: This line of code loads data into the 'stockProjectDBDataSet.Tbl_Order' table. You can move, or remove it, as needed.

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("SELECT Id,StockName,StockCode,UnitPrice,Amount,RowPrice FROM Tbl_Orders", baglanti.connection());
            da.Fill(dt);
            dataGridView1.DataSource = dt;


        }

        private void btnNewRow_Click(object sender, EventArgs e)
        {
            // Yeni bir DataRow oluşturun
            DataRow newRow = ((DataTable)dataGridView1.DataSource).NewRow();

            // DataRow'ı istediğiniz değerlerle doldurun
            newRow["Id"] = 0; // Id sütunu otomatik artan bir sütunsa 0 olarak bırakabilirsiniz veya istediğiniz bir değeri atayabilirsiniz
            newRow["StockName"] = ""; // Varsayılan bir değer atayabilirsiniz
            newRow["StockCode"] = ""; // Varsayılan bir değer atayabilirsiniz
            newRow["UnitPrice"] = 0; // Varsayılan bir değer atayabilirsiniz
            newRow["Amount"] = 0; // Varsayılan bir değer atayabilirsiniz
            newRow["RowPrice"] = 0; // Varsayılan bir değer atayabilirsiniz

            // DataTable'a yeni satırı ekleyin
            ((DataTable)dataGridView1.DataSource).Rows.Add(newRow);

            // Eklenen satırı seçin
            int rowIndex = dataGridView1.Rows.Count - 1;
            dataGridView1.Rows[rowIndex].Selected = true;

            // Sadece StockCode ve Amount sütunlarına izin verin
            dataGridView1.Rows[rowIndex].Cells["StockName"].ReadOnly = true;
            dataGridView1.Rows[rowIndex].Cells["UnitPrice"].ReadOnly = true;
            dataGridView1.Rows[rowIndex].Cells["RowPrice"].ReadOnly = true;



        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Eğer değişiklik yapılan hücre "StockCode" sütunu ise
            if (e.ColumnIndex == dataGridView1.Columns["StockCode"].Index && e.RowIndex != -1)
            {
                string stockCode = dataGridView1.Rows[e.RowIndex].Cells["StockCode"].Value.ToString();

                // StockCode'a göre Tbl_Books tablosundan veriyi çek
                // Burada, baglanti.connection() fonksiyonunu ve SQL sorgusunu kendi projenize uygun şekilde düzenleyin.
               
               
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

                    reader.Close();
                
            }

            // Eğer değişiklik yapılan hücre "StockCode", "UnitPrice" veya "Amount" sütunu ise
            if ((e.ColumnIndex == dataGridView1.Columns["StockCode"].Index ||
                e.ColumnIndex == dataGridView1.Columns["UnitPrice"].Index ||
                e.ColumnIndex == dataGridView1.Columns["Amount"].Index) &&
                e.RowIndex != -1)
            {
                // Gerekli hücrelerin değerlerini al
                string stockCode = dataGridView1.Rows[e.RowIndex].Cells["StockCode"].Value.ToString();
                decimal unitPrice, amount, rowPrice;
                

                // Hücredeki değerleri decimal olarak alabiliyorsak işleme devam et
                if (decimal.TryParse(dataGridView1.Rows[e.RowIndex].Cells["UnitPrice"].Value.ToString(), out unitPrice) &&
                    decimal.TryParse(dataGridView1.Rows[e.RowIndex].Cells["Amount"].Value.ToString(), out amount))
                {
                    // RowPrice hesaplaması
                    rowPrice = unitPrice * amount;
                   

                    // RowPrice hücresine değeri yerleştir
                    dataGridView1.Rows[e.RowIndex].Cells["RowPrice"].Value = rowPrice.ToString();
                    
                }
                else
                {
                    // Hücredeki değerler decimal olarak alınamazsa veya çarpma işlemi başarısız olursa buraya düşebilirsiniz.
                    // Hata işlemlerini burada ekleyebilirsiniz.
                }

                
            }

        }


        


        private void btnSaveOrder_Click(object sender, EventArgs e)
        {


            // DataGridView'deki her satırı Tbl_Orders tablosuna ekleyin
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
                        string query = "INSERT INTO Tbl_Orders (StockCode, StockName, UnitPrice, Amount, RowPrice,EvrakNo,Date) " +
                                       "VALUES (@StockCode, @StockName, @UnitPrice, @Amount, @RowPrice,@EvrakNo,@Date)";

                        SqlCommand command = new SqlCommand(query, baglanti.connection());

                        // Parametreleri doğru değerlerle doldurun
                        command.Parameters.AddWithValue("@StockCode", stockCode);
                        command.Parameters.AddWithValue("@StockName", stockName);
                        command.Parameters.AddWithValue("@UnitPrice", unitPrice);
                        command.Parameters.AddWithValue("@Amount", amount);
                        command.Parameters.AddWithValue("@RowPrice", rowPrice);
                        command.Parameters.AddWithValue("@EvrakNo", txtEvrakNo.Text);
                        command.Parameters.AddWithValue("@Date", dateTimePicker1.Text);

                    // Komutu çalıştırın
                    command.ExecuteNonQuery();
                    
                }
                else
                {
                    // Hata işlemlerini burada ekleyebilirsiniz
                }
            }

            MessageBox.Show("Veriler başarıyla kaydedildi.");
        }

        
    }
    }

