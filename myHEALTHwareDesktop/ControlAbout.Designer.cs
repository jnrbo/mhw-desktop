namespace myHEALTHwareDesktop
{
	partial class ControlAbout
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.linkLicense = new System.Windows.Forms.LinkLabel();
			this.groupBoxAboutAcknowledgements = new System.Windows.Forms.GroupBox();
			this.linkLabelRestSharp = new System.Windows.Forms.LinkLabel();
			this.linkLabelCEFSharp = new System.Windows.Forms.LinkLabel();
			this.linkCommandLine = new System.Windows.Forms.LinkLabel();
			this.linkGostScript = new System.Windows.Forms.LinkLabel();
			this.linkmfilemon = new System.Windows.Forms.LinkLabel();
			this.labelAboutCopyright = new System.Windows.Forms.Label();
			this.labelAboutVersion = new System.Windows.Forms.Label();
			this.labelAboutProductName = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.toolTipPortMonitor = new System.Windows.Forms.ToolTip(this.components);
			this.toolTipGhostScript = new System.Windows.Forms.ToolTip(this.components);
			this.toolTipComandLine = new System.Windows.Forms.ToolTip(this.components);
			this.toolTipCEFSharp = new System.Windows.Forms.ToolTip(this.components);
			this.toolTipRestSharp = new System.Windows.Forms.ToolTip(this.components);
			this.toolTipGPL = new System.Windows.Forms.ToolTip(this.components);
			this.groupBoxAboutAcknowledgements.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// linkLicense
			// 
			this.linkLicense.AutoSize = true;
			this.linkLicense.Location = new System.Drawing.Point(6, 106);
			this.linkLicense.Name = "linkLicense";
			this.linkLicense.Size = new System.Drawing.Size(109, 13);
			this.linkLicense.TabIndex = 10;
			this.linkLicense.TabStop = true;
			this.linkLicense.Text = "Licensed via GPL 2.0";
			this.linkLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.aboutGNULicense_LinkClicked);
			// 
			// groupBoxAboutAcknowledgements
			// 
			this.groupBoxAboutAcknowledgements.Controls.Add(this.linkLabelRestSharp);
			this.groupBoxAboutAcknowledgements.Controls.Add(this.linkLabelCEFSharp);
			this.groupBoxAboutAcknowledgements.Controls.Add(this.linkCommandLine);
			this.groupBoxAboutAcknowledgements.Controls.Add(this.linkGostScript);
			this.groupBoxAboutAcknowledgements.Controls.Add(this.linkmfilemon);
			this.groupBoxAboutAcknowledgements.Location = new System.Drawing.Point(248, 50);
			this.groupBoxAboutAcknowledgements.Name = "groupBoxAboutAcknowledgements";
			this.groupBoxAboutAcknowledgements.Size = new System.Drawing.Size(219, 126);
			this.groupBoxAboutAcknowledgements.TabIndex = 9;
			this.groupBoxAboutAcknowledgements.TabStop = false;
			this.groupBoxAboutAcknowledgements.Text = "Acknowledgements";
			// 
			// linkLabelRestSharp
			// 
			this.linkLabelRestSharp.AutoSize = true;
			this.linkLabelRestSharp.Location = new System.Drawing.Point(39, 98);
			this.linkLabelRestSharp.Name = "linkLabelRestSharp";
			this.linkLabelRestSharp.Size = new System.Drawing.Size(57, 13);
			this.linkLabelRestSharp.TabIndex = 6;
			this.linkLabelRestSharp.TabStop = true;
			this.linkLabelRestSharp.Text = "RestSharp";
			this.linkLabelRestSharp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.aboutRestSharp_LinkClicked);
			// 
			// linkLabelCEFSharp
			// 
			this.linkLabelCEFSharp.AutoSize = true;
			this.linkLabelCEFSharp.Location = new System.Drawing.Point(39, 79);
			this.linkLabelCEFSharp.Name = "linkLabelCEFSharp";
			this.linkLabelCEFSharp.Size = new System.Drawing.Size(55, 13);
			this.linkLabelCEFSharp.TabIndex = 5;
			this.linkLabelCEFSharp.TabStop = true;
			this.linkLabelCEFSharp.Text = "CEFSharp";
			this.linkLabelCEFSharp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.aboutCEFSharp_LinkClicked);
			// 
			// linkCommandLine
			// 
			this.linkCommandLine.AutoSize = true;
			this.linkCommandLine.Location = new System.Drawing.Point(39, 60);
			this.linkCommandLine.Name = "linkCommandLine";
			this.linkCommandLine.Size = new System.Drawing.Size(144, 13);
			this.linkCommandLine.TabIndex = 4;
			this.linkCommandLine.TabStop = true;
			this.linkCommandLine.Text = "Command Line Parser Library";
			this.linkCommandLine.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.aboutCommandLine_LinkClicked);
			// 
			// linkGostScript
			// 
			this.linkGostScript.AutoSize = true;
			this.linkGostScript.Location = new System.Drawing.Point(39, 41);
			this.linkGostScript.Name = "linkGostScript";
			this.linkGostScript.Size = new System.Drawing.Size(62, 13);
			this.linkGostScript.TabIndex = 3;
			this.linkGostScript.TabStop = true;
			this.linkGostScript.Text = "GhostScript";
			this.linkGostScript.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.aboutGhostScript_LinkClicked);
			// 
			// linkmfilemon
			// 
			this.linkmfilemon.AutoSize = true;
			this.linkmfilemon.Location = new System.Drawing.Point(39, 22);
			this.linkmfilemon.Name = "linkmfilemon";
			this.linkmfilemon.Size = new System.Drawing.Size(108, 13);
			this.linkmfilemon.TabIndex = 2;
			this.linkmfilemon.TabStop = true;
			this.linkmfilemon.Text = "Multi File Port Monitor";
			this.linkmfilemon.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.aboutMFileMon_LinkClicked);
			// 
			// labelAboutCopyright
			// 
			this.labelAboutCopyright.AutoSize = true;
			this.labelAboutCopyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAboutCopyright.Location = new System.Drawing.Point(6, 85);
			this.labelAboutCopyright.Name = "labelAboutCopyright";
			this.labelAboutCopyright.Size = new System.Drawing.Size(193, 15);
			this.labelAboutCopyright.TabIndex = 8;
			this.labelAboutCopyright.Text = "Copyright (c) SOAPware, Inc. 2016";
			// 
			// labelAboutVersion
			// 
			this.labelAboutVersion.AutoSize = true;
			this.labelAboutVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAboutVersion.Location = new System.Drawing.Point(6, 66);
			this.labelAboutVersion.Name = "labelAboutVersion";
			this.labelAboutVersion.Size = new System.Drawing.Size(68, 15);
			this.labelAboutVersion.TabIndex = 7;
			this.labelAboutVersion.Text = "Version 1.0";
			// 
			// labelAboutProductName
			// 
			this.labelAboutProductName.AutoSize = true;
			this.labelAboutProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAboutProductName.ForeColor = System.Drawing.Color.Firebrick;
			this.labelAboutProductName.Location = new System.Drawing.Point(71, 18);
			this.labelAboutProductName.Name = "labelAboutProductName";
			this.labelAboutProductName.Size = new System.Drawing.Size(261, 26);
			this.labelAboutProductName.TabIndex = 6;
			this.labelAboutProductName.Text = "myHEALTHware Desktop";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::myHEALTHwareDesktop.Properties.Resources.MHWIcon_2Color;
			this.pictureBox1.Location = new System.Drawing.Point(9, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(60, 60);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 11;
			this.pictureBox1.TabStop = false;
			// 
			// toolTipPortMonitor
			// 
			this.toolTipPortMonitor.ShowAlways = true;
			// 
			// toolTipGhostScript
			// 
			this.toolTipGhostScript.ShowAlways = true;
			// 
			// toolTipComandLine
			// 
			this.toolTipComandLine.ShowAlways = true;
			// 
			// toolTipCEFSharp
			// 
			this.toolTipCEFSharp.ShowAlways = true;
			// 
			// toolTipRestSharp
			// 
			this.toolTipRestSharp.ShowAlways = true;
			// 
			// toolTipGPL
			// 
			this.toolTipGPL.ShowAlways = true;
			// 
			// ControlAbout
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.linkLicense);
			this.Controls.Add(this.labelAboutCopyright);
			this.Controls.Add(this.labelAboutVersion);
			this.Controls.Add(this.labelAboutProductName);
			this.Controls.Add(this.groupBoxAboutAcknowledgements);
			this.Name = "ControlAbout";
			this.Size = new System.Drawing.Size(492, 183);
			this.Load += new System.EventHandler(this.ControlAbout_Load);
			this.groupBoxAboutAcknowledgements.ResumeLayout(false);
			this.groupBoxAboutAcknowledgements.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.LinkLabel linkLicense;
		private System.Windows.Forms.GroupBox groupBoxAboutAcknowledgements;
		private System.Windows.Forms.LinkLabel linkGostScript;
		private System.Windows.Forms.LinkLabel linkmfilemon;
		private System.Windows.Forms.Label labelAboutCopyright;
		private System.Windows.Forms.Label labelAboutVersion;
		private System.Windows.Forms.Label labelAboutProductName;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.LinkLabel linkCommandLine;
		private System.Windows.Forms.LinkLabel linkLabelRestSharp;
		private System.Windows.Forms.LinkLabel linkLabelCEFSharp;
		private System.Windows.Forms.ToolTip toolTipPortMonitor;
		private System.Windows.Forms.ToolTip toolTipGhostScript;
		private System.Windows.Forms.ToolTip toolTipComandLine;
		private System.Windows.Forms.ToolTip toolTipCEFSharp;
		private System.Windows.Forms.ToolTip toolTipRestSharp;
		private System.Windows.Forms.ToolTip toolTipGPL;
	}
}
