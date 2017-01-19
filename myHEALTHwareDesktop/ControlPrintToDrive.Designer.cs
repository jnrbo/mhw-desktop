namespace myHEALTHwareDesktop
{
	public partial class ControlPrintToDrive
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
			this.buttonPrintToDriveInstall = new System.Windows.Forms.Button();
			this.textBoxPrintToDriveFolder = new System.Windows.Forms.TextBox();
			this.buttonBrowsePrintToDrivePath = new System.Windows.Forms.Button();
			this.radioButtonPrompt = new System.Windows.Forms.RadioButton();
			this.radioButtonUseDefault = new System.Windows.Forms.RadioButton();
			this.groupBoxPrintToDrive = new System.Windows.Forms.GroupBox();
			this.errorProviderDriveFolder = new System.Windows.Forms.ErrorProvider(this.components);
			this.pictureStartedStopped = new System.Windows.Forms.PictureBox();
			this.labelMonitorStatus = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.errorProviderDriveFolder)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureStartedStopped)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonPrintToDriveInstall
			// 
			this.buttonPrintToDriveInstall.Location = new System.Drawing.Point(429, 146);
			this.buttonPrintToDriveInstall.Name = "buttonPrintToDriveInstall";
			this.buttonPrintToDriveInstall.Size = new System.Drawing.Size(75, 23);
			this.buttonPrintToDriveInstall.TabIndex = 31;
			this.buttonPrintToDriveInstall.Text = "Install Printer";
			this.buttonPrintToDriveInstall.UseVisualStyleBackColor = true;
			this.buttonPrintToDriveInstall.Click += new System.EventHandler(this.ButtonPrintToDriveInstallClick);
			// 
			// textBoxPrintToDriveFolder
			// 
			this.textBoxPrintToDriveFolder.Location = new System.Drawing.Point(118, 83);
			this.textBoxPrintToDriveFolder.Name = "textBoxPrintToDriveFolder";
			this.textBoxPrintToDriveFolder.ReadOnly = true;
			this.textBoxPrintToDriveFolder.Size = new System.Drawing.Size(332, 20);
			this.textBoxPrintToDriveFolder.TabIndex = 32;
			// 
			// buttonBrowsePrintToDrivePath
			// 
			this.buttonBrowsePrintToDrivePath.Location = new System.Drawing.Point(471, 82);
			this.buttonBrowsePrintToDrivePath.Name = "buttonBrowsePrintToDrivePath";
			this.buttonBrowsePrintToDrivePath.Size = new System.Drawing.Size(51, 23);
			this.buttonBrowsePrintToDrivePath.TabIndex = 34;
			this.buttonBrowsePrintToDrivePath.Text = "Browse";
			this.buttonBrowsePrintToDrivePath.UseVisualStyleBackColor = true;
			this.buttonBrowsePrintToDrivePath.Click += new System.EventHandler(this.ButtonBrowsePrintToDrivePathClick);
			// 
			// radioButtonPrompt
			// 
			this.radioButtonPrompt.AutoSize = true;
			this.radioButtonPrompt.Location = new System.Drawing.Point(38, 45);
			this.radioButtonPrompt.Name = "radioButtonPrompt";
			this.radioButtonPrompt.Size = new System.Drawing.Size(148, 17);
			this.radioButtonPrompt.TabIndex = 35;
			this.radioButtonPrompt.TabStop = true;
			this.radioButtonPrompt.Text = "Prompt for upload location";
			this.radioButtonPrompt.UseVisualStyleBackColor = true;
			this.radioButtonPrompt.CheckedChanged += new System.EventHandler(this.RadioButtonsCheckedChanged);
			// 
			// radioButtonUseDefault
			// 
			this.radioButtonUseDefault.AutoSize = true;
			this.radioButtonUseDefault.Location = new System.Drawing.Point(38, 84);
			this.radioButtonUseDefault.Name = "radioButtonUseDefault";
			this.radioButtonUseDefault.Size = new System.Drawing.Size(74, 17);
			this.radioButtonUseDefault.TabIndex = 36;
			this.radioButtonUseDefault.TabStop = true;
			this.radioButtonUseDefault.Text = "Upload to:";
			this.radioButtonUseDefault.UseVisualStyleBackColor = true;
			this.radioButtonUseDefault.CheckedChanged += new System.EventHandler(this.RadioButtonsCheckedChanged);
			// 
			// groupBoxPrintToDrive
			// 
			this.groupBoxPrintToDrive.Location = new System.Drawing.Point(9, 13);
			this.groupBoxPrintToDrive.Name = "groupBoxPrintToDrive";
			this.groupBoxPrintToDrive.Size = new System.Drawing.Size(539, 117);
			this.groupBoxPrintToDrive.TabIndex = 37;
			this.groupBoxPrintToDrive.TabStop = false;
			this.groupBoxPrintToDrive.Text = "With each print:";
			// 
			// errorProviderDriveFolder
			// 
			this.errorProviderDriveFolder.ContainerControl = this;
			// 
			// pictureStartedStopped
			// 
			this.pictureStartedStopped.Image = global::myHEALTHwareDesktop.Properties.Resources.stopped;
			this.pictureStartedStopped.Location = new System.Drawing.Point(381, 141);
			this.pictureStartedStopped.Name = "pictureStartedStopped";
			this.pictureStartedStopped.Size = new System.Drawing.Size(32, 32);
			this.pictureStartedStopped.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureStartedStopped.TabIndex = 39;
			this.pictureStartedStopped.TabStop = false;
			// 
			// label_MonitorStatus
			// 
			this.labelMonitorStatus.Location = new System.Drawing.Point(201, 151);
			this.labelMonitorStatus.Margin = new System.Windows.Forms.Padding(0);
			this.labelMonitorStatus.Name = "labelMonitorStatus";
			this.labelMonitorStatus.Size = new System.Drawing.Size(172, 13);
			this.labelMonitorStatus.TabIndex = 38;
			this.labelMonitorStatus.Text = "Print to Drive printer is not installed.";
			this.labelMonitorStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ControlPrintToDrive
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pictureStartedStopped);
			this.Controls.Add(this.labelMonitorStatus);
			this.Controls.Add(this.radioButtonUseDefault);
			this.Controls.Add(this.radioButtonPrompt);
			this.Controls.Add(this.textBoxPrintToDriveFolder);
			this.Controls.Add(this.buttonBrowsePrintToDrivePath);
			this.Controls.Add(this.buttonPrintToDriveInstall);
			this.Controls.Add(this.groupBoxPrintToDrive);
			this.Name = "ControlPrintToDrive";
			this.Size = new System.Drawing.Size(565, 186);
			((System.ComponentModel.ISupportInitialize)(this.errorProviderDriveFolder)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureStartedStopped)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.Button buttonPrintToDriveInstall;
		public System.Windows.Forms.TextBox textBoxPrintToDriveFolder;
		public System.Windows.Forms.Button buttonBrowsePrintToDrivePath;
		private System.Windows.Forms.RadioButton radioButtonPrompt;
		private System.Windows.Forms.RadioButton radioButtonUseDefault;
		private System.Windows.Forms.GroupBox groupBoxPrintToDrive;
		private System.Windows.Forms.ErrorProvider errorProviderDriveFolder;
		private System.Windows.Forms.PictureBox pictureStartedStopped;
		public System.Windows.Forms.Label labelMonitorStatus;
	}
}
