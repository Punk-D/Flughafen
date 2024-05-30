namespace WindowsFormsApp2
{
    partial class AddFlight
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddFlight));
            this.FlightCode = new System.Windows.Forms.TextBox();
            this.AirportCombo = new System.Windows.Forms.ComboBox();
            this.dateval = new System.Windows.Forms.DateTimePicker();
            this.timeval = new System.Windows.Forms.DateTimePicker();
            this.PlaneCombo = new System.Windows.Forms.ComboBox();
            this.outgoing = new System.Windows.Forms.RadioButton();
            this.incoming = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // FlightCode
            // 
            this.FlightCode.Location = new System.Drawing.Point(89, 103);
            this.FlightCode.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.FlightCode.Name = "FlightCode";
            this.FlightCode.Size = new System.Drawing.Size(218, 25);
            this.FlightCode.TabIndex = 0;
            // 
            // AirportCombo
            // 
            this.AirportCombo.FormattingEnabled = true;
            this.AirportCombo.Location = new System.Drawing.Point(89, 158);
            this.AirportCombo.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.AirportCombo.Name = "AirportCombo";
            this.AirportCombo.Size = new System.Drawing.Size(218, 25);
            this.AirportCombo.TabIndex = 1;
            this.AirportCombo.SelectedIndexChanged += new System.EventHandler(this.AirportCombo_SelectedIndexChanged);
            // 
            // dateval
            // 
            this.dateval.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateval.Location = new System.Drawing.Point(89, 223);
            this.dateval.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.dateval.Name = "dateval";
            this.dateval.Size = new System.Drawing.Size(102, 25);
            this.dateval.TabIndex = 2;
            // 
            // timeval
            // 
            this.timeval.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timeval.Location = new System.Drawing.Point(201, 223);
            this.timeval.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.timeval.Name = "timeval";
            this.timeval.Size = new System.Drawing.Size(106, 25);
            this.timeval.TabIndex = 3;
            // 
            // PlaneCombo
            // 
            this.PlaneCombo.FormattingEnabled = true;
            this.PlaneCombo.Location = new System.Drawing.Point(89, 290);
            this.PlaneCombo.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.PlaneCombo.Name = "PlaneCombo";
            this.PlaneCombo.Size = new System.Drawing.Size(218, 25);
            this.PlaneCombo.TabIndex = 4;
            // 
            // outgoing
            // 
            this.outgoing.AutoSize = true;
            this.outgoing.Checked = true;
            this.outgoing.Location = new System.Drawing.Point(89, 368);
            this.outgoing.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.outgoing.Name = "outgoing";
            this.outgoing.Size = new System.Drawing.Size(84, 21);
            this.outgoing.TabIndex = 5;
            this.outgoing.TabStop = true;
            this.outgoing.Text = "Outgoing";
            this.outgoing.UseVisualStyleBackColor = true;
            // 
            // incoming
            // 
            this.incoming.AutoSize = true;
            this.incoming.Location = new System.Drawing.Point(224, 368);
            this.incoming.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.incoming.Name = "incoming";
            this.incoming.Size = new System.Drawing.Size(83, 21);
            this.incoming.TabIndex = 6;
            this.incoming.Text = "Incoming";
            this.incoming.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Location = new System.Drawing.Point(150, 429);
            this.button1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 30);
            this.button1.TabIndex = 7;
            this.button1.Text = "Submit";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Nirmala UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label4.Location = new System.Drawing.Point(226, 513);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(163, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "©Flughafen MÜNCHEN GmbH";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Yu Gothic UI Light", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(89, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 17);
            this.label2.TabIndex = 17;
            this.label2.Text = "Flight code";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::WindowsFormsApp2.Properties.Resources.Munchen_flughafen;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(376, 56);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 18;
            this.pictureBox1.TabStop = false;
            // 
            // AddFlight
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(401, 535);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.incoming);
            this.Controls.Add(this.outgoing);
            this.Controls.Add(this.PlaneCombo);
            this.Controls.Add(this.timeval);
            this.Controls.Add(this.dateval);
            this.Controls.Add(this.AirportCombo);
            this.Controls.Add(this.FlightCode);
            this.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "AddFlight";
            this.Text = "Add Flight";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox FlightCode;
        private System.Windows.Forms.ComboBox AirportCombo;
        private System.Windows.Forms.DateTimePicker dateval;
        private System.Windows.Forms.DateTimePicker timeval;
        private System.Windows.Forms.ComboBox PlaneCombo;
        private System.Windows.Forms.RadioButton outgoing;
        private System.Windows.Forms.RadioButton incoming;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}