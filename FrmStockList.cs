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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Proje_Hastane
{
    public partial class FrmStockList : Form
    {
        public FrmStockList()
        {
            InitializeComponent();
        }

        sqlbaglantisi baglanti = new sqlbaglantisi();

        private void btnSaveInfo_Click(object sender, EventArgs e)
        {
            SqlCommand komut = new SqlCommand("INSERT INTO Tbl_Books (Code,Name,UnitPrice) VALUES (@p1,@p2,@p3)", baglanti.connection());
            komut.Parameters.AddWithValue("@p1", txtStockCode.Text);
            komut.Parameters.AddWithValue("@p2", txtStockName.Text);
            komut.Parameters.AddWithValue("@p3", txtUnitPrice.Text);
            

            komut.ExecuteNonQuery();

            txtStockCode.Text = "";
            txtStockName.Text = "";
            txtUnitPrice.Text = "";

            // Close the database connection
            baglanti.connection().Close();

            // Refresh the data source of the dataGridView
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Tbl_Books", baglanti.connection());
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            baglanti.connection().Close();
            MessageBox.Show("Kayıt Gerçekleşmiştir.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FrmStockList_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Tbl_Books", baglanti.connection());
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void dataGridView1_RowDividerDoubleClick(object sender, DataGridViewRowDividerDoubleClickEventArgs e)
        {
         
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Seçili satırın indeksini alın
            int selectedRowIndex = e.RowIndex;

            // Seçili satırdaki verileri alın
            DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];
            string column1Value = selectedRow.Cells["Code"].Value.ToString(); // Sütun adını değiştirin
            string column2Value = selectedRow.Cells["Name"].Value.ToString(); // Sütun adını değiştirin
            string column3Value = selectedRow.Cells["UnitPrice"].Value.ToString(); // Sütun adını değiştirin

            // TextBox kontrollerine verileri yerleştirin
            txtStockCode.Text = column1Value;
            txtStockName.Text = column2Value;
            txtUnitPrice.Text = column3Value;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string code = txtStockCode.Text;
            string name = txtStockName.Text;
            string unitPrice = txtUnitPrice.Text;


            // UPDATE sorgusunu çalıştırın
            SqlCommand komut = new SqlCommand("UPDATE Tbl_Books SET Code = @p1, Name = @p2, UnitPrice = @p3 WHERE Code = @p4", baglanti.connection());
            komut.Parameters.AddWithValue("@p1", code);
            komut.Parameters.AddWithValue("@p2", name);
            komut.Parameters.AddWithValue("@p3", unitPrice);
            komut.Parameters.AddWithValue("@p4", code);

            komut.ExecuteNonQuery();

            // DataGridView'ı yenile
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Tbl_Books", baglanti.connection());
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            // Bir mesaj kutusu gösterin
            MessageBox.Show("Kayıt başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }
    }
}
