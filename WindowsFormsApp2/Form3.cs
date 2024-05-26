using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsFormsApp2.Form1;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace WindowsFormsApp2
{
    public partial class Form3 : Form
    {
        public bool IsCompanyEmail(string email)
        {
            string pattern = @"^[a-zA-Z]+[a-zA-Z]+\.flugmungmbh@gmail.com$";
            return Regex.IsMatch(email, pattern);
        }
        private string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Flughafen;Integrated Security=true;";
        public void NotifyEmployees(string newEmployeeEmail)
        {
            List<string> employeeEmails = new List<string>();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                string cmd = @"
                SELECT a.mail 
                FROM Accounts a
                JOIN LOA l ON a.LOA = l.ID
                WHERE l.LOA_assignation = 1";

                using (SqlCommand command = new SqlCommand(cmd, sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employeeEmails.Add(reader["mail"].ToString());
                        }
                    }
                }
            }

            EmailSender emailSender = new EmailSender();

            foreach (string employeeEmail in employeeEmails)
            {
                string subject = "Review LOA for new employee";
                string body = $"Please review LOA for employee {newEmployeeEmail} as soon as possible.";

                emailSender.SendEmail(employeeEmail, subject, body);
            }
        }
        public static string CheckAndHashPassword(string password)
        {
            // Check if the password meets the safety criteria
            if (password.Length >= 8 &&
                Regex.IsMatch(password, @"[A-Z]") &&
                Regex.IsMatch(password, @"[a-z]") &&
                Regex.IsMatch(password, @"[0-9]") &&
                Regex.IsMatch(password, @"[\W_]"))
            {
                // Hash the password using bcrypt
                return BCrypt.Net.BCrypt.HashPassword(password);
            }
            else
            {
                return "ERROR_password_not_safe";
            }
        }

        public Form3()
        {
            InitializeComponent();
            
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !checkBox2.Checked;
            label6.Visible = !checkBox2.Checked;
            textBox4.Visible = !checkBox2.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            user newuser = new user();
            newuser.email=textBox1.Text;
            string haspass = CheckAndHashPassword(textBox2.Text);
            if (checkBox2.Checked&& haspass != "ERROR_password_not_safe")
            {
                newuser.password = haspass;
            }
            else if(textBox2.Text == textBox4.Text&& haspass != "ERROR_password_not_safe")
            {
                newuser.password = haspass;
            }
            else if(haspass== "ERROR_password_not_safe")
            {
                label3.Text = "Password not safe. Uppercase, lowercase, number and symbol";
            }
            newuser.login =textBox3.Text;
            string res = newuser.RegisterUser();
            MessageBox.Show("Registration successful", "Success!", MessageBoxButtons.OK);
            DialogResult ress = MessageBox.Show("Do you want to proceed to checking your Email address now?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ress == DialogResult.Yes)
            {
                TokenService tokenService = new TokenService(connectionString);
                newuser = newuser.GetUserByLogin(newuser.login);
                string token = tokenService.GenerateToken(newuser.id, "Confirm");
                EmailSender emailSender = new EmailSender();
                emailSender.SendEmail(newuser.email, "Your confirmation code", "Your confirmation code is"+Environment.NewLine+token);
                Form4 confirm = new Form4(token, newuser.email);
                confirm.ShowDialog();
                if (confirm.DialogResult != DialogResult.OK) { label3.Text = "Failed to confirm email, but registered"; }
                else { newuser._2fa = true; newuser.verified = true; newuser.UpdateUser(); }
            }
            if (IsCompanyEmail(textBox1.Text))
            {
                NotifyEmployees(textBox1.Text);
                MessageBox.Show("Your level of access will be reviewed soon", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
    }
}
