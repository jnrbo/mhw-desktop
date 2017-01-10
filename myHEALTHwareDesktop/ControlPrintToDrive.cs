using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SOAPware.PortalSdk;
using SOAPware.PortalApi.Models;
using SOAPware.PortalApi.Model.Conversations;
using SOAPware.PortalApi.Model.Drive;
using MHWVirtualPrinter;

namespace myHEALTHwareDesktop
{
	public partial class ControlPrintToDrive : UserControl
	{
		private SettingsForm parentForm;
		public MhwSdk sdk;
		public string selectedMHWAccountId;
		private VirtualPrinter virtualPrinter = new VirtualPrinter();
		private bool isPrintToDriveInstalled;
		private DrivePicker drivePicker;
		private string drivePickerResult;
		private FileSystemWatcher localPathWatcher = null;
		private bool isDefaultUploadPathSet = false;
		private bool isDrivePickerSuccess = false;
		private bool isWatcherRunning = false;
		private bool isLoggedIn = false;
		private string drivePickerFileName;

		public ControlPrintToDrive()
		{
			InitializeComponent();
		}

		public void LoadSettings(SettingsForm parent, MhwSdk sdk, string selectedMHWAccountId)
		{
			parentForm = parent;
			this.sdk = sdk;
			this.selectedMHWAccountId = selectedMHWAccountId;

			if (LoadPrinterInstalled())
			{
				SetPromptOrDefaultState();
				InitRadioButtons();
			}

			isLoggedIn = true;
		}

		private bool LoadPrinterInstalled()
		{
			isPrintToDriveInstalled = virtualPrinter.IsPrinterAlreadyInstalled(MHWPrinter.PrintToDrive.PrinterName);

			if (isPrintToDriveInstalled)
			{
				this.buttonPrintToDriveInstall.Text = "Uninstall Printer";
			}
			else
			{
				this.buttonPrintToDriveInstall.Text = "Install Printer";
				this.label_MonitorStatus.Text = "Print to Drive printer is not installed.";
				// Clear any errors.
				this.errorProviderDriveFolder.SetError(textBoxPrintToDriveFolder, "");
			}

			this.radioButtonPrompt.Enabled = isPrintToDriveInstalled;
			this.radioButtonUseDefault.Enabled = isPrintToDriveInstalled;
			this.textBoxPrintToDriveFolder.Enabled = isPrintToDriveInstalled;
			this.buttonBrowsePrintToDrivePath.Enabled = isPrintToDriveInstalled;

			return isPrintToDriveInstalled;
		}

		private void InitRadioButtons()
		{
			// Set the initial state of the radio buttons.
			if (isDefaultUploadPathSet)
			{
				this.radioButtonPrompt.Checked = Properties.Settings.Default.PrintToDrivePrompt;
				this.radioButtonUseDefault.Checked = !Properties.Settings.Default.PrintToDrivePrompt;
			}
			else
			{
				// Default location not set so force to prompt.
				this.radioButtonPrompt.Checked = true;
				this.radioButtonUseDefault.Checked = false;
			}
		}

		private void SetPromptOrDefaultState()
		{
			if (Properties.Settings.Default.PrintToDrivePrompt)
			{
				// Disable the Drive path and browse button.
				this.textBoxPrintToDriveFolder.Enabled = false;
				this.buttonBrowsePrintToDrivePath.Enabled = false;

				// Clear any errors.
				this.errorProviderDriveFolder.SetError(textBoxPrintToDriveFolder, "");

				StartMonitoring();
			}
			else
			{
				// Enable the Drive path and browse button.
				this.textBoxPrintToDriveFolder.Enabled = true;
				this.buttonBrowsePrintToDrivePath.Enabled = true;

				// Make sure current value is valid.
				LoadDefaultDriveLocation();

				if (isDefaultUploadPathSet)
				{
					StartMonitoring();
				}
				else
				{
					StopMonitoring();
				}
			}
		}

		private string LoadDefaultDriveLocation()
		{
			if (String.IsNullOrWhiteSpace(Properties.Settings.Default.PrintToDriveDefaultDestinationId))
			{
				this.errorProviderDriveFolder.SetError(this.textBoxPrintToDriveFolder,
							"Please select a Drive folder to upload files to.");

				isDefaultUploadPathSet = false;
				return null;
			}

			ApiDriveItem item;

			// Using SDK, retrieve Drive Item's full path
			try
			{
				item = this.sdk.DriveItems.GetDriveItem(this.selectedMHWAccountId, Properties.Settings.Default.PrintToDriveDefaultDestinationId);
			}
			catch (Exception ex)
			{
				isDefaultUploadPathSet = false;
				Properties.Settings.Default.PrintToDriveDefaultDestinationId = "";

				string message = String.Format("Error setting Drive path: {0}", ex.Message.Substring(0, 60));
				this.errorProviderDriveFolder.SetError(this.textBoxPrintToDriveFolder, message);

				return null;
			}

			string path = string.Format("{0}/{1}", item.Path, item.Name);
			SetUploadPathText(path);
			isDefaultUploadPathSet = true;
			return path;
		}

		private void radioButtons_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton radioButton = sender as RadioButton;

			if (radioButton == null || radioButton.Checked == false)
				return;

			string message = "";

			if (radioButton == this.radioButtonUseDefault)
			{
				Properties.Settings.Default.PrintToDrivePrompt = false;
				//message = "Print to Drive will upload to default Drive location";
			}
			else
			{
				Properties.Settings.Default.PrintToDrivePrompt = true;
				//message = "Print to Drive will prompt you for Drive location with each print";
			}

			parentForm.SaveSettings(message);

			SetPromptOrDefaultState();
		}

		private void buttonPrintToDriveInstall_Click(object sender, EventArgs e)
		{
			this.buttonPrintToDriveInstall.Enabled = false;

			if (isPrintToDriveInstalled)
			{
				Uninstall();
			}
			else
			{
				Install();
			}

			this.buttonPrintToDriveInstall.Enabled = true;
		}

		public void Uninstall()
		{
			// Args to perform uninstall
			string setupArgs = "-a -u -d";

			StopMonitoring();

			// Reset previous settings.
			this.isDefaultUploadPathSet = false;
			Properties.Settings.Default.PrintToDriveDefaultDestinationId = "";
			SetUploadPathText("");
			Properties.Settings.Default.PrintToDrivePrompt = true;
			parentForm.SaveSettings();

			// Launch setup process with args to uninstall.
			var process = new Process
			{
				StartInfo = new ProcessStartInfo { FileName = "Setup.exe", Arguments = setupArgs }
			};

			process.Start();
			process.WaitForExit();

			// Verify the printer is now uninstalled and set state.
			if (LoadPrinterInstalled())
			{
				SetPromptOrDefaultState();
			}
		}

		public void Install()
		{
			string setupArgs = "-a -d";

			// Launch setup process with args to install.
			var process = new Process
			{
				StartInfo = new ProcessStartInfo { FileName = "Setup.exe", Arguments = setupArgs }
			};

			process.Start();
			process.WaitForExit();

			// Reset previous settings.
			this.isDefaultUploadPathSet = false;
			Properties.Settings.Default.PrintToDriveDefaultDestinationId = "";
			SetUploadPathText("");
			Properties.Settings.Default.PrintToDrivePrompt = true;
			parentForm.SaveSettings();

			// Verify the printer is now installed and set state.
			if (LoadPrinterInstalled())
			{
				InitRadioButtons();
				SetPromptOrDefaultState();
			}
		}

		delegate void SetTextCallback(string text);

		// This method handles calling from cross thread or directly.
		private void SetUploadPathText(string text)
		{
			if (this.textBoxPrintToDriveFolder.InvokeRequired)
			{
				SetTextCallback d = new SetTextCallback(SetUploadPathText);
				this.Invoke(d, new object[] { text });
			}
			else
			{
				this.textBoxPrintToDriveFolder.Text = text;

				// Valid. Clear any previous error.
				this.errorProviderDriveFolder.SetError(this.textBoxPrintToDriveFolder, "");
			}
		}

		private void buttonBrowsePrintToDrivePath_Click(object sender, EventArgs e)
		{
			LaunchDrivePicker(false);

			if (isDrivePickerSuccess)
			{
				Properties.Settings.Default.PrintToDriveDefaultDestinationId = this.drivePickerResult;
			}
		}

		private void LaunchDrivePicker(bool isShowFileName, string defaultFileName=null)
		{
			drivePicker = new DrivePicker(SettingsForm.appId, SettingsForm.appSecret);
			drivePicker.EnableFileName(isShowFileName, defaultFileName);
			drivePicker.InitBrowser(parentForm.connectionId, parentForm.accessToken, this.selectedMHWAccountId);

			// Register a method to recieve click event callback.
			drivePicker.Click += new System.EventHandler(this.drivePicker_OnClick);

			drivePicker.ShowDialog(this);

			if (isDrivePickerSuccess)
			{
				// Valid. Clear any previous error.
				this.errorProviderDriveFolder.SetError(this.textBoxPrintToDriveFolder, "");

				if (isShowFileName)
				{
					drivePickerFileName = drivePicker.GetFileName();
				}

				// Display folder in dialog box.
				string path = SetUploadPath(drivePickerResult);

				// Save Drive item ID.
				this.parentForm.SaveSettings();

				StartMonitoring();
			}
		}

		private string SetUploadPath(string folderId)
		{
			ApiDriveItem item;

			// Using SDK, retrieve Drive Item's full path
			try
			{
				item = this.sdk.DriveItems.GetDriveItem(this.selectedMHWAccountId, folderId);
			}
			catch (Exception ex)
			{
				parentForm.ShowBalloonError("Could not retrieve upload Drive folder: {0}", ex.Message);
				return null;
			}

			string path = string.Format("{0}/{1}", item.Path, item.Name);
			SetUploadPathText(path);
			isDefaultUploadPathSet = true;
			return path;
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
				drivePickerResult = args.Message.data.driveItemId;
				isDrivePickerSuccess = true;
			}
			else if (args.Message.eventType == "mhw.drive.select.cancelled")
			{
				isDrivePickerSuccess = false;
				parentForm.ShowBalloonWarning("Print to Drive upload was cancelled.");
			}
			else
			{
				isDrivePickerSuccess = false;
				parentForm.ShowBalloonError("Print to Drive Error: {0}", args.Message.data.message);
			}

			// Close the Drive picker form in a cross thread acceptable way.
			this.BeginInvoke((MethodInvoker)delegate { drivePicker.Dispose(); });
		}

		private void StartMonitoring()
		{
			if (this.isWatcherRunning)
			{
				// Already running.
				return;
			}

			if (!this.isPrintToDriveInstalled)
			{
				// Printer not installed.
				return;
			}

			if ((!Properties.Settings.Default.PrintToDrivePrompt) && (!this.isDefaultUploadPathSet))
			{
				// Configured to upload to a default Drive path but it is not set.
				return;
			}

			// Create the folder watcher.
			localPathWatcher = new FileSystemWatcher();

			try
			{
				localPathWatcher.Path = Path.Combine(Path.GetTempPath(), MHWPrinter.PrintToDrive.MonitorName);
			}
			catch (ArgumentException ex)
			{
				localPathWatcher.Dispose();
				localPathWatcher = null;
				isWatcherRunning = false;
				parentForm.ShowBalloonError("Start Print to Drive monitor failed: {0}", ex.Message);
				return;
			}

			localPathWatcher.EnableRaisingEvents = true;
			localPathWatcher.NotifyFilter = ((System.IO.NotifyFilters)(((System.IO.NotifyFilters.FileName | 
									System.IO.NotifyFilters.LastWrite) | System.IO.NotifyFilters.LastAccess)));
			localPathWatcher.SynchronizingObject = this;
			localPathWatcher.Created += new System.IO.FileSystemEventHandler(this.localPathWatcher_Created);
			isWatcherRunning = true;
			this.pictureStartedStopped.Image = Properties.Resources.started;
			this.label_MonitorStatus.Text = "Print to Drive monitor is running.";
		}

		private void StopMonitoring()
		{
			if (localPathWatcher == null)
				return;

			try
			{
				localPathWatcher.Dispose();
				localPathWatcher = null;
			}
			catch (ArgumentException ex)
			{
				parentForm.ShowBalloonError("Stop Print to Drive monitor error: {0}", ex.Message);
			}

			isWatcherRunning = false;
			this.pictureStartedStopped.Image = Properties.Resources.stopped;
			this.label_MonitorStatus.Text = "Print to Drive monitor is stopped.";
		}

		private void localPathWatcher_Created(object sender, FileSystemEventArgs e)
		{
			ProcessNewPrintFile(e.FullPath, e.Name);
		}

		// The Watcher calls this method when a new file shows up in the watched folder.
		private void ProcessNewPrintFile(string fullPath, string name)
		{
			if (isLoggedIn == false)
			{
				StopMonitoring();
				parentForm.ShowBalloonError("Please log in and try again. Print job deleted.");
				File.Delete(fullPath);
				return;
			}

			string driveItemId = Properties.Settings.Default.PrintToDriveDefaultDestinationId;
			string fileName = name;

			if (Properties.Settings.Default.PrintToDrivePrompt)
			{
				LaunchDrivePicker(true, name);

				if (isDrivePickerSuccess)
				{
					driveItemId = drivePickerResult;
					fileName = drivePickerFileName;
				}
				else
				{
					File.Delete(fullPath);
					return;
				}
			}

			if (this.parentForm.UploadFile(fullPath, fileName, driveItemId, true) == null)
			{
				parentForm.ShowBalloonError("Print to Drive upload failed: {0}", name);
			}
		}

		public void LoggedOut()
		{
			isLoggedIn = false;
		}
	}
}
