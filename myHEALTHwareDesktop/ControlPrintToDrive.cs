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
		private readonly VirtualPrinterManager virtualPrinterManager = new VirtualPrinterManager();
		private DrivePicker drivePicker;
		private string drivePickerFileName;
		private string drivePickerResult;
		private bool isDefaultUploadPathSet;
		private bool isDrivePickerSuccess;
		//private bool isLoggedIn;
		private bool isPrintToDriveInstalled;
		private bool isWatcherRunning;
		private FileSystemWatcher localPathWatcher;
		private readonly ActiveUserSession userSession;

		public ControlPrintToDrive()
		{
			InitializeComponent();

			userSession = ActiveUserSession.GetInstance();
			userSession.ActingAsChanged += SelectedUserChanged;
		}

		private MhwSdk Sdk
		{
			get { return userSession.Sdk; }
		}

		public INotificationService NotificationService { get; set; }
		public IUploadService UploadService { get; set; }

		private void SelectedUserChanged( object sender, EventArgs e )
		{
			if( !LoadPrinterInstalled() )
			{
				return;
			}

			SetPromptOrDefaultState();
			InitRadioButtons();
		}

		private bool LoadPrinterInstalled()
		{
			isPrintToDriveInstalled = virtualPrinterManager.IsPrinterAlreadyInstalled( MhwPrinter.PRINT_TO_DRIVE.PrinterName );

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
				radioButtonPrompt.Checked = userSession.Settings.PrintToDrivePrompt;
				radioButtonUseDefault.Checked = !userSession.Settings.PrintToDrivePrompt;
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
			if( userSession.Settings.PrintToDrivePrompt )
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

		private void LoadDefaultDriveLocation()
		{
			if( string.IsNullOrWhiteSpace( userSession.Settings.PrintToDriveDefaultDestinationId ) )
			{
				errorProviderDriveFolder.SetError( textBoxPrintToDriveFolder, "Please select a Drive folder to upload files to." );

				isDefaultUploadPathSet = false;
				return;
			}

			ApiDriveItem item;

			// Using SDK, retrieve Drive Item's full path
			try
			{
				item = Sdk.DriveItems.GetDriveItem( userSession.ActingAsAccount.AccountId,
				                                    userSession.Settings.PrintToDriveDefaultDestinationId );
			}
			catch( Exception ex )
			{
				isDefaultUploadPathSet = false;
				userSession.Settings.PrintToDriveDefaultDestinationId = "";

				string message = string.Format( "Error setting Drive path: {0}", ex.Message.Substring( 0, 60 ) );
				errorProviderDriveFolder.SetError( textBoxPrintToDriveFolder, message );

				return;
			}

			string path = string.Format( "{0}/{1}", item.Path, item.Name );
			SetUploadPathText( path );
			isDefaultUploadPathSet = true;
		}

		private void RadioButtonsCheckedChanged( object sender, EventArgs e )
		{
			var radioButton = sender as RadioButton;

			if( radioButton == null || radioButton.Checked == false )
			{
				return;
			}

			////var message = "";

			if( radioButton == radioButtonUseDefault )
			{
				userSession.Settings.PrintToDrivePrompt = false;
				//message = "Print to Drive will upload to default Drive location";
			}
			else
			{
				userSession.Settings.PrintToDrivePrompt = true;
				//message = "Print to Drive will prompt you for Drive location with each print";
			}

			userSession.Settings.Save();
			////NotificationService.ShowBalloonInfo(message);

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
			userSession.Settings.PrintToDriveDefaultDestinationId = "";
			SetUploadPathText( "" );
			userSession.Settings.PrintToDrivePrompt = true;
			userSession.Settings.Save();

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
			userSession.Settings.PrintToDriveDefaultDestinationId = "";
			SetUploadPathText( "" );
			userSession.Settings.PrintToDrivePrompt = true;
			userSession.Settings.Save();

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
			LaunchDrivePicker();

			if( isDrivePickerSuccess )
			{
				userSession.Settings.PrintToDriveDefaultDestinationId = drivePickerResult;
			}
		}

		private void LaunchDrivePicker( string fileName = null )
		{
			drivePicker = new DrivePicker( userSession, fileName );

			// Register a method to receive click event callback.
			drivePicker.Click += DrivePickerOnClick;

			drivePicker.ShowDialog( this );
			if( !isDrivePickerSuccess )
			{
				return;
			}

			// Valid. Clear any previous error.
			errorProviderDriveFolder.SetError( textBoxPrintToDriveFolder, "" );

			// Display folder in dialog box.
			SetUploadPath( drivePickerResult );

			// Save Drive item ID.
			userSession.Settings.Save();

			StartMonitoring();
		}

		private void SetUploadPath( string folderId )
		{
			ApiDriveItem item;

			// Using SDK, retrieve Drive Item's full path
			try
			{
				item = Sdk.DriveItems.GetDriveItem( userSession.ActingAsAccount.AccountId, folderId );
			}
			catch( Exception ex )
			{
				NotificationService.ShowBalloonError( "Could not retrieve upload Drive folder: {0}", ex.Message );
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
				drivePickerFileName = args.Message.data.itemName;
				isDrivePickerSuccess = true;
			}
			else if( args.Message.eventType == "mhw.drive.select.cancelled" )
			{
				isDrivePickerSuccess = false;
				////parentForm.ShowBalloonWarning( "Print to Drive upload was cancelled." );
			}
			else
			{
				isDrivePickerSuccess = false;
				NotificationService.ShowBalloonError( "Print to Drive Error: {0}", args.Message.data.message );
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

			if( !userSession.Settings.PrintToDrivePrompt && !isDefaultUploadPathSet )
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
				NotificationService.ShowBalloonError( "Start Print to Drive monitor failed: {0}", ex.Message );
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
				NotificationService.ShowBalloonError( "Stop Print to Drive monitor error: {0}", ex.Message );
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
			if( userSession.IsLoggedIn == false )
			{
				StopMonitoring();
				NotificationService.ShowBalloonError( "Please log in and try again. Print job deleted." );
				File.Delete( fullPath );
				return;
			}

			string accountId = userSession.ActingAsAccount.AccountId;
			string driveItemId = userSession.Settings.PrintToDriveDefaultDestinationId;
			string fileName = name;
			string extension = Path.GetExtension( fileName );

			if( userSession.Settings.PrintToDrivePrompt )
			{
				LaunchDrivePicker( name );

				if( isDrivePickerSuccess )
				{
					driveItemId = drivePickerResult ?? accountId;
					fileName = drivePickerFileName;
				}
				else
				{
					File.Delete( fullPath );
					return;
				}
			}

			// Make sure the extension didn't get removed
			fileName = Path.ChangeExtension( fileName, extension );

			// NOTE: If the drive-select-dialog returns a null folder selection then that needs to be interpreted as the root drive folder.
			// To upload to the root folder the account ID needs to be sent as the folder ID instead of null.
			string fileId = UploadService.UploadFile( fullPath, fileName, driveItemId ?? accountId );

			File.Delete( fullPath );

			if( fileId == null )
			{
				NotificationService.ShowBalloonError( "Print to Drive failed: {0}", name );
			}
			else
			{
				NotificationService.ShowBalloonInfo( "Print to Drive succeeded: {0}", name );
			}
		}

		private delegate void SetTextCallback( string text );
	}
}