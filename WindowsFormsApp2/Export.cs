using ClosedXML.Excel;
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
    public partial class Export : Form
    {
        public Export()
        {
            InitializeComponent();
        }
        private void ExportFlightsToExcel(DateTime date)
        {
            // SQL query to select flights taking off at the received date
            string query = @"
            SELECT FlightCode, AirportCode, DepartureTime, Plane
            FROM Flights
            WHERE CONVERT(date, DepartureTime) = @Date";

            DataTable flightsTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", date);

                    try
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(flightsTable);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while exporting flights: " + ex.Message);
                        return;
                    }
                }
            }

            if (flightsTable.Rows.Count == 0)
            {
                MessageBox.Show("No flights found for the selected date.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Create a SaveFileDialog to choose the path to save the Excel file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            saveFileDialog.DefaultExt = "xlsx";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Title = "Save Flights to Excel";
            saveFileDialog.FileName = "Flights_" + date.ToString("yyyyMMdd_HHmmss") + ".xlsx";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Export flights data to Excel
                ExportDataTableToExcel(flightsTable, saveFileDialog.FileName);
                MessageBox.Show("Flights exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ExportDataTableToExcel(DataTable dataTable, string filePath)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable, "Flights");
                wb.SaveAs(filePath);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ExportFlightsToExcel(dateTimePicker1.Value.Date);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
