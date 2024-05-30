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
    public partial class Delete_flight : Form
    {
        public Delete_flight()
        {
            InitializeComponent();
            PopulateFlightCodesComboBox(comboBox1);
        }

        public static bool DeleteFlightAndTickets(string flightCode, string cancelReason)
        {
            // SQL query to insert the flight details into CanceledFlights table and delete the flight and its associated tickets
            string query = @"
            BEGIN TRY
                BEGIN TRANSACTION;

                DECLARE @FlightID INT;
                SELECT @FlightID = FlightID FROM Flights WHERE FlightCode = @FlightCode;

                -- Check if the flight exists
                IF @FlightID IS NOT NULL
                BEGIN
                    -- Insert canceled flight details into CanceledFlights table
                    INSERT INTO CanceledFlights (FlightCode, Reason)
                    VALUES (@FlightCode, @CancelReason);

                    -- Delete tickets associated with the flight
                    DELETE FROM Tickets WHERE FlightID = @FlightID;

                    -- Delete the flight
                    DELETE FROM Flights WHERE FlightID = @FlightID;

                    COMMIT TRANSACTION;
                    
                END
                ELSE
                BEGIN
                    ROLLBACK TRANSACTION;
                    
                END
            END TRY
            BEGIN CATCH
                IF @@TRANCOUNT > 0
                    ROLLBACK TRANSACTION;
                
            END CATCH";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FlightCode", flightCode);
                    command.Parameters.AddWithValue("@CancelReason", cancelReason);

                    try
                    {
                        connection.Open();
                        command.ExecuteScalar();

                            return true; // Deletion successful
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while deleting the flight: " + ex.Message);
                        return false; // Deletion unsuccessful
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DeleteFlightAndTickets(comboBox1.Text, textBox1.Text);
        }

        public void PopulateFlightCodesComboBox(ComboBox comboBox)
        {
            // SQL query to select all flight codes
            string query = "SELECT FlightCode FROM Flights";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        // Clear existing items in the ComboBox
                        comboBox.Items.Clear();

                        // Loop through the result set and add flight codes to the ComboBox
                        while (reader.Read())
                        {
                            comboBox.Items.Add(reader["FlightCode"].ToString());
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while populating flight codes: " + ex.Message);
                    }
                }
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
