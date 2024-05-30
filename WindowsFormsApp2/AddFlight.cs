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
    public partial class AddFlight : Form
    {
        public static int AddFlightt(string flightCode, bool type, string destination, DateTime departureTime, string planeID)
        {

            // SQL query to insert a flight
            string query = @"
            INSERT INTO Flights (FlightCode, Type, AirportCode, DepartureTime, Plane)
            VALUES (@FlightCode, @Type, @Destination, @DepartureTime, @PlaneID)";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Adding parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@FlightCode", flightCode);
                    command.Parameters.AddWithValue("@Type", type);
                    command.Parameters.AddWithValue("@Destination", destination);
                    command.Parameters.AddWithValue("@DepartureTime", departureTime);
                    command.Parameters.AddWithValue("@PlaneID", planeID);

                    //try
                    {
                        connection.Open();

                        // Execute the query and get the inserted FlightID
                        command.ExecuteScalar();
                        return 1;
                    }
                    /*catch (Exception ex)
                    {
                        // Handle exceptions (e.g., log the error)
                        Console.WriteLine("An error occurred: " + ex.Message);
                        return -1; // Indicates an error
                    }*/
                }
            }
        }
        public void PopulateComboBoxes(ComboBox comboBoxAirports, ComboBox comboBoxPlanes)
        {
            // Populate Airport ComboBox
            DataTable airportsTable = GetAirports();
            if (airportsTable != null)
            {
                comboBoxAirports.Items.Clear(); // Clear existing items
                foreach (DataRow row in airportsTable.Rows)
                {
                    comboBoxAirports.Items.Add(row["AirportCode"].ToString());
                }
            }

            // Populate Plane ComboBox
            DataTable planesTable = GetPlanes();
            if (planesTable != null)
            {
                comboBoxPlanes.Items.Clear(); // Clear existing items
                foreach (DataRow row in planesTable.Rows)
                {
                    comboBoxPlanes.Items.Add(row["PlaneCode"].ToString());
                }
            }
        }


        public static DataTable GetAirports()
        {
            string query = "SELECT AirportCode FROM Airports";
            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while retrieving airport data: " + ex.Message);
                        return null;
                    }
                }
            }
        }

        private DataTable GetPlanes()
        {
            string query = "SELECT PlaneCode FROM Planes";
            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while retrieving plane data: " + ex.Message);
                        return null;
                    }
                }
            }
        }
        public AddFlight()
        {
            InitializeComponent();
            PopulateComboBoxes(AirportCombo, PlaneCombo);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime date = dateval.Value.Date ;
            DateTime time = timeval.Value;
            date = date.Add(time.TimeOfDay);
            if (AddFlightt(FlightCode.Text, outgoing.Checked, AirportCombo.Text, date, PlaneCombo.Text) != -1)
            {
                MessageBox.Show("Successfully added flight", "Success");
            };

        }

        private void AirportCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
