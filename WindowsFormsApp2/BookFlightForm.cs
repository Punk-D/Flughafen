using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsFormsApp2.Form1;
using static WindowsFormsApp2.Form2;

namespace WindowsFormsApp2
{
    public partial class BookFlightForm : Form
    {
        int ticketsLeft;
        float price;
        int flightCode;
        user userr= new user();
        public BookFlightForm(int flightCode, user userr)
        {
            InitializeComponent();
            this.flightCode = flightCode;
            GetPriceAndTicketsLeft();

            // Update labels with the retrieved data
            label1.Text = "Price: " + price.ToString("C"); // Assuming price is in decimal format
            label2.Text = "Tickets Left: " + ticketsLeft.ToString();
            this.userr= userr;
        }
        private void GetPriceAndTicketsLeft()
        {
            // SQL query to retrieve price from Tickets table for the specified flight code
            string query = "SELECT TOP 1 Price, (SELECT COUNT(*) FROM Tickets WHERE FlightID = @FlightCode AND AccountID IS NULL) AS TicketsLeft FROM Tickets WHERE FlightID = @FlightCode ORDER BY Price DESC";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FlightCode", flightCode);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            price = (float)reader.GetDecimal(reader.GetOrdinal("Price"));
                            ticketsLeft = reader.GetInt32(reader.GetOrdinal("TicketsLeft"));
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while retrieving flight details: " + ex.Message);
                    }
                }
            }
        }
        



        private void BookFlightForm_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(userr.BookTicket(flightCode, textBox1.Text, textBox2.Text) == "OK")
            {
                MessageBox.Show("Booked successfully", "Success");
            }
        }
    }
}
