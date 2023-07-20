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

        private bool userClosing = false;

        public StringInputDialog(string defaultDirectusUrl = "", string defaultEmail = "", string defaultPassword = "")
        {
            InitializeComponent();

            txtDirectusUrl.Text = defaultDirectusUrl;
            txtEmail.Text = defaultEmail;
            txtPassword.Text = defaultPassword;
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

        private void StringInputDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !userClosing)
            {
                // Handle the user closing the form
                DialogResult = DialogResult.Cancel;
            }
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
            // 
            // txtDirectusUrl
            // 
            this.txtDirectusUrl.Location = new System.Drawing.Point(133, 9);
            this.txtDirectusUrl.Name = "txtDirectusUrl";
            this.txtDirectusUrl.Size = new System.Drawing.Size(230, 26);
            this.txtDirectusUrl.TabIndex = 1;
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(133, 41);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(230, 26);
            this.txtEmail.TabIndex = 3;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(133, 73);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(230, 26);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // lblDirectusUrl
            // 
            this.lblDirectusUrl.AutoSize = true;
            this.lblDirectusUrl.Location = new System.Drawing.Point(31, 15);
            this.lblDirectusUrl.Name = "lblDirectusUrl";
            this.lblDirectusUrl.Size = new System.Drawing.Size(33, 20);
            this.lblDirectusUrl.TabIndex = 0;
            this.lblDirectusUrl.Text = "Url:";
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(31, 47);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(52, 20);
            this.lblEmail.TabIndex = 2;
            this.lblEmail.Text = "Email:";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(31, 79);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(82, 20);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Password:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(133, 123);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(135, 34);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // StringInputDialog
            // 
            this.ClientSize = new System.Drawing.Size(400, 169);
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
            this.Text = "Directus Login";
            this.TopMost = true;
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
