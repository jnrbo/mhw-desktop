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
			this.linkLabelLicense = new System.Windows.Forms.LinkLabel();
			this.groupBoxAboutAcknowledgements = new System.Windows.Forms.GroupBox();
			this.linkLabelRestSharp = new System.Windows.Forms.LinkLabel();
			this.linkLabelCefSharp = new System.Windows.Forms.LinkLabel();
			this.linkLabelCommandLine = new System.Windows.Forms.LinkLabel();
			this.linkLLabelGhostScript = new System.Windows.Forms.LinkLabel();
			this.linkLabelMfilemon = new System.Windows.Forms.LinkLabel();
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
			// linkLabelLicense
			// 
			this.linkLabelLicense.AutoSize = true;
			this.linkLabelLicense.Location = new System.Drawing.Point(6, 106);
			this.linkLabelLicense.Name = "linkLabelLicense";
			this.linkLabelLicense.Size = new System.Drawing.Size(109, 13);
			this.linkLabelLicense.TabIndex = 10;
			this.linkLabelLicense.TabStop = true;
			this.linkLabelLicense.Text = "Licensed via GPL 2.0";
			this.linkLabelLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AboutGnuLicenseLinkClicked);
			// 
			// groupBoxAboutAcknowledgements
			// 
			this.groupBoxAboutAcknowledgements.Controls.Add(this.linkLabelRestSharp);
			this.groupBoxAboutAcknowledgements.Controls.Add(this.linkLabelCefSharp);
			this.groupBoxAboutAcknowledgements.Controls.Add(this.linkLabelCommandLine);
			this.groupBoxAboutAcknowledgements.Controls.Add(this.linkLLabelGhostScript);
			this.groupBoxAboutAcknowledgements.Controls.Add(this.linkLabelMfilemon);
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
			this.linkLabelRestSharp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AboutRestSharpLinkClicked);
			// 
			// linkLabelCefSharp
			// 
			this.linkLabelCefSharp.AutoSize = true;
			this.linkLabelCefSharp.Location = new System.Drawing.Point(39, 79);
			this.linkLabelCefSharp.Name = "linkLabelCefSharp";
			this.linkLabelCefSharp.Size = new System.Drawing.Size(55, 13);
			this.linkLabelCefSharp.TabIndex = 5;
			this.linkLabelCefSharp.TabStop = true;
			this.linkLabelCefSharp.Text = "CEFSharp";
			this.linkLabelCefSharp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AboutCefSharpLinkClicked);
			// 
			// linkLabelCommandLine
			// 
			this.linkLabelCommandLine.AutoSize = true;
			this.linkLabelCommandLine.Location = new System.Drawing.Point(39, 60);
			this.linkLabelCommandLine.Name = "linkLabelCommandLine";
			this.linkLabelCommandLine.Size = new System.Drawing.Size(144, 13);
			this.linkLabelCommandLine.TabIndex = 4;
			this.linkLabelCommandLine.TabStop = true;
			this.linkLabelCommandLine.Text = "Command Line Parser Library";
			this.linkLabelCommandLine.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AboutCommandLineLinkClicked);
			// 
			// linkLLabelGhostScript
			// 
			this.linkLLabelGhostScript.AutoSize = true;
			this.linkLLabelGhostScript.Location = new System.Drawing.Point(39, 41);
			this.linkLLabelGhostScript.Name = "linkLLabelGhostScript";
			this.linkLLabelGhostScript.Size = new System.Drawing.Size(62, 13);
			this.linkLLabelGhostScript.TabIndex = 3;
			this.linkLLabelGhostScript.TabStop = true;
			this.linkLLabelGhostScript.Text = "GhostScript";
			this.linkLLabelGhostScript.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AboutGhostScriptLinkClicked);
			// 
			// linkLabelMfilemon
			// 
			this.linkLabelMfilemon.AutoSize = true;
			this.linkLabelMfilemon.Location = new System.Drawing.Point(39, 22);
			this.linkLabelMfilemon.Name = "linkLabelMfilemon";
			this.linkLabelMfilemon.Size = new System.Drawing.Size(108, 13);
			this.linkLabelMfilemon.TabIndex = 2;
			this.linkLabelMfilemon.TabStop = true;
			this.linkLabelMfilemon.Text = "Multi File Port Monitor";
			this.linkLabelMfilemon.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AboutMFilemonLinkClicked);
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
			this.Controls.Add(this.linkLabelLicense);
			this.Controls.Add(this.labelAboutCopyright);
			this.Controls.Add(this.labelAboutVersion);
			this.Controls.Add(this.labelAboutProductName);
			this.Controls.Add(this.groupBoxAboutAcknowledgements);
			this.Name = "ControlAbout";
			this.Size = new System.Drawing.Size(492, 183);
			this.Load += new System.EventHandler(this.ControlAboutLoad);
			this.groupBoxAboutAcknowledgements.ResumeLayout(false);
			this.groupBoxAboutAcknowledgements.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.LinkLabel linkLabelLicense;
		private System.Windows.Forms.GroupBox groupBoxAboutAcknowledgements;
		private System.Windows.Forms.LinkLabel linkLLabelGhostScript;
		private System.Windows.Forms.LinkLabel linkLabelMfilemon;
		private System.Windows.Forms.Label labelAboutCopyright;
		private System.Windows.Forms.Label labelAboutVersion;
		private System.Windows.Forms.Label labelAboutProductName;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.LinkLabel linkLabelCommandLine;
		private System.Windows.Forms.LinkLabel linkLabelRestSharp;
		private System.Windows.Forms.LinkLabel linkLabelCefSharp;
		private System.Windows.Forms.ToolTip toolTipPortMonitor;
		private System.Windows.Forms.ToolTip toolTipGhostScript;
		private System.Windows.Forms.ToolTip toolTipComandLine;
		private System.Windows.Forms.ToolTip toolTipCEFSharp;
		private System.Windows.Forms.ToolTip toolTipRestSharp;
		private System.Windows.Forms.ToolTip toolTipGPL;
	}
}
