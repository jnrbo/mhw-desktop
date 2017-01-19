using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using myHEALTHwareDesktop.Properties;
using MHWVirtualPrinter;
using SOAPware.PortalApi.Model.Drive;
using SOAPware.PortalSdk;

namespace myHEALTHwareDesktop
{
	public partial class ControlPrintToDrive : UserControl
	{
		private readonly VirtualPrinter virtualPrinter = new VirtualPrinter();
		private DrivePicker drivePicker;
		private string drivePickerFileName;
		private string drivePickerResult;
		private bool isDefaultUploadPathSet;
		private bool isDrivePickerSuccess;
		private bool isLoggedIn;
		private bool isPrintToDriveInstalled;
		private bool isWatcherRunning;
		private FileSystemWatcher localPathWatcher;
		private MhwDesktopForm parentForm;
		private MhwSdk sdk;
		private string selectedMhwAccountId;

		public ControlPrintToDrive()
		{
			InitializeComponent();
		}

		public void LoadSettings( MhwDesktopForm parent, MhwSdk sdk, string selectedMhwAccountId )
		{
			parentForm = parent;
			this.sdk = sdk;
			this.selectedMhwAccountId = selectedMhwAccountId;

			if( LoadPrinterInstalled() )
			{
				SetPromptOrDefaultState();
				InitRadioButtons();
			}

			isLoggedIn = true;
		}

		private bool LoadPrinterInstalled()
		{
			isPrintToDriveInstalled = virtualPrinter.IsPrinterAlreadyInstalled( MhwPrinter.PRINT_TO_DRIVE.PrinterName );

			if( isPrintToDriveInstalled )
			{
				buttonPrintToDriveInstall.Text = "Uninstall Printer";
			}
			else
			{
				buttonPrintToDriveInstall.Text = "Install Printer";
				labelMonitorStatus.Text = "Print to Drive printer is not installed.";

				// Clear any errors.
				errorProviderDriveFolder.SetError( textBoxPrintToDriveFolder, "" );
			}

			radioButtonPrompt.Enabled = isPrintToDriveInstalled;
			radioButtonUseDefault.Enabled = isPrintToDriveInstalled;
			textBoxPrintToDriveFolder.Enabled = isPrintToDriveInstalled;
			buttonBrowsePrintToDrivePath.Enabled = isPrintToDriveInstalled;

			return isPrintToDriveInstalled;
		}

		private void InitRadioButtons()
		{
			// Set the initial state of the radio buttons.
			if( isDefaultUploadPathSet )
			{
				radioButtonPrompt.Checked = Settings.Default.PrintToDrivePrompt;
				radioButtonUseDefault.Checked = !Settings.Default.PrintToDrivePrompt;
			}
			else
			{
				// Default location not set so force to prompt.
				radioButtonPrompt.Checked = true;
				radioButtonUseDefault.Checked = false;
			}
		}

		private void SetPromptOrDefaultState()
		{
			if( Settings.Default.PrintToDrivePrompt )
			{
				// Disable the Drive path and browse button.
				textBoxPrintToDriveFolder.Enabled = false;
				buttonBrowsePrintToDrivePath.Enabled = false;

				// Clear any errors.
				errorProviderDriveFolder.SetError( textBoxPrintToDriveFolder, "" );

				StartMonitoring();
			}
			else
			{
				// Enable the Drive path and browse button.
				textBoxPrintToDriveFolder.Enabled = true;
				buttonBrowsePrintToDrivePath.Enabled = true;

				// Make sure current value is valid.
				LoadDefaultDriveLocation();

				if( isDefaultUploadPathSet )
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
			if( string.IsNullOrWhiteSpace( Settings.Default.PrintToDriveDefaultDestinationId ) )
			{
				errorProviderDriveFolder.SetError( textBoxPrintToDriveFolder, "Please select a Drive folder to upload files to." );

				isDefaultUploadPathSet = false;
				return null;
			}

			ApiDriveItem item;

			// Using SDK, retrieve Drive Item's full path
			try
			{
				item = sdk.DriveItems.GetDriveItem( selectedMhwAccountId, Settings.Default.PrintToDriveDefaultDestinationId );
			}
			catch( Exception ex )
			{
				isDefaultUploadPathSet = false;
				Settings.Default.PrintToDriveDefaultDestinationId = "";

				string message = string.Format( "Error setting Drive path: {0}", ex.Message.Substring( 0, 60 ) );
				errorProviderDriveFolder.SetError( textBoxPrintToDriveFolder, message );

				return null;
			}

			string path = string.Format( "{0}/{1}", item.Path, item.Name );
			SetUploadPathText( path );
			isDefaultUploadPathSet = true;
			return path;
		}

		private void RadioButtonsCheckedChanged( object sender, EventArgs e )
		{
			var radioButton = sender as RadioButton;

			if( radioButton == null || radioButton.Checked == false )
			{
				return;
			}

			var message = "";

			if( radioButton == radioButtonUseDefault )
			{
				Settings.Default.PrintToDrivePrompt = false;
				//message = "Print to Drive will upload to default Drive location";
			}
			else
			{
				Settings.Default.PrintToDrivePrompt = true;
				//message = "Print to Drive will prompt you for Drive location with each print";
			}

			parentForm.SaveSettings( message );

			SetPromptOrDefaultState();
		}

		private void ButtonPrintToDriveInstallClick( object sender, EventArgs e )
		{
			buttonPrintToDriveInstall.Enabled = false;

			if( isPrintToDriveInstalled )
			{
				Uninstall();
			}
			else
			{
				Install();
			}

			buttonPrintToDriveInstall.Enabled = true;
		}

		public void Uninstall()
		{
			// Args to perform uninstall
			var setupArgs = "-a -u -d";

			StopMonitoring();

			// Reset previous settings.
			isDefaultUploadPathSet = false;
			Settings.Default.PrintToDriveDefaultDestinationId = "";
			SetUploadPathText( "" );
			Settings.Default.PrintToDrivePrompt = true;
			parentForm.SaveSettings();

			// Launch setup process with args to uninstall.
			var process = new Process { StartInfo = new ProcessStartInfo { FileName = "Setup.exe", Arguments = setupArgs } };

			process.Start();
			process.WaitForExit();

			// Verify the printer is now uninstalled and set state.
			if( LoadPrinterInstalled() )
			{
				SetPromptOrDefaultState();
			}
		}

		public void Install()
		{
			var setupArgs = "-a -d";

			// Launch setup process with args to install.
			var process = new Process { StartInfo = new ProcessStartInfo { FileName = "Setup.exe", Arguments = setupArgs } };

			process.Start();
			process.WaitForExit();

			// Reset previous settings.
			isDefaultUploadPathSet = false;
			Settings.Default.PrintToDriveDefaultDestinationId = "";
			SetUploadPathText( "" );
			Settings.Default.PrintToDrivePrompt = true;
			parentForm.SaveSettings();

			// Verify the printer is now installed and set state.
			if( LoadPrinterInstalled() )
			{
				InitRadioButtons();
				SetPromptOrDefaultState();
			}
		}

		// This method handles calling from cross thread or directly.
		private void SetUploadPathText( string text )
		{
			if( textBoxPrintToDriveFolder.InvokeRequired )
			{
				SetTextCallback d = SetUploadPathText;
				Invoke( d, text );
			}
			else
			{
				textBoxPrintToDriveFolder.Text = text;

				// Valid. Clear any previous error.
				errorProviderDriveFolder.SetError( textBoxPrintToDriveFolder, "" );
			}
		}

		private void ButtonBrowsePrintToDrivePathClick( object sender, EventArgs e )
		{
			LaunchDrivePicker( false );

			if( isDrivePickerSuccess )
			{
				Settings.Default.PrintToDriveDefaultDestinationId = drivePickerResult;
			}
		}

		private void LaunchDrivePicker( bool isShowFileName, string defaultFileName = null )
		{
			drivePicker = new DrivePicker( MhwDesktopForm.APP_ID, MhwDesktopForm.APP_SECRET );
			drivePicker.EnableFileName( isShowFileName, defaultFileName );
			drivePicker.InitBrowser( parentForm.ConnectionId, parentForm.AccessToken, selectedMhwAccountId );

			// Register a method to receive click event callback.
			drivePicker.Click += DrivePickerOnClick;

			drivePicker.ShowDialog( this );
			if( !isDrivePickerSuccess )
			{
				return;
			}

			// Valid. Clear any previous error.
			errorProviderDriveFolder.SetError( textBoxPrintToDriveFolder, "" );

			if( isShowFileName )
			{
				drivePickerFileName = drivePicker.GetFileName();
			}

			// Display folder in dialog box.
			SetUploadPath( drivePickerResult );

			// Save Drive item ID.
			parentForm.SaveSettings();

			StartMonitoring();
		}

		private void SetUploadPath( string folderId )
		{
			ApiDriveItem item;

			// Using SDK, retrieve Drive Item's full path
			try
			{
				item = sdk.DriveItems.GetDriveItem( selectedMhwAccountId, folderId );
			}
			catch( Exception ex )
			{
				parentForm.ShowBalloonError( "Could not retrieve upload Drive folder: {0}", ex.Message );
				return;
			}

			string path = string.Format( "{0}/{1}", item.Path, item.Name );
			SetUploadPathText( path );
			isDefaultUploadPathSet = true;
		}

		private void DrivePickerOnClick( object sender, EventArgs e )
		{
			GetDrivePickerResults( (PostMessageListenerEventArgs) e );
		}

		// This method is called when DrivePicker fires the Click event so that we can
		// retrieve the results from it.
		private void GetDrivePickerResults( PostMessageListenerEventArgs args )
		{
			if( args.Message.eventType == "mhw.drive.select.success" )
			{
				drivePickerResult = args.Message.data.driveItemId;
				isDrivePickerSuccess = true;
			}
			else if( args.Message.eventType == "mhw.drive.select.cancelled" )
			{
				isDrivePickerSuccess = false;
				parentForm.ShowBalloonWarning( "Print to Drive upload was cancelled." );
			}
			else
			{
				isDrivePickerSuccess = false;
				parentForm.ShowBalloonError( "Print to Drive Error: {0}", args.Message.data.message );
			}

			// Close the Drive picker form in a cross thread acceptable way.
			BeginInvoke( (MethodInvoker) delegate { drivePicker.Dispose(); } );
		}

		private void StartMonitoring()
		{
			if( isWatcherRunning )
			{
				// Already running.
				return;
			}

			if( !isPrintToDriveInstalled )
			{
				// Printer not installed.
				return;
			}

			if( !Settings.Default.PrintToDrivePrompt && !isDefaultUploadPathSet )
			{
				// Configured to upload to a default Drive path but it is not set.
				return;
			}

			// Create the folder watcher.
			localPathWatcher = new FileSystemWatcher();

			try
			{
				localPathWatcher.Path = Path.Combine( Path.GetTempPath(), MhwPrinter.PRINT_TO_DRIVE.MonitorName );
			}
			catch( ArgumentException ex )
			{
				localPathWatcher.Dispose();
				localPathWatcher = null;
				isWatcherRunning = false;
				parentForm.ShowBalloonError( "Start Print to Drive monitor failed: {0}", ex.Message );
				return;
			}

			localPathWatcher.EnableRaisingEvents = true;
			localPathWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.LastAccess;
			localPathWatcher.SynchronizingObject = this;
			localPathWatcher.Created += LocalPathWatcherCreated;
			isWatcherRunning = true;
			pictureStartedStopped.Image = Resources.started;
			labelMonitorStatus.Text = "Print to Drive monitor is running.";
		}

		private void StopMonitoring()
		{
			if( localPathWatcher == null )
			{
				return;
			}

			try
			{
				localPathWatcher.Dispose();
				localPathWatcher = null;
			}
			catch( ArgumentException ex )
			{
				parentForm.ShowBalloonError( "Stop Print to Drive monitor error: {0}", ex.Message );
			}

			isWatcherRunning = false;
			pictureStartedStopped.Image = Resources.stopped;
			labelMonitorStatus.Text = "Print to Drive monitor is stopped.";
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

			string driveItemId = Settings.Default.PrintToDriveDefaultDestinationId;
			string fileName = name;

			if( Settings.Default.PrintToDrivePrompt )
			{
				LaunchDrivePicker( true, name );

				if( isDrivePickerSuccess )
				{
					driveItemId = drivePickerResult;
					fileName = drivePickerFileName;
				}
				else
				{
					File.Delete( fullPath );
					return;
				}
			}

			if( parentForm.UploadFile( fullPath, fileName, driveItemId, true ) == null )
			{
				parentForm.ShowBalloonError( "Print to Drive upload failed: {0}", name );
			}
		}

		public void LogOut()
		{
			isLoggedIn = false;
		}

		private delegate void SetTextCallback( string text );
	}
}