using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SOAPware.PortalSdk;
using SOAPware.PortalApi.Models;
using SOAPware.PortalApi.Model.Drive;

namespace myHEALTHwareDesktop
{
	public partial class ControlFolderToDrive : UserControl
	{
		private MhwDesktopForm parentForm;
		public MhwSdk sdk;
		public string selectedMHWAccountId;
		private DrivePicker drivePicker;
		private FileSystemWatcher localPathWatcher = null;
		private bool isWatcherRunning = false;
		private bool isLocalPathSet = false;
		private bool isUploadPathSet = false;
		private bool isDrivePickerSuccess = false;
		private string uploadDriveItemId = null;
		private bool isLoggedIn = false;

		public ControlFolderToDrive()
		{
			InitializeComponent();
		}

		public void LoadSettings(MhwDesktopForm parent, MhwSdk sdk, string selectedMHWAccountId)
		{
			parentForm = parent;
			this.sdk = sdk;
			this.selectedMHWAccountId = selectedMHWAccountId;
			isLoggedIn = true;
			LoadSettingDeleteFileAfterUpload();
			SetLocalPath(Properties.Settings.Default.FolderToDriveLocalPath);
			LoadDriveLocation(Properties.Settings.Default.FolderToDriveDestinationId);

			// If all set, start monitoring.
			if (Properties.Settings.Default.FolderToDriveRunning && isLocalPathSet && isUploadPathSet)
			{
				StartMonitoring();
			}
		}

		private void LoadSettingDeleteFileAfterUpload()
		{
			this.deleteFileAfterUploadCheckBox.Checked = Properties.Settings.Default.FolderToDriveDeleteFileAfterUpload;

			if (Properties.Settings.Default.FolderToDriveDeleteFileAfterUpload)
			{
				this.deleteFileAfterUploadCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			}
			else
			{
				this.deleteFileAfterUploadCheckBox.CheckState = System.Windows.Forms.CheckState.Unchecked;
			}
		}

		private void DeleteFileAfterUploadCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			string message;
			if (this.deleteFileAfterUploadCheckBox.Checked)
			{
				message = "Files will be deleted after upload.";
			}
			else
			{
				message = "Files will not be deleted after upload.  You may want to periodically clean out your local upload folder.";
			}

			// Save the new value.
			Properties.Settings.Default.FolderToDriveDeleteFileAfterUpload = this.deleteFileAfterUploadCheckBox.Checked;
			parentForm.SaveSettings(message);
		}

		private void buttonBrowseLocalPath_Click(object sender, EventArgs e)
		{
			// Set initial folder to start browsing from.
			this.folderBrowserDialog.SelectedPath = this.textBoxLocalPath.Text;

			DialogResult result = this.folderBrowserDialog.ShowDialog();

			if (result == DialogResult.OK) // Test result.
			{
				SetLocalPath(this.folderBrowserDialog.SelectedPath);
			}
		}

		// This method called when local directory is set manually via typing into text box.
		private void textBoxLocalPath_Validating(object sender, CancelEventArgs e)
		{
			SetLocalPath(this.textBoxLocalPath.Text);
		}

		private void SetLocalPath(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				this.errorProviderLocalFolder.SetError(this.textBoxLocalPath, 
							"Please select a local folder to monitor for files to upload.");
				isLocalPathSet = false;
				SetButtonState();
			}
			else if (Directory.Exists(path) == false)
			{
				this.textBoxLocalPath.Text = path;
				this.errorProviderLocalFolder.SetError(this.textBoxLocalPath,
							"The selected folder does not exist.  Please select a new one.");
				isLocalPathSet = false;
				SetButtonState();
			}
			else
			{
				// Valid. Clear any previous error.
				this.errorProviderLocalFolder.SetError(this.textBoxLocalPath, "");

				this.textBoxLocalPath.Text = path;
				isLocalPathSet = true;
				SetButtonState();

				// Has the value actually changed since it was last valid?
				if (path != Properties.Settings.Default.FolderToDriveLocalPath)
				{
					Properties.Settings.Default.FolderToDriveLocalPath = path;
					parentForm.SaveSettings();
				}
			}
		}

		private string LoadDriveLocation(string folderId)
		{
			if (String.IsNullOrWhiteSpace(folderId))
			{
				this.errorProviderDriveFolder.SetError(this.textBoxMHWFolder,
					"Please select a Drive folder to upload files to.");
				isUploadPathSet = false;
				return null;
			}

			ApiDriveItem item;

			// Using SDK, retrieve Drive Item's full path
			try
			{
				item = this.sdk.DriveItems.GetDriveItem(this.selectedMHWAccountId, folderId);
			}
			catch (Exception ex)
			{
				isUploadPathSet = false;

				string message = String.Format("Error setting Drive path: {0}", ex.Message.Substring(0, 60));
				this.errorProviderDriveFolder.SetError(this.textBoxMHWFolder, message);
				SetButtonState();
				return null;
			}

			// Valid. Clear any previous error.
			this.errorProviderDriveFolder.SetError(this.textBoxMHWFolder, "");

			string path = string.Format("{0}/{1}", item.Path, item.Name);
			SetUploadPathText(path);
			this.uploadDriveItemId = folderId;
			isUploadPathSet = true;
			return path;
		}

		delegate void SetTextCallback(string text);

		// This method handles calling from cross thread or directly.
		private void SetUploadPathText(string text)
		{
			if (this.textBoxMHWFolder.InvokeRequired)
			{
				SetTextCallback d = new SetTextCallback(SetUploadPathText);
				this.Invoke(d, new object[] { text });
			}
			else
			{
				this.textBoxMHWFolder.Text = text;
			}
		}

		private void buttonBrowseUploadPath_Click(object sender, EventArgs e)
		{
			BrowseUploadPath();
		}

		private void BrowseUploadPath()
		{
			drivePicker = new DrivePicker(MhwDesktopForm.APP_ID, MhwDesktopForm.APP_SECRET);
			drivePicker.EnableFileName(false);
			drivePicker.InitBrowser(parentForm.ConnectionId, parentForm.AccessToken, this.selectedMHWAccountId);
			
			// Register a method to recieve click event callback.
			drivePicker.Click += new System.EventHandler(this.drivePicker_OnClick);

			drivePicker.ShowDialog(this);

			if (isDrivePickerSuccess)
			{
				// Valid. Clear any previous error.
				this.errorProviderDriveFolder.SetError(this.textBoxMHWFolder, "");

				// Display folder in dialog box.
				string path = LoadDriveLocation(this.uploadDriveItemId);

				// Save Drive item ID.
				Properties.Settings.Default.FolderToDriveDestinationId = this.uploadDriveItemId;
				parentForm.SaveSettings();
			}

			SetButtonState();
		}

		private void drivePicker_OnClick(object sender, EventArgs e)
		{
			GetDrivePickerResults((PostMessageListenerEventArgs)e);
		}

		// This method is called when DrivePicker fires the Click event so that we can
		// retrieve the results from it.
		private void GetDrivePickerResults(PostMessageListenerEventArgs args)
		{
			if (args.Message.eventType == "mhw.drive.select.success")
			{
				this.uploadDriveItemId = args.Message.data.driveItemId;
				isDrivePickerSuccess = true;
			}
			else if (args.Message.eventType == "mhw.drive.select.cancelled")
			{
				// Cancelled.
				isDrivePickerSuccess = false;
			}
			else
			{
				isDrivePickerSuccess = false;
				parentForm.ShowBalloonError("Drive picker error: {0}", args.Message.data.message);
			}

			// Close the Drive picker form in a cross thread acceptable way.
			this.BeginInvoke((MethodInvoker)delegate { drivePicker.Dispose(); });
		}

		private void SetButtonState()
		{
			if (isWatcherRunning)
			{
				this.pictureStartedStopped.Image = Properties.Resources.started;
				this.label_MonitorStatus.Text = "Folder to Drive monitor is running.";
				this.buttonStartStopWatcher.Text = "Stop";
				this.textBoxLocalPath.Enabled = false;
				this.textBoxMHWFolder.Enabled = false;
				this.buttonBrowseLocalPath.Enabled = false;
				this.buttonBrowseUploadPath.Enabled = false;
				this.buttonStartStopWatcher.Enabled = true;
			}
			else
			{
				this.pictureStartedStopped.Image = Properties.Resources.stopped;
				this.label_MonitorStatus.Text = "Folder monitor is stopped.";
				this.buttonStartStopWatcher.Text = "Start";
				this.textBoxLocalPath.Enabled = true;
				this.textBoxMHWFolder.Enabled = true;
				this.buttonBrowseLocalPath.Enabled = true;
				this.buttonBrowseUploadPath.Enabled = true;
				this.buttonStartStopWatcher.Enabled = (isLocalPathSet && isUploadPathSet);
			}
		}

		private void buttonStartStopMonitor_Click(object sender, EventArgs e)
		{
			StartOrStopMonitor();
		}

		private void StartOrStopMonitor()
		{
			if (isWatcherRunning)
			{
				StopMonitoring();
				Properties.Settings.Default.FolderToDriveRunning = false;
				parentForm.SaveSettings("Stopped Folder to Drive monitor");
			}
			else
			{
				StartMonitoring();
				Properties.Settings.Default.FolderToDriveRunning = true;
				parentForm.SaveSettings("Started Folder to Drive monitor");
			}

			SetButtonState();
		}

		private void StartMonitoring()
		{
			if (!isLocalPathSet || !isUploadPathSet)
			{
				MessageBox.Show("Please set local and Drive folders first.", "Error");
				return;
			}

			// Create the folder watcher.
			localPathWatcher = new FileSystemWatcher();

			try
			{
				localPathWatcher.Path = this.textBoxLocalPath.Text;
			}
			catch (ArgumentException ex)
			{
				localPathWatcher.Dispose();
				localPathWatcher = null;
				isWatcherRunning = false;
				parentForm.ShowBalloonError("Start Folder to Drive monitor failed: {0}", ex.Message);
				return;
			}

			localPathWatcher.EnableRaisingEvents = true;
			localPathWatcher.NotifyFilter = ((System.IO.NotifyFilters)(((System.IO.NotifyFilters.FileName |
									System.IO.NotifyFilters.LastWrite) | System.IO.NotifyFilters.LastAccess)));
			localPathWatcher.SynchronizingObject = this;
			localPathWatcher.Created += new System.IO.FileSystemEventHandler(this.localPathWatcher_Created);

			isWatcherRunning = true;
		}

		private void StopMonitoring()
		{
			try
			{
				localPathWatcher.Dispose();
				localPathWatcher = null;
			}
			catch (ArgumentException ex)
			{
				parentForm.ShowBalloonError("Stop Folder to Drive monitor error: {0}", ex.Message);
			}

			isWatcherRunning = false;
		}

		private void localPathWatcher_Created(object sender, FileSystemEventArgs e)
		{
			ProcessNewLocalFile(e.FullPath, e.Name);
		}

		// The Watcher calls this method when a new file shows up in the watched folder.
		private void ProcessNewLocalFile(string fullPath, string name)
		{
			if (isLoggedIn == false)
			{
				StopMonitoring();
				parentForm.ShowBalloonError("Please log in and try again. Folder to Drive file deleted.");
				File.Delete(fullPath);
				return;
			}

			if (this.parentForm.UploadFile(fullPath, name, this.uploadDriveItemId, Properties.Settings.Default.FolderToDriveDeleteFileAfterUpload) == null)
			{
				parentForm.ShowBalloonError("Folder to Drive upload failed: {0}", name);
			}
		}

		public void LogOut()
		{
			isLoggedIn = false;
		}
	}
}
