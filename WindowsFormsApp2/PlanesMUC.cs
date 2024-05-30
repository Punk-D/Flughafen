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
using static WindowsFormsApp2.Form2;

namespace WindowsFormsApp2
{
    public partial class PlanesMUC : Form
    {
        public PlanesMUC()
        {
            InitializeComponent();
            CheckPlanesAtMUC();
        }
        private void CheckPlanesAtMUC()
        {
            // SQL query to select planes currently staying at MUC
            string query = "SELECT * FROM Planes WHERE CurrentLocation = 'MUC'";

            DataTable planesAtMUCTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(planesAtMUCTable);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while checking planes at MUC: " + ex.Message);
                        return;
                    }
                }
            }

            // Display the planes currently staying at MUC
            dataGridView1.DataSource = planesAtMUCTable;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
