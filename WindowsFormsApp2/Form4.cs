using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form4 : Form
    {
        public string token, mail;
        public Form4(string token, string email)
        {
            InitializeComponent();
            this.token = token;
            label2.Text = email;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == token)
            {
                this.DialogResult = DialogResult.OK; 
            }
            else { label1.Text = "Code incorrect"; }
        }
    }
}
