using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pixiv.Utilities.Ugoira.Convert.WebP
{
    public partial class LoginDialog : Form
    {
        private string id, password;
        public bool hidden 
        { 
            set 
            {
                if (value)
                {
                    WindowState = FormWindowState.Minimized;
                    ShowInTaskbar = false;
                }
                else
                {
                    WindowState = FormWindowState.Normal;
                    ShowInTaskbar = true;
                }
            } 
        }

        public LoginDialog()
        {
            InitializeComponent();
        }

        private void LoginDialog_Load(object sender, EventArgs e)
        {
            loginButton.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void LoginDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != System.Windows.Forms.DialogResult.OK)
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            this.id = idField.Text;
            this.password = pwField.Text;
        }

        private void field_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                loginButton.PerformClick();
        }

        public string getID()
        {
            return id;
        }

        public string getPassword()
        {
            return password;
        }

        public void clearLoginInfo()
        {
            idField.Text = pwField.Text = id = password = string.Empty;
        }
    }
}
