namespace Pixiv.Utilities.Ugoira.Convert.WebP
{
    partial class LoginDialog
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
            this.messageLabel1 = new System.Windows.Forms.Label();
            this.messageLabel2 = new System.Windows.Forms.Label();
            this.idField = new System.Windows.Forms.TextBox();
            this.pwField = new System.Windows.Forms.TextBox();
            this.loginButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // messageLabel1
            // 
            this.messageLabel1.AutoSize = true;
            this.messageLabel1.Location = new System.Drawing.Point(12, 16);
            this.messageLabel1.Name = "messageLabel1";
            this.messageLabel1.Size = new System.Drawing.Size(124, 12);
            this.messageLabel1.TabIndex = 0;
            this.messageLabel1.Text = "ID / Email address : ";
            // 
            // messageLabel2
            // 
            this.messageLabel2.AutoSize = true;
            this.messageLabel2.Location = new System.Drawing.Point(12, 43);
            this.messageLabel2.Name = "messageLabel2";
            this.messageLabel2.Size = new System.Drawing.Size(74, 12);
            this.messageLabel2.TabIndex = 1;
            this.messageLabel2.Text = "Password : ";
            // 
            // idField
            // 
            this.idField.Location = new System.Drawing.Point(142, 12);
            this.idField.Name = "idField";
            this.idField.Size = new System.Drawing.Size(230, 21);
            this.idField.TabIndex = 2;
            this.idField.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.field_KeyPress);
            // 
            // pwField
            // 
            this.pwField.Location = new System.Drawing.Point(142, 39);
            this.pwField.Name = "pwField";
            this.pwField.PasswordChar = '*';
            this.pwField.Size = new System.Drawing.Size(230, 21);
            this.pwField.TabIndex = 3;
            this.pwField.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.field_KeyPress);
            // 
            // loginButton
            // 
            this.loginButton.Location = new System.Drawing.Point(14, 66);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(358, 23);
            this.loginButton.TabIndex = 4;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // LoginDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 99);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.pwField);
            this.Controls.Add(this.idField);
            this.Controls.Add(this.messageLabel2);
            this.Controls.Add(this.messageLabel1);
            this.KeyPreview = true;
            this.Name = "LoginDialog";
            this.Text = "LoginDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoginDialog_FormClosing);
            this.Load += new System.EventHandler(this.LoginDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label messageLabel1;
        private System.Windows.Forms.Label messageLabel2;
        private System.Windows.Forms.TextBox idField;
        private System.Windows.Forms.TextBox pwField;
        private System.Windows.Forms.Button loginButton;

    }
}