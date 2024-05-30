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
    public partial class FreeSpace : Form
    {
        public FreeSpace()
        {
            InitializeComponent();
            OutputFlightsByUnfilledTickets();
        }
        private void OutputFlightsByUnfilledTickets()
        {
            // SQL query to retrieve flights along with the count of unfilled tickets
            string query = @"
                SELECT Flights.FlightID, Flights.FlightCode, Flights.AirportCode, Flights.DepartureTime, Flights.Plane, 
                       COUNT(Tickets.TicketID) AS UnfilledTickets
                FROM Flights
                LEFT JOIN Tickets ON Flights.FlightID = Tickets.FlightID
                WHERE Tickets.AccountID IS NULL
                GROUP BY Flights.FlightID, Flights.FlightCode, Flights.AirportCode, Flights.DepartureTime, Flights.Plane
                ORDER BY UnfilledTickets DESC";

            DataTable flightsTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(flightsTable);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while retrieving flights: " + ex.Message);
                        return;
                    }
                }
            }

            // Display the flights data (e.g., in a DataGridView)
            dataGridView1.DataSource = flightsTable;
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
