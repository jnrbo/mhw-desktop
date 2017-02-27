using System;
using System.IO;
using System.Windows.Forms;
using myHEALTHwareDesktop.Properties;
using MHWVirtualPrinter;
using Setup;
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
		private bool isDrivePickerSuccess;
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

		private bool IsUploadPathSet
		{
			get { return !string.IsNullOrWhiteSpace( userSession.Settings.PrintToDriveDefaultDestinationId ); }
		}

		private bool IsPrintToDriveInstalled
		{
			get { return virtualPrinterManager.IsPrinterAlreadyInstalled( MhwPrinter.PRINT_TO_DRIVE.PrinterName ); }
		}

		private bool IsWatcherRunning
		{
			get { return localPathWatcher != null && localPathWatcher.EnableRaisingEvents; }
		}

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
			if( IsPrintToDriveInstalled )
			{
				buttonPrintToDriveInstall.Text = "Uninstall Printer";
			}
			else
			{
				buttonPrintToDriveInstall.Text = "Install Printer";
				labelMonitorStatus.Text = "Print to Drive printer is not installed.";

				// Clear any errors.
				ClearDriveFolderMessage();
			}

			radioButtonPrompt.Enabled = IsPrintToDriveInstalled;
			radioButtonUseDefault.Enabled = IsPrintToDriveInstalled;
			textBoxPrintToDriveFolder.Enabled = IsPrintToDriveInstalled;
			buttonBrowsePrintToDrivePath.Enabled = IsPrintToDriveInstalled;

			return IsPrintToDriveInstalled;
		}

		private void InitRadioButtons()
		{
			// Set the initial state of the radio buttons.
			if( IsUploadPathSet )
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
				ClearDriveFolderMessage();

				StartMonitoring();
			}
			else
			{
				// Enable the Drive path and browse button.
				textBoxPrintToDriveFolder.Enabled = true;
				buttonBrowsePrintToDrivePath.Enabled = true;

				// Make sure current value is valid.
				LoadDefaultDriveLocation();

				if( IsUploadPathSet )
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
			if( !IsUploadPathSet )
			{
				ResetDriveFolderState();
				return;
			}

			try
			{
				LoadDriveLocation( userSession.Settings.PrintToDriveDefaultDestinationId );
			}
			catch( Exception )
			{
				userSession.Settings.PrintToDriveDefaultDestinationId = null;
				userSession.Settings.Save();

				ResetDriveFolderState();
			}
		}

		private void ResetDriveFolderState()
		{
			errorProviderDriveFolder.SetError( textBoxPrintToDriveFolder, "Please select a Drive folder to upload files to." );
			SetUploadPathText( "" );
		}

		private void LoadDriveLocation( string folderId )
		{
			// Using SDK, retrieve Drive Item's full path
			ApiDriveItem item = Sdk.DriveItems.GetDriveItem( userSession.ActingAsAccount.AccountId,
			                                                 TranslateDriveItemId( folderId ) );

			string path = string.Format( "{0}/{1}", item.Path, item.Name );
			SetUploadPathText( path );
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

			if( IsPrintToDriveInstalled )
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
			ResetSettings();

			// Launch setup process with args to uninstall.
			MhwSetup.LaunchAndWaitForExit( setupArgs );

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
			MhwSetup.LaunchAndWaitForExit( setupArgs );

			// Reset previous settings.
			ResetSettings();

			// Verify the printer is now installed and set state.
			if( LoadPrinterInstalled() )
			{
				InitRadioButtons();
				SetPromptOrDefaultState();
			}
		}

		private void ResetSettings()
		{
			userSession.Settings.PrintToDriveDefaultDestinationId = null;
			SetUploadPathText( "" );
			userSession.Settings.PrintToDrivePrompt = true;
			userSession.Settings.Save();
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
			}
		}

		private void ButtonBrowsePrintToDrivePathClick( object sender, EventArgs e )
		{
			LaunchDrivePicker();
			if( !isDrivePickerSuccess )
			{
				return;
			}

			// Valid. Clear any previous error.
			ClearDriveFolderMessage();

			try
			{
				// Display folder in dialog box.
				LoadDriveLocation( drivePickerResult );
			}
			catch( Exception ex )
			{
				NotificationService.ShowBalloonError( "Could not retrieve upload Drive folder" );
				return;
			}

			// Save Drive item ID.
			userSession.Settings.PrintToDriveDefaultDestinationId = drivePickerResult;
			userSession.Settings.Save();

			StartMonitoring();
		}

		private void LaunchDrivePicker( string fileName = null )
		{
			drivePicker = new DrivePicker( userSession, fileName );

			// Register a method to receive click event callback.
			drivePicker.Click += DrivePickerOnClick;

			try
			{
				drivePicker.ShowDialog( this );
			}
			finally
			{
				drivePicker.Dispose();
			}
		}

		private void ClearDriveFolderMessage()
		{
			errorProviderDriveFolder.SetError( textBoxPrintToDriveFolder, "" );
		}

		private string TranslateDriveItemId( string folderId )
		{
			return folderId != userSession.ActingAsAccount.AccountId ? folderId : null;
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
				// NOTE: If the drive-select-dialog returns a null folder selection then that needs to be interpreted as the root drive folder.
				// To upload to the root folder the account ID needs to be sent as the folder ID instead of null.
				drivePickerResult = args.Message.data.driveItemId ?? userSession.ActingAsAccount.AccountId;
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
			if( IsWatcherRunning )
			{
				// Already running.
				return;
			}

			if( !IsPrintToDriveInstalled )
			{
				// Printer not installed.
				return;
			}

			if( !userSession.Settings.PrintToDrivePrompt && !IsUploadPathSet )
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

				NotificationService.ShowBalloonError( "Start Print to Drive monitor failed: {0}", ex.Message );
				return;
			}

			localPathWatcher.EnableRaisingEvents = true;
			localPathWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.LastAccess;
			localPathWatcher.SynchronizingObject = this;
			localPathWatcher.Created += LocalPathWatcherCreated;

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

			string driveItemId = userSession.Settings.PrintToDriveDefaultDestinationId;
			string fileName = name;
			string extension = Path.GetExtension( fileName );

			if( userSession.Settings.PrintToDrivePrompt )
			{
				LaunchDrivePicker( name );

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

			// Make sure the extension didn't get removed
			fileName = Path.ChangeExtension( fileName, extension );

			string fileId = UploadService.UploadFile( fullPath, fileName, driveItemId );

			File.Delete( fullPath );

			if( fileId == null )
			{
				NotificationService.ShowBalloonError( "Print to Drive failed: {0}", fileName ?? name );
			}
			else
			{
				NotificationService.ShowBalloonInfo( "Print to Drive succeeded: {0}", fileName );
			}
		}

		private delegate void SetTextCallback( string text );
	}
}