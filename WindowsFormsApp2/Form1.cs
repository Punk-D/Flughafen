using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Windows.Forms;
using MailKit.Net.Smtp;
using MimeKit;
using static WindowsFormsApp2.Form2;
using static WindowsFormsApp2.AddFlight;

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
            public bool verified;
            public bool _2fa;

            public static string connectionString = ("Server=(localdb)\\MSSQLLocalDB;Database=Flughafen;Integrated Security=true;");

            public string BookTicket(int flightID, string passengerName, string passengerSurname)
            {
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                    {
                        sqlConnection.Open();

                        // Update the first ticket with NULL AccountID for the given flight
                        string updateTicketCmd = @"
                UPDATE TOP (1) Tickets
                SET AccountID = @UserID,
                    PassengerName = @PassengerName,
                    PassengerSurname = @PassengerSurname
                WHERE FlightID = @FlightID
                AND AccountID IS NULL";

                        using (SqlCommand command = new SqlCommand(updateTicketCmd, sqlConnection))
                        {
                            command.Parameters.AddWithValue("@FlightID", flightID);
                            command.Parameters.AddWithValue("@UserID", this.id);
                            command.Parameters.AddWithValue("@PassengerName", passengerName);
                            command.Parameters.AddWithValue("@PassengerSurname", passengerSurname);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                return "OK";
                            }
                            else
                            {
                                return "ERROR_No_ticket_found";
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

            public user GetUserByLogin(string login)
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    string cmdText = "SELECT ID, Login, password, mail, verified, _2fa FROM Accounts WHERE Login = @Login OR mail = @Login";

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
                                    bool bverified = Convert.ToBoolean(reader["verified"]);
                                    bool b_2fa = Convert.ToBoolean(reader["_2fa"]);
                                
                                user foundUser = new user();
                                foundUser.id = sid;
                                foundUser.email = eemail;
                                foundUser.login = llogin;
                                foundUser.password = ppassword;
                                foundUser.verified = bverified;
                                foundUser._2fa = b_2fa;

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
                        verified = @Verified,
                        _2fa = @TwoFA
                    WHERE
                        Login = @Login";

                        using (SqlCommand command = new SqlCommand(cmdText, sqlConnection))
                        {
                            command.Parameters.AddWithValue("@Login", this.login);
                            command.Parameters.AddWithValue("@Password", (this.password)); // Hashing the password before storing
                            command.Parameters.AddWithValue("@Email", this.email);
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
        public user userr;
        public Form1(user userr)
        {
            InitializeComponent();
            
            this.userr = userr;
            dateTimePicker7.Value = dateTimePicker7.MinDate;
            dateTimePicker1.Value = dateTimePicker1.MinDate;
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

        private void GetFlightSchedule(DateTime startDate, DateTime endDate, string destination)
        {
            // SQL query to select flight schedule between two dates and flying to the destination
            string query = "SELECT * FROM Flights WHERE DepartureTime BETWEEN @StartDate AND @EndDate AND AirportCode = @Destination";

            DataTable flightScheduleTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    command.Parameters.AddWithValue("@Destination", destination);

                    try
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(flightScheduleTable);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while fetching flight schedule: " + ex.Message);
                        return;
                    }
                }
            }

            // Display the flight schedule
            dataGridView1.DataSource = flightScheduleTable;
        }

        private void GetFlightsToDestination(string destination)
        {
            // SQL query to select all records of flights to the destination sorted by date
            string query = "SELECT * FROM Flights WHERE AirportCode = @Destination ORDER BY DepartureTime";

            DataTable flightsToDestinationTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Destination", destination);

                    try
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(flightsToDestinationTable);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while fetching flights to destination: " + ex.Message);
                        return;
                    }
                }
            }

            // Display the flights to destination sorted by date
            dataGridView1.DataSource = flightsToDestinationTable;
        }

        private void dateTimePicker7_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value==dateTimePicker1.MinDate&&dateTimePicker7.Value==dateTimePicker7.MinDate)
            {
                GetFlightsToDestination(comboBox1.Text);
            }
            else
            {
                GetFlightSchedule(dateTimePicker1.Value.Date, dateTimePicker7.Value.Date, comboBox1.Text);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Check if a valid row index is clicked
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                int flightCode = (int)selectedRow.Cells["FlightID"].Value; // Assuming FlightCode is the column name

                // Open the BookFlightForm with the selected flight code
                BookFlightForm bookFlightForm = new BookFlightForm(flightCode, userr);
                bookFlightForm.ShowDialog();
            }
        }
    }
}