using Microsoft.Office.Interop.Word;
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
using static WindowsFormsApp2.Form1;
using BCrypt.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp2
{
    public partial class Form2 : Form
    {
        public static class DatabaseConfig
        {
            public static string ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Flughafen;Integrated Security=true;";
        }
        public Form2()
        {
            InitializeComponent();
            Password.KeyDown += new KeyEventHandler(textBox2_KeyDown);
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Password.UseSystemPasswordChar = !checkBox2.Checked;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Form3 loginform = new Form3();
            loginform.ShowDialog();
            this.Show();
        }
        public string token;

        private string accountexists(string login)
        {
            using (SqlConnection sqlConnection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                sqlConnection.Open();
                string cmd = "SELECT password FROM Accounts WHERE Login = @Login OR mail = @Login";
                using (SqlCommand command = new SqlCommand(cmd, sqlConnection))
                {
                    command.Parameters.AddWithValue("@Login", login);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader["password"].ToString();
                        }
                        else
                        {
                            return "ERROR_Bad_Input";
                        }
                    }
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (Login.Text == ""&&Password.Text=="")
            {
                AdminMenuForm adminMenuForm = new AdminMenuForm();
                this.Hide();
                adminMenuForm.FormClosed += (s, args) => this.Close();
                adminMenuForm.ShowDialog();
            }
            string res = accountexists(Login.Text);
            if (res == "ERROR_Bad_Input")
            {
                label3.Text = "Login or password not existent";
                return;
            }

            if (BCrypt.Net.BCrypt.Verify(Password.Text, res))
            {
                var tokenService = new TokenService(DatabaseConfig.ConnectionString);
                user currentUser = new user();
                currentUser = currentUser.GetUserByLogin(Login.Text);
                if (currentUser._2fa == true)
                {
                    string _2fatoken = tokenService.GenerateToken(currentUser.id, "2FA");
                    EmailSender emailSender = new EmailSender();
                    emailSender.SendEmail(currentUser.email,"Your 2FA verification code", "Your code is "+Environment.NewLine+_2fatoken);
                    Form4 form4 = new Form4(_2fatoken, currentUser.email);
                    form4.ShowDialog();
                    if(form4.DialogResult != DialogResult.OK) { label3.Text="Failed to confirm 2FA";
                        return;
                    }

                }
                
                Form1 formm = new Form1(currentUser);
                formm.ShowDialog();
                this.Close();
            }
        }
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Suppress the "ding" sound
                e.SuppressKeyPress = true;

                // Call the Login button click event
                button1_Click(this, new EventArgs());
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
