using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Windows.Forms;
using MailKit.Net.Smtp;
using MimeKit;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public class EmailSender
        {
            private const string SmtpServer = "smtp.gmail.com";
            private const int Port = 465; // Port number may vary based on your SMTP server configuration
            private const string SenderEmail = "etlgruppe.noreply@gmail.com";
            private const string SenderPassword = "bzfm vinl lfdf nqjb";

            public void SendEmail(string recipientEmail, string subject, string body)
            {
                try
                {
                    // Create a new MimeMessage
                    MimeMessage message = new MimeMessage();

                    // Add sender and recipient addresses
                    message.From.Add(new MailboxAddress("Münich Flughafen GmbH", SenderEmail));
                    message.To.Add(new MailboxAddress("Recipient",recipientEmail));

                    // Set the message subject and body
                    message.Subject = subject;
                    message.Body = new TextPart("html") { Text = body };

                    // Connect to the SMTP server
                    using (SmtpClient client = new SmtpClient())
                    {
                        client.Connect(SmtpServer, Port, true);

                        // Authenticate with the SMTP server
                        client.Authenticate(SenderEmail, SenderPassword);

                        // Send the message
                        client.Send(message);

                        // Disconnect from the SMTP server
                        client.Disconnect(true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                    // You can log the exception or handle it as needed
                }
            }
        }

        public class user
        {
            public int id;
            public string login;
            public string password;
            public string email;
            public int LOA;
            public bool verified;
            public bool _2fa;

            public static string connectionString = ("Server=(localdb)\\MSSQLLocalDB;Database=Flughafen;Integrated Security=true;");

            public user GetUserByLogin(string login)
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    string cmdText = "SELECT ID, Login, password, mail, LOA, verified, _2fa FROM Accounts WHERE Login = @Login OR mail = @Login";

                    using (SqlCommand command = new SqlCommand(cmdText, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@Login", login);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                
                                
                                    int sid = reader.GetInt32(0);
                                    string llogin = reader["Login"].ToString();
                                    string ppassword = reader["password"].ToString();
                                    string eemail = reader["mail"].ToString();
                                    int LLOA = Convert.ToInt32(reader["LOA"]);
                                    bool bverified = Convert.ToBoolean(reader["verified"]);
                                    bool b_2fa = Convert.ToBoolean(reader["_2fa"]);
                                
                                user foundUser = new user();
                                foundUser.id = sid;
                                foundUser.email = eemail;
                                foundUser.login = llogin;
                                foundUser.password = ppassword;
                                foundUser.verified = bverified;
                                foundUser._2fa = b_2fa;
                                foundUser.LOA = LLOA;

                                return foundUser;
                            }
                            else
                            {
                                return null; // User not found
                            }
                        }
                    }
                }
            }

            public string UpdateUser()
            {
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                    {
                        sqlConnection.Open();

                        string cmdText = @"
                    UPDATE Accounts
                    SET 
                        password = @Password,
                        mail = @Email,
                        LOA = @LOA,
                        verified = @Verified,
                        _2fa = @TwoFA
                    WHERE
                        Login = @Login";

                        using (SqlCommand command = new SqlCommand(cmdText, sqlConnection))
                        {
                            command.Parameters.AddWithValue("@Login", this.login);
                            command.Parameters.AddWithValue("@Password", (this.password)); // Hashing the password before storing
                            command.Parameters.AddWithValue("@Email", this.email);
                            command.Parameters.AddWithValue("@LOA", this.LOA);
                            command.Parameters.AddWithValue("@Verified", this.verified);
                            command.Parameters.AddWithValue("@TwoFA", this._2fa);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                return "OK";
                            }
                            else
                            {
                                return "ERROR_User_not_found";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (ex) if needed
                    return "ERROR_Database_issue";
                }
            }

            public string RegisterUser()
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    // Check if login is unique
                    string checkLoginCmd = "SELECT COUNT(*) FROM Accounts WHERE Login = @Login";
                    using (SqlCommand command = new SqlCommand(checkLoginCmd, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@Login", this.login);
                        int loginCount = (int)command.ExecuteScalar();

                        if (loginCount > 0)
                        {
                            return "ERROR_Username_already_taken";
                        }
                    }

                    // Check if email is unique
                    string checkEmailCmd = "SELECT COUNT(*) FROM Accounts WHERE mail = @Email";
                    using (SqlCommand command = new SqlCommand(checkEmailCmd, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@Email", this.email);
                        int emailCount = (int)command.ExecuteScalar();

                        if (emailCount > 0)
                        {
                            return "ERROR_Email_already_taken";
                        }
                    }

                    // Insert the new user
                    string insertUserCmd = @"
                INSERT INTO Accounts (Login, password, mail) 
                VALUES (@Login, @Password, @Email)"; // Assuming a default LOA of 1 for simplicity

                    using (SqlCommand command = new SqlCommand(insertUserCmd, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@Login", this.login);
                        command.Parameters.AddWithValue("@Password", this.password); // Make sure to hash the password before saving in a real app
                        command.Parameters.AddWithValue("@Email", this.email);

                        command.ExecuteNonQuery();
                    }

                    return "OK";
                }
            }
        }
        public class TokenService
        {
            private readonly string _connectionString;

            public TokenService(string connectionString)
            {
                _connectionString = connectionString;
            }

            public string GenerateToken(int userId, string tokenType)
            {
                var token = Guid.NewGuid().ToString();
                var expirationTime = GetExpirationTime(tokenType);

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("INSERT INTO Tokens (code, ID_account, type, timeout) VALUES (@Token, @UserID, @TokenType, @ExpirationTime)", connection);
                    command.Parameters.AddWithValue("@Token", token);
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@TokenType", tokenType);
                    command.Parameters.AddWithValue("@ExpirationTime", expirationTime);
                    command.ExecuteNonQuery();
                }

                return token;
            }

            public bool ValidateToken(string token, string tokenType)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT ExpirationTime FROM Tokens WHERE Token = @Token AND TokenType = @TokenType", connection);
                    command.Parameters.AddWithValue("@Token", token);
                    command.Parameters.AddWithValue("@TokenType", tokenType);

                    var expirationTime = command.ExecuteScalar() as DateTime?;

                    if (expirationTime == null || expirationTime < DateTime.UtcNow)
                    {
                        return false; // Token is invalid or expired
                    }

                    return true;
                }
            }

            private DateTime GetExpirationTime(string tokenType)
            {
                switch (tokenType)
                {
                    case "Auth":
                        return DateTime.UtcNow.AddHours(5);

                    case "2FA":
                        return DateTime.UtcNow.AddMinutes(30);
                    case "Confirm":
                        return DateTime.UtcNow.AddMinutes(30);
                    case "PasswordReset":
                        return DateTime.UtcNow.AddMinutes(90);
                    case "AuthRem":
                        return DateTime.UtcNow.AddMonths(3);
                    default:
                        throw new ArgumentException("Invalid token type");
                }
            }
        }

        public class DatabaseManager
        {
            private readonly string connectionString;

            public System.Data.DataTable GetScheduleByDestinationAndTime(string destinationAirportCode, DateTime startTime, DateTime endTime)
            {
                System.Data.DataTable schedule = new System.Data.DataTable();

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand("dbo.GetScheduleByDestinationAndTime", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@DestinationAirportCode", destinationAirportCode);
                            command.Parameters.AddWithValue("@StartTime", startTime);
                            command.Parameters.AddWithValue("@EndTime", endTime);
                            connection.Open();

                            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                            {
                                adapter.Fill(schedule);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception
                    MessageBox.Show("Error: " + ex.Message);
                }

                return schedule;
            }

            public System.Data.DataTable GetTimetableByDestination(string destinationAirportCode)
            {
                System.Data.DataTable timetable = new System.Data.DataTable();

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand("dbo.GetTimetableByDestination", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@DestinationAirportCode", destinationAirportCode);
                            connection.Open();

                            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                            {
                                adapter.Fill(timetable);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception
                    Console.WriteLine("Error: " + ex.Message);
                }

                return timetable;
            }

            public String GetLongestFlight()
            {
                string longestFlight = "";
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand("SELECT TOP 1 FlightCode FROM Flights ORDER BY Distance DESC", connection))
                        {
                            connection.Open();
                            object result = command.ExecuteScalar();
                            if (result != DBNull.Value)
                            {
                                return longestFlight = result.ToString();
                            }
                            return longestFlight;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception
                    return longestFlight = ("Error: " + ex.Message);
                }
            }

            public decimal GetAverageTicketPriceByDestination(string destinationAirportCode)
            {
                decimal averagePrice = 0;

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand("GetAverageTicketPriceByDestination", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@DestinationAirportCode", destinationAirportCode);
                            connection.Open();
                            object result = command.ExecuteScalar();
                            if (result != DBNull.Value)
                            {
                                averagePrice = Convert.ToDecimal(result);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception
                    MessageBox.Show("Error: " + ex.Message);
                }

                return averagePrice;
            }

            public DatabaseManager(string connectionString)
            {
                this.connectionString = connectionString;
            }

            public void CreateWeekdayFlightTable(int weekday)
            {
                try
                {
                    string weekdayName = GetWeekdayName(weekday);
                    string tableName = "Flights_" + weekdayName;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("CreateWeekdayFlightTable", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Weekday", weekday);
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Table created successfully: " + tableName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("FillWeekdayFlightTable", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Weekday", weekday);
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Table populated successfully: " + tableName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error creating table: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private string GetWeekdayName(int weekday)
            {
                switch (weekday)
                {
                    case 0: return "Monday";
                    case 1: return "Tuesday";
                    case 2: return "Wednesday";
                    case 3: return "Thursday";
                    case 4: return "Friday";
                    case 5: return "Saturday";
                    case 6: return "Sunday";
                    default: throw new ArgumentException("Invalid weekday number.");
                }
            }

            public string GetAllAirportCodes()
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT AirportCode FROM Airports";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        string airportCodes = "";
                        while (reader.Read())
                        {
                            airportCodes += reader["AirportCode"].ToString() + Environment.NewLine;
                        }
                        return airportCodes;
                    }
                }
            }

            public string GetAllAirplaneCodes()
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT PlaneCode FROM Planes";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        string airplaneCodes = "";
                        while (reader.Read())
                        {
                            airplaneCodes += reader["PlaneCode"].ToString() + Environment.NewLine;
                        }
                        return airplaneCodes;
                    }
                }
            }

            public string GetAllFlightCodes()
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT FlightCode FROM Flights";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        string flightCodes = "";
                        while (reader.Read())
                        {
                            flightCodes += reader["FlightCode"].ToString() + Environment.NewLine;
                        }
                        return flightCodes;
                    }
                }
            }

            public void RegisterNewFlight(string flightCode, string destination, string planeCode, DateTime departure)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // First, retrieve the PlaneID based on the PlaneCode
                    int planeId;
                    string planeIdQuery = "SELECT PlaneID FROM Planes WHERE PlaneCode = @PlaneCode";
                    using (SqlCommand planeIdCommand = new SqlCommand(planeIdQuery, connection))
                    {
                        planeIdCommand.Parameters.AddWithValue("@PlaneCode", planeCode);
                        connection.Open();
                        // ExecuteScalar is used to retrieve a single value from the query result
                        planeId = (int)planeIdCommand.ExecuteScalar();
                    }

                    // Now that we have the PlaneID, we can insert the new flight record
                    string query = "INSERT INTO Flights (FlightCode, Destination, Plane, DepartureTime) " +
                                   "VALUES (@FlightCode, @Destination, @PlaneID, @Departure)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FlightCode", flightCode);
                        command.Parameters.AddWithValue("@Destination", destination);
                        command.Parameters.AddWithValue("@PlaneID", planeId); // Use PlaneID instead of PlaneCode
                        command.Parameters.AddWithValue("@Departure", departure);
                        command.ExecuteNonQuery();
                    }
                }
            }

            public void RegisterPassengerForFlight(string flightCode, string passengerName, string passengerSurname, decimal ticketPrice)
            {
                try
                {
                    // Retrieve the FlightID associated with the FlightCode
                    int flightId;
                    string flightIdQuery = "SELECT FlightID FROM Flights WHERE FlightCode = @FlightCode";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand flightIdCommand = new SqlCommand(flightIdQuery, connection))
                        {
                            flightIdCommand.Parameters.AddWithValue("@FlightCode", flightCode);
                            connection.Open();
                            flightId = (int)flightIdCommand.ExecuteScalar();
                        }
                    }

                    // Check if there are free seats for the flight
                    if (GetFreeSeatsByFlightCode(flightCode) > 0)
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            string query = "INSERT INTO Tickets (FlightID, PassengerName, PassengerSurname, Price) " +
                                           "VALUES (@FlightID, @PassengerName, @PassengerSurname, @TicketPrice)";
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@FlightID", flightId);
                                command.Parameters.AddWithValue("@PassengerName", passengerName);
                                command.Parameters.AddWithValue("@PassengerSurname", passengerSurname);
                                command.Parameters.AddWithValue("@TicketPrice", ticketPrice);
                                connection.Open();
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("No free seats available for the flight.");
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error registering passenger for flight: " + ex.Message);
                }
            }

            public int GetFreeSeatsByFlightCode(string flightCode)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "EXEC GetFreeSeatsByFlightCode @FlightCode ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FlightCode", flightCode);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }

            // Implement other methods as needed
        }

        private void FillComboBoxes()
        {
            // Fill comboBox3, comboBox2, comboBox7, comboBox9, comboBox5, and comboBox8 with airport codes
            FillComboBoxFromDatabase(comboBox2, databaseManager.GetAllAirportCodes());
            FillComboBoxFromDatabase(comboBox7, databaseManager.GetAllAirportCodes());

            FillComboBoxFromDatabase(comboBox5, databaseManager.GetAllAirportCodes());
            FillComboBoxFromDatabase(comboBox8, databaseManager.GetAllAirportCodes());

            // Fill comboBox4 with plane codes
            FillComboBoxFromDatabase(comboBox4, databaseManager.GetAllAirplaneCodes());

            // Fill comboBox1 with flight codes
            FillComboBoxFromDatabase(comboBox1, databaseManager.GetAllFlightCodes());
            FillComboBoxFromDatabase(comboBox9, databaseManager.GetAllFlightCodes());
        }

        private void FillComboBoxFromDatabase(System.Windows.Forms.ComboBox comboBox, string data)
        {
            // Clear existing items in the ComboBox
            comboBox.Items.Clear();
            // Split the data by newline character and add each item to the ComboBox
            comboBox.Items.AddRange(data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
        }

        public DatabaseManager databaseManager;
        public string token;
        public Form1()
        {
            Form2 Form22 = new Form2();
            Form22.ShowDialog();
            if (Form22.DialogResult == DialogResult.OK) { token = Form22.token; }
            else {
                MessageBox.Show("Failed to log in", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            InitializeComponent();
            databaseManager = new DatabaseManager("Server=(localdb)\\MSSQLLocalDB;Database=Flughafen;Integrated Security=true;");
            FillComboBoxes();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Combine date and time from DateTimePicker controls
                DateTime departureDateTime = dateTimePicker3.Value.Date + dateTimePicker4.Value.TimeOfDay;

                // Call the RegisterNewFlight method with the input values
                databaseManager.RegisterNewFlight(textBox1.Text, comboBox2.Text, comboBox4.Text, departureDateTime);

                // Flight registration successful
                label19.Text = "Flight registered successfully!";
            }
            catch (Exception ex)
            {
                // Flight registration failed, display error message
                label19.Text = "Error: " + ex.Message;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            FillComboBoxes();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FillComboBoxes();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            {
                // Get the input values from the form controls
                string flightCode = comboBox1.SelectedItem?.ToString();
                string name = textBox2.Text;
                string surname = textBox3.Text;
                decimal price = numericUpDown1.Value;

                // Check if all required fields are filled
                if (string.IsNullOrEmpty(flightCode) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
                {
                    label20.Text = "Please fill in all fields.";
                    return;
                }

                try
                {
                    // Call the RegisterPassengerForFlight function in the Database Manager
                    databaseManager.RegisterPassengerForFlight(flightCode, name, surname, price);
                    label20.Text = "Flight registered successfully!";
                }
                catch (Exception ex)
                {
                    // Display the result
                    label20.Text = "Error: " + ex.Message; ;
                }
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Get the selected index of comboBox6
                int selectedIndex = comboBox6.SelectedIndex;

                // Check if a valid index is selected (-1 represents no selection)
                if (selectedIndex != -1)
                {
                    // Get the selected weekday number (0 for Monday, 1 for Tuesday, etc.)
                    int weekday = selectedIndex;

                    // Create an instance of the DatabaseManager class

                    // Call the CreateWeekdayFlightTable method with the selected weekday
                    databaseManager.CreateWeekdayFlightTable(weekday);

                    // Update label16 with success message
                    label16.Text = "Table created successfully.";
                }
                else
                {
                    // Update label16 with error message for no selection
                    label16.Text = "Please select a weekday from the dropdown list.";
                }
            }
            catch (Exception ex)
            {
                // Update label16 with error message for exception
                label16.Text = "An error occurred: " + ex.Message;
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            string a = comboBox6.SelectedItem?.ToString();
            label16.Text = databaseManager.GetAverageTicketPriceByDestination(a).ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label16.Text = databaseManager.GetLongestFlight().ToString();
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            label16.Text = databaseManager.GetFreeSeatsByFlightCode(comboBox9.SelectedItem?.ToString()).ToString();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = databaseManager.GetTimetableByDestination(comboBox5.SelectedItem?.ToString());
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime DateTime1 = dateTimePicker6.Value.Date + dateTimePicker5.Value.TimeOfDay;
            DateTime DateTime2 = dateTimePicker2.Value.Date + dateTimePicker1.Value.TimeOfDay;
            string destination = comboBox8.SelectedItem?.ToString();
            dataGridView2.DataSource = databaseManager.GetScheduleByDestinationAndTime(destination, DateTime1, DateTime2);
        }
    }
}