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
using SOAPware.PortalApi.Model.Faxes;
using MHWVirtualPrinter;

namespace myHEALTHwareDesktop
{
	public partial class ControlPrintToFax : UserControl
	{
		private SettingsForm parentForm;
		public MhwSdk sdk;
		public string selectedMHWAccountId;
		private VirtualPrinter virtualPrinter = new VirtualPrinter();
		private bool isMHWFaxInstalled;
		private bool isPrinterInstalled;
		private FileSystemWatcher localPathWatcher = null;
		private bool isWatcherRunning;
		private SendFax sendFax;
		public bool isSendFaxSuccess;
		private bool isLoggedIn = false;

		public ControlPrintToFax()
		{
			InitializeComponent();
		}

		public void LoadSettings(SettingsForm parent, MhwSdk sdk, string selectedMHWAccountId)
		{
			parentForm = parent;
			this.sdk = sdk;
			this.selectedMHWAccountId = selectedMHWAccountId;
			LoadPrintToFaxState();
			isLoggedIn = true;
		}

		private bool LoadPrintToFaxState()
		{
			const string faxAppId = "FAC5FAC5-45E9-434A-AF34-C9E070729299";

			// Is MHW Fax installed on MHW account?
			try
			{
				ApiConnectedApplication app = sdk.Application.GetConnection(selectedMHWAccountId, faxAppId);
				isMHWFaxInstalled = true;
				this.buttonPrintToFaxInstall.Enabled = true;
			}
			catch (Exception)
			{
				// Check network connection status.
				parentForm.CheckNetworkStatus();

				isMHWFaxInstalled = false;
				this.labelStatus.Text = "myHEALTHware Fax is not installed on selected account.";
				this.labelFaxLearnMoreLink.Show();
				this.buttonPrintToFaxInstall.Enabled = false;
				this.radioButtonPrompt.Enabled = false;
				this.radioButtonSaveDraft.Enabled = false;
				StopMonitoring();
				return false;
			}

			this.labelFaxLearnMoreLink.Hide();
			isPrinterInstalled = virtualPrinter.IsPrinterAlreadyInstalled(MHWPrinter.PrintToFax.PrinterName);

			if (isPrinterInstalled)
			{
				StartMonitoring();
				this.buttonPrintToFaxInstall.Text = "Uninstall Printer";
				this.labelStatus.Text = "Print to Fax is ready.";
			}
			else
			{
				StopMonitoring();
				this.buttonPrintToFaxInstall.Text = "Install Printer";
				this.labelStatus.Text = "Print to Fax printer is not installed.";
			}

			// Set the initial state of the radio buttons.
			this.radioButtonPrompt.Checked = Properties.Settings.Default.PrintToFaxPrompt;
			this.radioButtonSaveDraft.Checked = !Properties.Settings.Default.PrintToFaxPrompt;

			this.radioButtonPrompt.Enabled = isPrinterInstalled;
			this.radioButtonSaveDraft.Enabled = isPrinterInstalled;

			return isPrinterInstalled;
		}

		private void radioButtons_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton radioButton = sender as RadioButton;

			if (radioButton == null || radioButton.Checked == false)
				return;

			string message = "";

			if (radioButton == this.radioButtonSaveDraft)
			{
				Properties.Settings.Default.PrintToFaxPrompt = false;
				//message = "Print to Fax will automatically save as draft fax";
			}
			else
			{
				Properties.Settings.Default.PrintToFaxPrompt = true;
				//message = "Print to Fax will prompt you to send fax with each print";
			}

			parentForm.SaveSettings(message);
		}

		private void buttonPrintToFaxInstall_Click(object sender, EventArgs e)
		{
			this.buttonPrintToFaxInstall.Enabled = false;

			if (isPrinterInstalled)
			{
				Uninstall();
			}
			else
			{
				Install();
			}

			this.buttonPrintToFaxInstall.Enabled = true;
		}

		public void Uninstall()
		{
			// Args for uninstall
			string setupArgs = "-a -u -f";

			// Launch setup process.
			var process = new Process
			{
				StartInfo = new ProcessStartInfo { FileName = "Setup.exe", Arguments = setupArgs }
			};

			process.Start();
			process.WaitForExit();

			// See if the printer is now uninstalled.
			LoadPrintToFaxState();
		}

		public void Install()
		{
			string setupArgs = "-a -f";

			// Launch setup process.
			var process = new Process
			{
				StartInfo = new ProcessStartInfo { FileName = "Setup.exe", Arguments = setupArgs }
			};

			process.Start();
			process.WaitForExit();

			// See if the printer is now installed.
			LoadPrintToFaxState();
		}

		private void StartMonitoring()
		{
			if (this.isWatcherRunning)
			{
				// Already running.
				return;
			}

			if (!this.isPrinterInstalled || !this.isMHWFaxInstalled)
			{
				// Printer or app not installed.
				return;
			}

			// Create the folder watcher.
			localPathWatcher = new FileSystemWatcher();

			try
			{
				localPathWatcher.Path = Path.Combine(Path.GetTempPath(), MHWPrinter.PrintToFax.MonitorName);
			}
			catch (ArgumentException ex)
			{
				localPathWatcher.Dispose();
				localPathWatcher = null;
				isWatcherRunning = false;
				parentForm.ShowBalloonError("Start Print to Fax monitor failed: {0}", ex.Message);
				return;
			}

			localPathWatcher.EnableRaisingEvents = true;
			localPathWatcher.NotifyFilter = ((System.IO.NotifyFilters)(((System.IO.NotifyFilters.FileName |
									System.IO.NotifyFilters.LastWrite) | System.IO.NotifyFilters.LastAccess)));
			localPathWatcher.SynchronizingObject = this;
			localPathWatcher.Created += new System.IO.FileSystemEventHandler(this.localPathWatcher_Created);
			isWatcherRunning = true;
			this.pictureStartedStopped.Image = Properties.Resources.started;
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
				parentForm.ShowBalloonError("Stop Print to Fax monitor error: {0}", ex.Message);
			}

			isWatcherRunning = false;
			this.pictureStartedStopped.Image = Properties.Resources.stopped;
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

			// Upload the printed PDF file to Drive.
			string fileId = parentForm.UploadFile(fullPath, name, null, true);

			if (fileId == null)
			{
				parentForm.ShowBalloonError("Print to Fax failed: Upload {0} failed", fullPath);
				return;
			}

			if (Properties.Settings.Default.PrintToFaxPrompt)
			{
				LaunchSendFax(fileId);
			}
			else
			{
				// Create draft fax.
				ApiFax draftFax = new ApiFax();
				draftFax.AccountId = selectedMHWAccountId;
				draftFax.FileId = fileId;
				draftFax.To = new List<ApiFaxRecipient> {};

				try
				{
					sdk.Fax.Create(draftFax, true, false, null, null);
				}
				catch (Exception ex)
				{
					parentForm.ShowBalloonError("Print to Fax create draft failed: {0}", ex.Message);
				}
			}
		}

		private void LaunchSendFax(string fileId)
		{
			sendFax = new SendFax(SettingsForm.appId, SettingsForm.appSecret);

			sendFax.InitBrowser(parentForm.connectionId, parentForm.accessToken, this.selectedMHWAccountId, fileId);

			// Register a method to recieve click event callback.
			sendFax.Click += new System.EventHandler(this.SendFax_OnClick);

			sendFax.ShowDialog(this);
		}

		private void SendFax_OnClick(object sender, EventArgs e)
		{
			GetSendFaxResults((PostMessageListenerEventArgs)e);
		}

		// This method is called when SendFax fires the Click event so that we can
		// retrieve the results from it.
		private void GetSendFaxResults(PostMessageListenerEventArgs args)
		{
			if (args.Message.eventType == "mhw.fax.send.success")
			{
				isSendFaxSuccess = true;
			}
			else if (args.Message.eventType == "mhw.fax.send.cancelled")
			{
				// Cancelled.
				isSendFaxSuccess = false;
				parentForm.ShowBalloonWarning("Print to Fax was cancelled.");
			}
			else
			{
				isSendFaxSuccess = false;
				parentForm.ShowBalloonInfo("Send Fax Error: {0}", args.Message.data.message);
			}

			// Close the Drive picker form in a cross thread acceptable way.
			this.BeginInvoke((MethodInvoker)delegate { sendFax.Dispose(); });
		}

		public void LoggedOut()
		{
			isLoggedIn = false;
		}

		private void labelFaxLearnMoreLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.myhealthware.com/#/marketplace");
		}

		private void pictureBoxMarketplaceFax_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.myhealthware.com/#/fax/");
		}
	}
}
