namespace TCPIPClient
{
    partial class frmClient
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmClient));
            this.buttonConnectToServer = new System.Windows.Forms.Button();
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.labelStatusInfo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxClientName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxServerListeningPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.imageListStatusLights = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.labelConnectionStuff = new System.Windows.Forms.Label();
            this.buttonSendDataToServer = new System.Windows.Forms.Button();
            this.textBoxText = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxNum1 = new System.Windows.Forms.TextBox();
            this.textBoxNum2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonConnectToServer
            // 
            this.buttonConnectToServer.Location = new System.Drawing.Point(129, 77);
            this.buttonConnectToServer.Name = "buttonConnectToServer";
            this.buttonConnectToServer.Size = new System.Drawing.Size(327, 23);
            this.buttonConnectToServer.TabIndex = 0;
            this.buttonConnectToServer.Text = "Connect To Server";
            this.buttonConnectToServer.UseVisualStyleBackColor = true;
            this.buttonConnectToServer.Click += new System.EventHandler(this.buttonConnectToServer_Click);
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(107, 23);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(173, 20);
            this.textBoxServer.TabIndex = 1;
            this.textBoxServer.Text = "localhost";
            // 
            // labelStatusInfo
            // 
            this.labelStatusInfo.AutoSize = true;
            this.labelStatusInfo.Location = new System.Drawing.Point(266, 106);
            this.labelStatusInfo.Name = "labelStatusInfo";
            this.labelStatusInfo.Size = new System.Drawing.Size(159, 13);
            this.labelStatusInfo.TabIndex = 2;
            this.labelStatusInfo.Text = "Click \'Connect to Server\'  button";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(104, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(166, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Address to the Server(name or IP)";
            // 
            // textBoxClientName
            // 
            this.textBoxClientName.Location = new System.Drawing.Point(325, 24);
            this.textBoxClientName.Name = "textBoxClientName";
            this.textBoxClientName.Size = new System.Drawing.Size(100, 20);
            this.textBoxClientName.TabIndex = 4;
            this.textBoxClientName.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(322, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Client Name";
            // 
            // textBoxServerListeningPort
            // 
            this.textBoxServerListeningPort.Location = new System.Drawing.Point(221, 46);
            this.textBoxServerListeningPort.Name = "textBoxServerListeningPort";
            this.textBoxServerListeningPort.Size = new System.Drawing.Size(50, 20);
            this.textBoxServerListeningPort.TabIndex = 6;
            this.textBoxServerListeningPort.Text = "9999";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(104, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Server\'s Listening Port:";
            // 
            // imageListStatusLights
            // 
            this.imageListStatusLights.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListStatusLights.ImageStream")));
            this.imageListStatusLights.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListStatusLights.Images.SetKeyName(0, "RED");
            this.imageListStatusLights.Images.SetKeyName(1, "GREEN");
            this.imageListStatusLights.Images.SetKeyName(2, "BLUE");
            this.imageListStatusLights.Images.SetKeyName(3, "PURPLE");
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(239, 106);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 24);
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Enabled = false;
            this.buttonDisconnect.Location = new System.Drawing.Point(462, 77);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(75, 23);
            this.buttonDisconnect.TabIndex = 9;
            this.buttonDisconnect.Text = "Disconnect";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // labelConnectionStuff
            // 
            this.labelConnectionStuff.AutoSize = true;
            this.labelConnectionStuff.Location = new System.Drawing.Point(12, 245);
            this.labelConnectionStuff.Name = "labelConnectionStuff";
            this.labelConnectionStuff.Size = new System.Drawing.Size(16, 13);
            this.labelConnectionStuff.TabIndex = 10;
            this.labelConnectionStuff.Text = "...";
            // 
            // buttonSendDataToServer
            // 
            this.buttonSendDataToServer.Enabled = false;
            this.buttonSendDataToServer.Location = new System.Drawing.Point(15, 137);
            this.buttonSendDataToServer.Name = "buttonSendDataToServer";
            this.buttonSendDataToServer.Size = new System.Drawing.Size(185, 23);
            this.buttonSendDataToServer.TabIndex = 11;
            this.buttonSendDataToServer.Text = "Send Data To Server";
            this.buttonSendDataToServer.UseVisualStyleBackColor = true;
            this.buttonSendDataToServer.Click += new System.EventHandler(this.buttonSendDataToServer_Click);
            // 
            // textBoxText
            // 
            this.textBoxText.Location = new System.Drawing.Point(12, 163);
            this.textBoxText.Multiline = true;
            this.textBoxText.Name = "textBoxText";
            this.textBoxText.Size = new System.Drawing.Size(692, 78);
            this.textBoxText.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(585, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Text to send:";
            // 
            // textBoxNum1
            // 
            this.textBoxNum1.Location = new System.Drawing.Point(710, 163);
            this.textBoxNum1.MaxLength = 10;
            this.textBoxNum1.Name = "textBoxNum1";
            this.textBoxNum1.Size = new System.Drawing.Size(81, 20);
            this.textBoxNum1.TabIndex = 14;
            this.textBoxNum1.Text = "123456";
            // 
            // textBoxNum2
            // 
            this.textBoxNum2.Location = new System.Drawing.Point(710, 221);
            this.textBoxNum2.MaxLength = 10;
            this.textBoxNum2.Name = "textBoxNum2";
            this.textBoxNum2.Size = new System.Drawing.Size(81, 20);
            this.textBoxNum2.TabIndex = 15;
            this.textBoxNum2.Text = "54321";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(709, 147);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Number to send:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(707, 205);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Another number";
            // 
            // frmClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(803, 262);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxNum2);
            this.Controls.Add(this.textBoxNum1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxText);
            this.Controls.Add(this.buttonSendDataToServer);
            this.Controls.Add(this.labelConnectionStuff);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxServerListeningPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxClientName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelStatusInfo);
            this.Controls.Add(this.textBoxServer);
            this.Controls.Add(this.buttonConnectToServer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmClient";
            this.Text = "TCPIP Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmClient_FormClosing);
            this.Load += new System.EventHandler(this.frmClient_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonConnectToServer;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.Label labelStatusInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxClientName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxServerListeningPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ImageList imageListStatusLights;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.Label labelConnectionStuff;
        private System.Windows.Forms.Button buttonSendDataToServer;
        private System.Windows.Forms.TextBox textBoxText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxNum1;
        private System.Windows.Forms.TextBox textBoxNum2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}

