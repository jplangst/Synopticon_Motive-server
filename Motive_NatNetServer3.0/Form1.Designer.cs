namespace NatNetServer
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
            this.components = new System.ComponentModel.Container();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuClear = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPause = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonBroadcastToWAMP = new System.Windows.Forms.Button();
            this.RadioMulticast = new System.Windows.Forms.RadioButton();
            this.RadioUnicast = new System.Windows.Forms.RadioButton();
            this.textBoxRouter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Local = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxLocal = new System.Windows.Forms.ComboBox();
            this.TimestampLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TimestampValue = new System.Windows.Forms.Label();
            this.TimecodeValue = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.DroppedFrameCountLabel = new System.Windows.Forms.Label();
            this.PollCheckBox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxWAMPPort = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.buttonConnectNatNet = new System.Windows.Forms.Button();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.bindingSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource2)).BeginInit();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.Location = new System.Drawing.Point(8, 321);
            this.listView1.Margin = new System.Windows.Forms.Padding(4);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(450, 230);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Time";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Message";
            this.columnHeader2.Width = 400;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuClear,
            this.menuPause});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(116, 52);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // menuClear
            // 
            this.menuClear.Name = "menuClear";
            this.menuClear.Size = new System.Drawing.Size(115, 24);
            this.menuClear.Text = "Clear";
            this.menuClear.Click += new System.EventHandler(this.menuClear_Click);
            // 
            // menuPause
            // 
            this.menuPause.CheckOnClick = true;
            this.menuPause.Name = "menuPause";
            this.menuPause.Size = new System.Drawing.Size(115, 24);
            this.menuPause.Text = "Pause";
            this.menuPause.Click += new System.EventHandler(this.menuPause_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.DarkGray;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(8, 321);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(450, 25);
            this.label1.TabIndex = 13;
            this.label1.Text = "Messages";
            // 
            // buttonBroadcastToWAMP
            // 
            this.buttonBroadcastToWAMP.Enabled = false;
            this.buttonBroadcastToWAMP.Location = new System.Drawing.Point(8, 197);
            this.buttonBroadcastToWAMP.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBroadcastToWAMP.Name = "buttonBroadcastToWAMP";
            this.buttonBroadcastToWAMP.Size = new System.Drawing.Size(249, 28);
            this.buttonBroadcastToWAMP.TabIndex = 11;
            this.buttonBroadcastToWAMP.Text = "Broadcast To WAMP";
            this.buttonBroadcastToWAMP.UseVisualStyleBackColor = true;
            this.buttonBroadcastToWAMP.Click += new System.EventHandler(this.buttonBroadcastToWAMP_Click);
            // 
            // RadioMulticast
            // 
            this.RadioMulticast.AutoCheck = false;
            this.RadioMulticast.AutoSize = true;
            this.RadioMulticast.Enabled = false;
            this.RadioMulticast.Location = new System.Drawing.Point(56, 94);
            this.RadioMulticast.Margin = new System.Windows.Forms.Padding(4);
            this.RadioMulticast.Name = "RadioMulticast";
            this.RadioMulticast.Size = new System.Drawing.Size(84, 21);
            this.RadioMulticast.TabIndex = 14;
            this.RadioMulticast.Text = "Multicast";
            this.RadioMulticast.UseVisualStyleBackColor = true;
            this.RadioMulticast.CheckedChanged += new System.EventHandler(this.RadioMulticast_CheckedChanged);
            // 
            // RadioUnicast
            // 
            this.RadioUnicast.AutoSize = true;
            this.RadioUnicast.Checked = true;
            this.RadioUnicast.Location = new System.Drawing.Point(176, 94);
            this.RadioUnicast.Margin = new System.Windows.Forms.Padding(4);
            this.RadioUnicast.Name = "RadioUnicast";
            this.RadioUnicast.Size = new System.Drawing.Size(76, 21);
            this.RadioUnicast.TabIndex = 15;
            this.RadioUnicast.TabStop = true;
            this.RadioUnicast.Text = "Unicast";
            this.RadioUnicast.UseVisualStyleBackColor = true;
            this.RadioUnicast.CheckedChanged += new System.EventHandler(this.RadioUnicast_CheckedChanged);
            // 
            // textBoxRouter
            // 
            this.textBoxRouter.Location = new System.Drawing.Point(56, 129);
            this.textBoxRouter.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxRouter.Name = "textBoxRouter";
            this.textBoxRouter.Size = new System.Drawing.Size(200, 22);
            this.textBoxRouter.TabIndex = 8;
            this.textBoxRouter.Text = "127.0.0.1";
            this.textBoxRouter.TextChanged += new System.EventHandler(this.textBoxServer_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 133);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Router";
            // 
            // Local
            // 
            this.Local.AutoSize = true;
            this.Local.Location = new System.Drawing.Point(4, 15);
            this.Local.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Local.Name = "Local";
            this.Local.Size = new System.Drawing.Size(42, 17);
            this.Local.TabIndex = 9;
            this.Local.Text = "Local";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 94);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 17);
            this.label3.TabIndex = 16;
            this.label3.Text = "Type";
            // 
            // comboBoxLocal
            // 
            this.comboBoxLocal.FormattingEnabled = true;
            this.comboBoxLocal.Location = new System.Drawing.Point(56, 11);
            this.comboBoxLocal.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxLocal.Name = "comboBoxLocal";
            this.comboBoxLocal.Size = new System.Drawing.Size(200, 24);
            this.comboBoxLocal.TabIndex = 17;
            // 
            // TimestampLabel
            // 
            this.TimestampLabel.AutoSize = true;
            this.TimestampLabel.Location = new System.Drawing.Point(4, 229);
            this.TimestampLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TimestampLabel.Name = "TimestampLabel";
            this.TimestampLabel.Size = new System.Drawing.Size(89, 17);
            this.TimestampLabel.TabIndex = 18;
            this.TimestampLabel.Text = "Timestamp : ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 278);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 17);
            this.label4.TabIndex = 19;
            this.label4.Text = "Timecode :";
            // 
            // TimestampValue
            // 
            this.TimestampValue.AutoSize = true;
            this.TimestampValue.Location = new System.Drawing.Point(128, 229);
            this.TimestampValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TimestampValue.Name = "TimestampValue";
            this.TimestampValue.Size = new System.Drawing.Size(56, 17);
            this.TimestampValue.TabIndex = 20;
            this.TimestampValue.Text = "<none>";
            // 
            // TimecodeValue
            // 
            this.TimecodeValue.AutoSize = true;
            this.TimecodeValue.Location = new System.Drawing.Point(128, 278);
            this.TimecodeValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TimecodeValue.Name = "TimecodeValue";
            this.TimecodeValue.Size = new System.Drawing.Size(56, 17);
            this.TimecodeValue.TabIndex = 21;
            this.TimecodeValue.Text = "<none>";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 250);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 17);
            this.label6.TabIndex = 22;
            this.label6.Text = "Dropped Frames : ";
            // 
            // DroppedFrameCountLabel
            // 
            this.DroppedFrameCountLabel.AutoSize = true;
            this.DroppedFrameCountLabel.Location = new System.Drawing.Point(128, 250);
            this.DroppedFrameCountLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DroppedFrameCountLabel.Name = "DroppedFrameCountLabel";
            this.DroppedFrameCountLabel.Size = new System.Drawing.Size(56, 17);
            this.DroppedFrameCountLabel.TabIndex = 23;
            this.DroppedFrameCountLabel.Text = "<none>";
            // 
            // PollCheckBox
            // 
            this.PollCheckBox.AutoSize = true;
            this.PollCheckBox.Location = new System.Drawing.Point(284, 92);
            this.PollCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.PollCheckBox.Name = "PollCheckBox";
            this.PollCheckBox.Size = new System.Drawing.Size(53, 21);
            this.PollCheckBox.TabIndex = 26;
            this.PollCheckBox.Text = "Poll";
            this.PollCheckBox.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(280, 133);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 17);
            this.label5.TabIndex = 27;
            this.label5.Text = "Port";
            // 
            // textBoxWAMPPort
            // 
            this.textBoxWAMPPort.Location = new System.Drawing.Point(323, 129);
            this.textBoxWAMPPort.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxWAMPPort.Name = "textBoxWAMPPort";
            this.textBoxWAMPPort.Size = new System.Drawing.Size(56, 22);
            this.textBoxWAMPPort.TabIndex = 28;
            this.textBoxWAMPPort.Text = "8080";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 54);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 17);
            this.label7.TabIndex = 29;
            this.label7.Text = "Server";
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(56, 50);
            this.textBoxServer.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(200, 22);
            this.textBoxServer.TabIndex = 30;
            this.textBoxServer.Text = "127.0.0.1";
            // 
            // buttonConnectNatNet
            // 
            this.buttonConnectNatNet.Location = new System.Drawing.Point(8, 161);
            this.buttonConnectNatNet.Margin = new System.Windows.Forms.Padding(4);
            this.buttonConnectNatNet.Name = "buttonConnectNatNet";
            this.buttonConnectNatNet.Size = new System.Drawing.Size(249, 28);
            this.buttonConnectNatNet.TabIndex = 31;
            this.buttonConnectNatNet.Text = "Connect";
            this.buttonConnectNatNet.UseVisualStyleBackColor = true;
            this.buttonConnectNatNet.Click += new System.EventHandler(this.buttonConnectNatNet_Click);
            // 
            // bindingSource2
            // 
            this.bindingSource2.CurrentChanged += new System.EventHandler(this.bindingSource2_CurrentChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(475, 559);
            this.Controls.Add(this.TimecodeValue);
            this.Controls.Add(this.DroppedFrameCountLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonConnectNatNet);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.TimestampValue);
            this.Controls.Add(this.textBoxServer);
            this.Controls.Add(this.textBoxWAMPPort);
            this.Controls.Add(this.textBoxRouter);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TimestampLabel);
            this.Controls.Add(this.buttonBroadcastToWAMP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PollCheckBox);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.Local);
            this.Controls.Add(this.comboBoxLocal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.RadioMulticast);
            this.Controls.Add(this.RadioUnicast);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "NatNet Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuClear;
        private System.Windows.Forms.ToolStripMenuItem menuPause;
        private System.Windows.Forms.Button buttonBroadcastToWAMP;
        private System.Windows.Forms.RadioButton RadioMulticast;
        private System.Windows.Forms.RadioButton RadioUnicast;
        private System.Windows.Forms.TextBox textBoxRouter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label Local;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxLocal;
        private System.Windows.Forms.Label TimestampLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label TimestampValue;
        private System.Windows.Forms.Label TimecodeValue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label DroppedFrameCountLabel;
        private System.Windows.Forms.CheckBox PollCheckBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxWAMPPort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.Button buttonConnectNatNet;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.BindingSource bindingSource2;
    }
}

