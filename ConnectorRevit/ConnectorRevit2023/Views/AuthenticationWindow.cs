using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calc.ConnectorRevit.Views
{
    public partial class StringInputDialog : Form
    {
        public string DirectusUrl { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

        public StringInputDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtDirectusUrl.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter all required fields.");
                return;
            }

            DirectusUrl = txtDirectusUrl.Text;
            Email = txtEmail.Text;
            Password = txtPassword.Text;

            DialogResult = DialogResult.OK;
        }

        private void InitializeComponent()
        {
            this.txtDirectusUrl = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblDirectusUrl = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblDirectusUrl
            this.lblDirectusUrl.AutoSize = true;
            this.lblDirectusUrl.Location = new System.Drawing.Point(12, 15);
            this.lblDirectusUrl.Name = "lblDirectusUrl";
            this.lblDirectusUrl.Size = new System.Drawing.Size(32, 13);
            this.lblDirectusUrl.TabIndex = 0;
            this.lblDirectusUrl.Text = "Url:";

            // txtDirectusUrl
            this.txtDirectusUrl.Location = new System.Drawing.Point(64, 12);
            this.txtDirectusUrl.Name = "txtDirectusUrl";
            this.txtDirectusUrl.Size = new System.Drawing.Size(200, 20);
            this.txtDirectusUrl.TabIndex = 1;

            // lblEmail
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(12, 41);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(38, 13);
            this.lblEmail.TabIndex = 2;
            this.lblEmail.Text = "Email:";

            // txtEmail
            this.txtEmail.Location = new System.Drawing.Point(64, 38);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(200, 20);
            this.txtEmail.TabIndex = 3;

            // lblPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(12, 67);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Password:";

            // txtPassword
            this.txtPassword.Location = new System.Drawing.Point(64, 64);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(200, 20);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.UseSystemPasswordChar = true;

            // btnOK
            this.btnOK.Location = new System.Drawing.Point(12, 90);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);

            // StringInputDialog
            this.TopMost = true;
            this.ClientSize = new System.Drawing.Size(276, 125);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.txtDirectusUrl);
            this.Controls.Add(this.lblDirectusUrl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StringInputDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "String Input Dialog";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox txtDirectusUrl;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblDirectusUrl;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Button btnOK;
    }
}
