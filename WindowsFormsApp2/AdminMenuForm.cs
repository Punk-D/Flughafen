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
using static WindowsFormsApp2.AddFlight;

namespace WindowsFormsApp2
{
    public partial class AdminMenuForm : Form
    {
        public AdminMenuForm()
        {
            InitializeComponent();
            
            DataTable airportsTable = GetAirports();
            if (airportsTable != null)
            {
                comboBox1.Items.Clear(); // Clear existing items
                comboBox1.Items.Add("MUC");
                foreach (DataRow row in airportsTable.Rows)
                {
                    comboBox1.Items.Add(row["AirportCode"].ToString());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddFlight addFlight = new AddFlight();
            this.Hide();
            addFlight.ShowDialog();
            this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Delete_flight delete_Flight = new Delete_flight();
            this.Hide();
            delete_Flight.ShowDialog();
            this.Show();
        }
        public string GetFlightWithLongestDistance()
        {
            // SQL query to retrieve the flight with the longest distance
            string query = @"
            SELECT TOP 1
                FlightCode,
                Distance
            FROM
                Flights
            ORDER BY
                Distance DESC";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        // Check if any rows were returned
                        if (reader.HasRows)
                        {
                            reader.Read(); // Read the first row
                            string flightCode = reader["FlightCode"].ToString();
                            decimal distance = (decimal)reader["Distance"];

                            // Construct the string containing flight information
                            string flightInfo = $"Flight Code: {flightCode}, Distance: {distance} km";

                            reader.Close();
                            return flightInfo;
                        }
                        else
                        {
                            reader.Close();
                            return "No flights found";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while retrieving flight with longest distance: " + ex.Message);
                        return "Error occurred";
                    }
                }
            }
        }
        public decimal GetAverageTicketPriceToDestination(string destinationAirportCode)
        {
            // SQL query to calculate the average ticket price based on flight type and destination airport
            string query = @"
            SELECT AVG(Price) AS AveragePrice
            FROM Tickets
            INNER JOIN Flights ON Tickets.FlightID = Flights.FlightID
            WHERE ";

            // Determine the flight type based on the destination airport code
            int flightType = 0; // Default to incoming flights (type 0)

            // Check if the destination airport code is MUC
            if (destinationAirportCode.Equals("MUC", StringComparison.OrdinalIgnoreCase))
            {
                // For MUC, select all incoming flights (type 0)
                query += "Flights.Type = 0";
            }
            else
            {
                // For other airports, select outgoing flights (type 1) to the specified destination
                query += "Flights.Type = 1 AND Flights.AirportCode = @DestinationAirportCode";
            }

            decimal averagePrice = 0;

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameter if needed
                    if (!destinationAirportCode.Equals("MUC", StringComparison.OrdinalIgnoreCase))
                    {
                        command.Parameters.AddWithValue("@DestinationAirportCode", destinationAirportCode);
                    }

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            averagePrice = Convert.ToDecimal(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while calculating the average ticket price: " + ex.Message);
                    }
                }
            }

            return averagePrice;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text=GetFlightWithLongestDistance();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label1.Text = GetAverageTicketPriceToDestination(comboBox1.Text).ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Export export = new Export();
            export.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FreeSpace freeSpace = new FreeSpace();
            freeSpace.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            PlanesMUC planesMUC = new PlanesMUC();
            planesMUC.Show();
        }
    }
}
