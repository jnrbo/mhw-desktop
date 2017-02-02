using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using myHEALTHwareDesktop.Properties;
using MHWVirtualPrinter;
using SOAPware.PortalApi.Model.Faxes;
using SOAPware.PortalSdk;

namespace myHEALTHwareDesktop
{
	public partial class ControlPrintToFax : UserControl
	{
		private const string FAX_APP_ID = "FAC5FAC5-45E9-434A-AF34-C9E070729299";
		private readonly VirtualPrinter virtualPrinter = new VirtualPrinter();

		private MhwDesktopForm parentForm;
		private MhwSdk sdk;
		private string selectedMhwAccountId;
		private bool isPrinterInstalled;
		private FileSystemWatcher localPathWatcher;
		private bool isWatcherRunning;
		private SendFax sendFax;
		private bool isSendFaxSuccess;
		private bool isLoggedIn;

		public bool IsMhwFaxInstalled { get; set; }

		public ControlPrintToFax()
		{
			InitializeComponent();
		}

		public void LoadSettings( MhwDesktopForm parent, MhwSdk sdk, string selectedMhwAccountId )
		{
			parentForm = parent;
			this.sdk = sdk;
			this.selectedMhwAccountId = selectedMhwAccountId;

			LoadPrintToFaxState();
			isLoggedIn = true;
		}

		private void LoadPrintToFaxState()
		{
			// Is MHW Fax installed on MHW account?
			try
			{
				sdk.Application.GetConnection( selectedMhwAccountId, FAX_APP_ID );
				IsMhwFaxInstalled = true;
				buttonPrintToFaxInstall.Enabled = true;
			}
			catch( Exception )
			{
				// Check network connection status.
				parentForm.CheckNetworkStatus();

				IsMhwFaxInstalled = false;
				labelStatus.Text = "myHEALTHware Fax is not installed on selected account.";
				labelFaxLink.Text = "Learn more about myHEALTHware Fax";
				buttonPrintToFaxInstall.Enabled = false;
				radioButtonPrompt.Enabled = false;
				radioButtonSaveDraft.Enabled = false;

				StopMonitoring();
				return;
			}

			labelFaxLink.Text = "Open myHEALTHware Fax";
			isPrinterInstalled = virtualPrinter.IsPrinterAlreadyInstalled( MhwPrinter.PRINT_TO_FAX.PrinterName );

			if( isPrinterInstalled )
			{
				StartMonitoring();
				buttonPrintToFaxInstall.Text = "Uninstall Printer";
				labelStatus.Text = "Print to Fax is ready.";
			}
			else
			{
				StopMonitoring();
				buttonPrintToFaxInstall.Text = "Install Printer";
				labelStatus.Text = "Print to Fax printer is not installed.";
			}

			// Set the initial state of the radio buttons.
			radioButtonPrompt.Checked = Settings.Default.PrintToFaxPrompt;
			radioButtonSaveDraft.Checked = !Settings.Default.PrintToFaxPrompt;

			radioButtonPrompt.Enabled = isPrinterInstalled;
			radioButtonSaveDraft.Enabled = isPrinterInstalled;
		}

		private void RadioButtonsCheckedChanged( object sender, EventArgs e )
		{
			var radioButton = sender as RadioButton;
			if( radioButton == null || radioButton.Checked == false )
			{
				return;
			}

			var message = "";

			if( radioButton == radioButtonSaveDraft )
			{
				Settings.Default.PrintToFaxPrompt = false;
				//message = "Print to Fax will automatically save as draft fax";
			}
			else
			{
				Settings.Default.PrintToFaxPrompt = true;
				//message = "Print to Fax will prompt you to send fax with each print";
			}

			parentForm.SaveSettings( message );
		}

		private void ButtonPrintToFaxInstallClick( object sender, EventArgs e )
		{
			buttonPrintToFaxInstall.Enabled = false;

			if( isPrinterInstalled )
			{
				Uninstall();
			}
			else
			{
				Install();
			}

			buttonPrintToFaxInstall.Enabled = true;
		}

		public void Uninstall()
		{
			// Args for uninstall
			string setupArgs = "-a -u -f";

			// Launch setup process.
			var process = new Process { StartInfo = new ProcessStartInfo { FileName = "Setup.exe", Arguments = setupArgs } };

			process.Start();
			process.WaitForExit();

			// See if the printer is now uninstalled.
			LoadPrintToFaxState();
		}

		public void Install()
		{
			string setupArgs = "-a -f";

			// Launch setup process.
			var process = new Process { StartInfo = new ProcessStartInfo { FileName = "Setup.exe", Arguments = setupArgs } };

			process.Start();
			process.WaitForExit();

			// See if the printer is now installed.
			LoadPrintToFaxState();
		}

		private void StartMonitoring()
		{
			if( isWatcherRunning )
			{
				// Already running.
				return;
			}

			if( !isPrinterInstalled || !IsMhwFaxInstalled )
			{
				// Printer or app not installed.
				return;
			}

			// Create the folder watcher.
			localPathWatcher = new FileSystemWatcher();

			try
			{
				localPathWatcher.Path = Path.Combine( Path.GetTempPath(), MhwPrinter.PRINT_TO_FAX.MonitorName );
			}
			catch( ArgumentException ex )
			{
				localPathWatcher.Dispose();
				localPathWatcher = null;
				isWatcherRunning = false;
				parentForm.ShowBalloonError( "Start Print to Fax monitor failed: {0}", ex.Message );
				return;
			}

			localPathWatcher.EnableRaisingEvents = true;
			localPathWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.LastAccess;
			localPathWatcher.SynchronizingObject = this;
			localPathWatcher.Created += LocalPathWatcherCreated;
			isWatcherRunning = true;
			pictureStartedStopped.Image = Resources.started;
		}

		private void StopMonitoring()
		{
			if( localPathWatcher == null )
				return;

			try
			{
				localPathWatcher.Dispose();
				localPathWatcher = null;
			}
			catch( ArgumentException ex )
			{
				parentForm.ShowBalloonError( "Stop Print to Fax monitor error: {0}", ex.Message );
			}

			isWatcherRunning = false;
			pictureStartedStopped.Image = Resources.stopped;
		}

		private void LocalPathWatcherCreated( object sender, FileSystemEventArgs e )
		{
			ProcessNewPrintFile( e.FullPath, e.Name );
		}

		// The Watcher calls this method when a new file shows up in the watched folder.
		private void ProcessNewPrintFile( string fullPath, string name )
		{
			if( isLoggedIn == false )
			{
				StopMonitoring();
				parentForm.ShowBalloonError( "Please log in and try again. Print job deleted." );
				File.Delete( fullPath );
				return;
			}

			// Upload the printed PDF file.
			string fileId = parentForm.UploadFile( fullPath, name, null, true );

			if( fileId == null )
			{
				parentForm.ShowBalloonError( "Print to Fax failed: Upload {0} failed", fullPath );
				return;
			}

			if( Settings.Default.PrintToFaxPrompt )
			{
				LaunchSendFax( fileId );
			}
			else
			{
				// Create draft fax.
				ApiFax draftFax = new ApiFax { AccountId = selectedMhwAccountId, FileId = fileId, To = new List<ApiFaxRecipient>() };

				try
				{
					sdk.Fax.Create( draftFax, true, false, null, null );
				}
				catch( Exception ex )
				{
					parentForm.ShowBalloonError( "Print to Fax create draft failed: {0}", ex.Message );
				}
			}
		}

		private void LaunchSendFax( string fileId )
		{
			sendFax = new SendFax( MhwDesktopForm.APP_ID, MhwDesktopForm.APP_SECRET );
			sendFax.InitBrowser( parentForm.ConnectionId, parentForm.AccessToken, selectedMhwAccountId, fileId );

			// Register a method to recieve click event callback.
			sendFax.Click += SendFaxOnClick;

			sendFax.ShowDialog( this );
		}

		private void SendFaxOnClick( object sender, EventArgs e )
		{
			GetSendFaxResults( (PostMessageListenerEventArgs) e );
		}

		// This method is called when SendFax fires the Click event so that we can
		// retrieve the results from it.
		private void GetSendFaxResults( PostMessageListenerEventArgs args )
		{
			if( args.Message.eventType == "mhw.fax.send.success" )
			{
				isSendFaxSuccess = true;
			}
			else if( args.Message.eventType == "mhw.fax.send.cancelled" )
			{
				// Cancelled.
				isSendFaxSuccess = false;
				parentForm.ShowBalloonWarning( "Print to Fax was cancelled." );
			}
			else
			{
				isSendFaxSuccess = false;
				parentForm.ShowBalloonInfo( "Send Fax Error: {0}", args.Message.data.message );
			}

			// Close the Drive picker form in a cross thread acceptable way.
			BeginInvoke( (MethodInvoker) ( () => sendFax.Dispose() ) );
		}

		public void LogOut()
		{
			isLoggedIn = false;
		}

		private void LabelFaxLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			string mhwLink = IsMhwFaxInstalled
				? string.Format( "{0}/#/Fax?accountId={1}", Settings.Default.myHEALTHwareDomain, selectedMhwAccountId )
				: string.Format( "{0}/#/Marketplace?accountId={1}", Settings.Default.myHEALTHwareDomain, selectedMhwAccountId );

			Process.Start( mhwLink );
		}
	}
}
