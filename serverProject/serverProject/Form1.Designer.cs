namespace serverProject
{
    partial class Form1
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
            this.box_port = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.serverLogs = new System.Windows.Forms.RichTextBox();
            this.button_listen = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // box_port
            // 
            this.box_port.Location = new System.Drawing.Point(95, 66);
            this.box_port.Name = "box_port";
            this.box_port.Size = new System.Drawing.Size(203, 20);
            this.box_port.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Port:";
            // 
            // serverLogs
            // 
            this.serverLogs.Enabled = false;
            this.serverLogs.Location = new System.Drawing.Point(95, 107);
            this.serverLogs.Name = "serverLogs";
            this.serverLogs.Size = new System.Drawing.Size(377, 282);
            this.serverLogs.TabIndex = 2;
            this.serverLogs.Text = "";
            // 
            // button_listen
            // 
            this.button_listen.Location = new System.Drawing.Point(374, 64);
            this.button_listen.Name = "button_listen";
            this.button_listen.Size = new System.Drawing.Size(75, 23);
            this.button_listen.TabIndex = 3;
            this.button_listen.Text = "Listen";
            this.button_listen.UseVisualStyleBackColor = true;
            this.button_listen.Click += new System.EventHandler(this.button_listen_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 440);
            this.Controls.Add(this.button_listen);
            this.Controls.Add(this.serverLogs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.box_port);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox box_port;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox serverLogs;
        private System.Windows.Forms.Button button_listen;
    }
}

