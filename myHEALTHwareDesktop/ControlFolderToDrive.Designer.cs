namespace myHEALTHwareDesktop
{
	public partial class ControlFolderToDrive
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
			this.label_MonitorStatus = new System.Windows.Forms.Label();
			this.buttonStartStopWatcher = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxLocalPath = new System.Windows.Forms.TextBox();
			this.buttonBrowseLocalPath = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxMHWFolder = new System.Windows.Forms.TextBox();
			this.deleteFileAfterUploadCheckBox = new System.Windows.Forms.CheckBox();
			this.buttonBrowseUploadPath = new System.Windows.Forms.Button();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.errorProviderLocalFolder = new System.Windows.Forms.ErrorProvider(this.components);
			this.pictureStartedStopped = new System.Windows.Forms.PictureBox();
			this.errorProviderDriveFolder = new System.Windows.Forms.ErrorProvider(this.components);
			((System.ComponentModel.ISupportInitialize)(this.errorProviderLocalFolder)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureStartedStopped)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProviderDriveFolder)).BeginInit();
			this.SuspendLayout();
			// 
			// label_MonitorStatus
			// 
			this.label_MonitorStatus.Location = new System.Drawing.Point(198, 114);
			this.label_MonitorStatus.Margin = new System.Windows.Forms.Padding(0);
			this.label_MonitorStatus.Name = "label_MonitorStatus";
			this.label_MonitorStatus.Size = new System.Drawing.Size(147, 13);
			this.label_MonitorStatus.TabIndex = 33;
			this.label_MonitorStatus.Text = "Uploader is currently stopped.";
			this.label_MonitorStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// buttonStartStopWatcher
			// 
			this.buttonStartStopWatcher.Location = new System.Drawing.Point(347, 109);
			this.buttonStartStopWatcher.Name = "buttonStartStopWatcher";
			this.buttonStartStopWatcher.Size = new System.Drawing.Size(75, 23);
			this.buttonStartStopWatcher.TabIndex = 32;
			this.buttonStartStopWatcher.Text = "Start";
			this.buttonStartStopWatcher.UseVisualStyleBackColor = true;
			this.buttonStartStopWatcher.Click += new System.EventHandler(this.buttonStartStopMonitor_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(58, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(114, 13);
			this.label1.TabIndex = 26;
			this.label1.Text = "Local folder to monitor:";
			// 
			// textBoxLocalPath
			// 
			this.textBoxLocalPath.Location = new System.Drawing.Point(174, 4);
			this.textBoxLocalPath.Name = "textBoxLocalPath";
			this.textBoxLocalPath.Size = new System.Drawing.Size(332, 20);
			this.textBoxLocalPath.TabIndex = 25;
			this.textBoxLocalPath.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxLocalPath_Validating);
			// 
			// buttonBrowseLocalPath
			// 
			this.buttonBrowseLocalPath.Location = new System.Drawing.Point(525, 2);
			this.buttonBrowseLocalPath.Name = "buttonBrowseLocalPath";
			this.buttonBrowseLocalPath.Size = new System.Drawing.Size(50, 23);
			this.buttonBrowseLocalPath.TabIndex = 27;
			this.buttonBrowseLocalPath.Text = "Browse";
			this.buttonBrowseLocalPath.UseVisualStyleBackColor = true;
			this.buttonBrowseLocalPath.Click += new System.EventHandler(this.buttonBrowseLocalPath_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(5, 42);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(166, 13);
			this.label4.TabIndex = 29;
			this.label4.Text = "Upload to myHEALTHware Drive:";
			// 
			// textBoxMHWFolder
			// 
			this.textBoxMHWFolder.Location = new System.Drawing.Point(174, 38);
			this.textBoxMHWFolder.Name = "textBoxMHWFolder";
			this.textBoxMHWFolder.ReadOnly = true;
			this.textBoxMHWFolder.Size = new System.Drawing.Size(332, 20);
			this.textBoxMHWFolder.TabIndex = 28;
			// 
			// deleteFileAfterUploadCheckBox
			// 
			this.deleteFileAfterUploadCheckBox.AutoSize = true;
			this.deleteFileAfterUploadCheckBox.Location = new System.Drawing.Point(174, 75);
			this.deleteFileAfterUploadCheckBox.Name = "deleteFileAfterUploadCheckBox";
			this.deleteFileAfterUploadCheckBox.Size = new System.Drawing.Size(201, 17);
			this.deleteFileAfterUploadCheckBox.TabIndex = 31;
			this.deleteFileAfterUploadCheckBox.Text = "Delete file after succesfully uploading";
			this.deleteFileAfterUploadCheckBox.UseVisualStyleBackColor = true;
			this.deleteFileAfterUploadCheckBox.Click += new System.EventHandler(this.DeleteFileAfterUploadCheckBox_CheckedChanged);
			// 
			// buttonBrowseUploadPath
			// 
			this.buttonBrowseUploadPath.Location = new System.Drawing.Point(525, 37);
			this.buttonBrowseUploadPath.Name = "buttonBrowseUploadPath";
			this.buttonBrowseUploadPath.Size = new System.Drawing.Size(51, 23);
			this.buttonBrowseUploadPath.TabIndex = 30;
			this.buttonBrowseUploadPath.Text = "Browse";
			this.buttonBrowseUploadPath.UseVisualStyleBackColor = true;
			this.buttonBrowseUploadPath.Click += new System.EventHandler(this.buttonBrowseUploadPath_Click);
			// 
			// errorProviderLocalFolder
			// 
			this.errorProviderLocalFolder.ContainerControl = this;
			// 
			// pictureStartedStopped
			// 
			this.pictureStartedStopped.Image = global::myHEALTHwareDesktop.Properties.Resources.stopped;
			this.pictureStartedStopped.Location = new System.Drawing.Point(433, 104);
			this.pictureStartedStopped.Name = "pictureStartedStopped";
			this.pictureStartedStopped.Size = new System.Drawing.Size(32, 32);
			this.pictureStartedStopped.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureStartedStopped.TabIndex = 34;
			this.pictureStartedStopped.TabStop = false;
			// 
			// errorProviderDriveFolder
			// 
			this.errorProviderDriveFolder.ContainerControl = this;
			// 
			// ControlFolderToDrive
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pictureStartedStopped);
			this.Controls.Add(this.label_MonitorStatus);
			this.Controls.Add(this.buttonStartStopWatcher);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxLocalPath);
			this.Controls.Add(this.buttonBrowseLocalPath);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBoxMHWFolder);
			this.Controls.Add(this.deleteFileAfterUploadCheckBox);
			this.Controls.Add(this.buttonBrowseUploadPath);
			this.Name = "ControlFolderToDrive";
			this.Size = new System.Drawing.Size(602, 147);
			((System.ComponentModel.ISupportInitialize)(this.errorProviderLocalFolder)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureStartedStopped)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProviderDriveFolder)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.Label label_MonitorStatus;
		public System.Windows.Forms.Button buttonStartStopWatcher;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.TextBox textBoxLocalPath;
		public System.Windows.Forms.Button buttonBrowseLocalPath;
		private System.Windows.Forms.Label label4;
		public System.Windows.Forms.TextBox textBoxMHWFolder;
		public System.Windows.Forms.CheckBox deleteFileAfterUploadCheckBox;
		public System.Windows.Forms.Button buttonBrowseUploadPath;
		public System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.PictureBox pictureStartedStopped;
		private System.Windows.Forms.ErrorProvider errorProviderDriveFolder;
		private System.Windows.Forms.ErrorProvider errorProviderLocalFolder;
	}
}
