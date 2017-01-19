using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using myHEALTHwareDesktop.Properties;
using SOAPware.PortalApi.Model.Drive;
using SOAPware.PortalSdk;

namespace myHEALTHwareDesktop
{
	public partial class ControlFolderToDrive : UserControl
	{
		private MhwDesktopForm parentForm;
		private MhwSdk sdk;
		private string selectedMhwAccountId;
		private DrivePicker drivePicker;
		private FileSystemWatcher localPathWatcher;
		private bool isWatcherRunning;
		private bool isLocalPathSet;
		private bool isUploadPathSet;
		private bool isDrivePickerSuccess;
		private string uploadDriveItemId;
		private bool isLoggedIn;

		public ControlFolderToDrive()
		{
			InitializeComponent();
		}

		public void LoadSettings( MhwDesktopForm parent, MhwSdk sdk, string selectedMhwAccountId )
		{
			parentForm = parent;
			this.sdk = sdk;
			this.selectedMhwAccountId = selectedMhwAccountId;
			isLoggedIn = true;

			LoadSettingDeleteFileAfterUpload();
			SetLocalPath( Settings.Default.FolderToDriveLocalPath );
			LoadDriveLocation( Settings.Default.FolderToDriveDestinationId );

			// If all set, start monitoring.
			if( Settings.Default.FolderToDriveRunning && isLocalPathSet && isUploadPathSet )
			{
				StartMonitoring();
			}
		}

		private void LoadSettingDeleteFileAfterUpload()
		{
			deleteFileAfterUploadCheckBox.Checked = Settings.Default.FolderToDriveDeleteFileAfterUpload;

			deleteFileAfterUploadCheckBox.CheckState = Settings.Default.FolderToDriveDeleteFileAfterUpload
				? CheckState.Checked
				: CheckState.Unchecked;
		}

		private void DeleteFileAfterUploadCheckBoxCheckedChanged( object sender, EventArgs e )
		{
			string message = deleteFileAfterUploadCheckBox.Checked
				? "Files will be deleted after upload."
				: "Files will not be deleted after upload. You may want to periodically clean out your local upload folder.";

			// Save the new value.
			Settings.Default.FolderToDriveDeleteFileAfterUpload = deleteFileAfterUploadCheckBox.Checked;
			parentForm.SaveSettings( message );
		}

		private void ButtonBrowseLocalPathClick( object sender, EventArgs e )
		{
			// Set initial folder to start browsing from.
			folderBrowserDialog.SelectedPath = textBoxLocalPath.Text;

			DialogResult result = folderBrowserDialog.ShowDialog();

			if( result == DialogResult.OK )
			{
				SetLocalPath( folderBrowserDialog.SelectedPath );
			}
		}

		// This method called when local directory is set manually via typing into text box.
		private void TextBoxLocalPathValidating( object sender, CancelEventArgs e )
		{
			SetLocalPath( textBoxLocalPath.Text );
		}

		private void SetLocalPath( string path )
		{
			if( string.IsNullOrWhiteSpace( path ) )
			{
				errorProviderLocalFolder.SetError( textBoxLocalPath, "Please select a local folder to monitor for files to upload." );
				isLocalPathSet = false;
				SetButtonState();
			}
			else if( !Directory.Exists( path ) )
			{
				textBoxLocalPath.Text = path;
				errorProviderLocalFolder.SetError( textBoxLocalPath, "The selected folder does not exist.  Please select a new one." );
				isLocalPathSet = false;
				SetButtonState();
			}
			else
			{
				// Valid. Clear any previous error.
				errorProviderLocalFolder.SetError( textBoxLocalPath, "" );

				textBoxLocalPath.Text = path;
				isLocalPathSet = true;
				SetButtonState();

				// Has the value actually changed since it was last valid?
				if( path != Settings.Default.FolderToDriveLocalPath )
				{
					Settings.Default.FolderToDriveLocalPath = path;
					parentForm.SaveSettings();
				}
			}
		}

		private string LoadDriveLocation( string folderId )
		{
			if( string.IsNullOrWhiteSpace( folderId ) )
			{
				errorProviderDriveFolder.SetError( textBoxMhwFolder, "Please select a Drive folder to upload files to." );
				isUploadPathSet = false;
				return null;
			}

			ApiDriveItem item;

			// Using SDK, retrieve Drive Item's full path
			try
			{
				item = sdk.DriveItems.GetDriveItem( selectedMhwAccountId, folderId );
			}
			catch( Exception ex )
			{
				isUploadPathSet = false;

				string message = string.Format( "Error setting Drive path: {0}", ex.Message.Substring( 0, 60 ) );
				errorProviderDriveFolder.SetError( textBoxMhwFolder, message );
				SetButtonState();
				return null;
			}

			// Valid. Clear any previous error.
			errorProviderDriveFolder.SetError( textBoxMhwFolder, "" );

			string path = string.Format( "{0}/{1}", item.Path, item.Name );
			SetUploadPathText( path );
			uploadDriveItemId = folderId;
			isUploadPathSet = true;
			return path;
		}

		private delegate void SetTextCallback( string text );

		// This method handles calling from cross thread or directly.
		private void SetUploadPathText( string text )
		{
			if( textBoxMhwFolder.InvokeRequired )
			{
				SetTextCallback d = SetUploadPathText;
				Invoke( d, text );
			}
			else
			{
				textBoxMhwFolder.Text = text;
			}
		}

		private void ButtonBrowseUploadPathClick( object sender, EventArgs e )
		{
			BrowseUploadPath();
		}

		private void BrowseUploadPath()
		{
			drivePicker = new DrivePicker( MhwDesktopForm.APP_ID, MhwDesktopForm.APP_SECRET );
			drivePicker.EnableFileName( false );
			drivePicker.InitBrowser( parentForm.ConnectionId, parentForm.AccessToken, selectedMhwAccountId );

			// Register a method to recieve click event callback.
			drivePicker.Click += drivePicker_OnClick;

			drivePicker.ShowDialog( this );

			if( isDrivePickerSuccess )
			{
				// Valid. Clear any previous error.
				errorProviderDriveFolder.SetError( textBoxMhwFolder, "" );

				// Display folder in dialog box.
				string path = LoadDriveLocation( uploadDriveItemId );

				// Save Drive item ID.
				Settings.Default.FolderToDriveDestinationId = uploadDriveItemId;
				parentForm.SaveSettings();
			}

			SetButtonState();
		}

		private void drivePicker_OnClick( object sender, EventArgs e )
		{
			GetDrivePickerResults( (PostMessageListenerEventArgs) e );
		}

		// This method is called when DrivePicker fires the Click event so that we can
		// retrieve the results from it.
		private void GetDrivePickerResults( PostMessageListenerEventArgs args )
		{
			if( args.Message.eventType == "mhw.drive.select.success" )
			{
				uploadDriveItemId = args.Message.data.driveItemId;
				isDrivePickerSuccess = true;
			}
			else if( args.Message.eventType == "mhw.drive.select.cancelled" )
			{
				// Cancelled.
				isDrivePickerSuccess = false;
			}
			else
			{
				isDrivePickerSuccess = false;
				parentForm.ShowBalloonError( "Drive picker error: {0}", args.Message.data.message );
			}

			// Close the Drive picker form in a cross thread acceptable way.
			BeginInvoke( (MethodInvoker) delegate { drivePicker.Dispose(); } );
		}

		private void SetButtonState()
		{
			if( isWatcherRunning )
			{
				pictureStartedStopped.Image = Resources.started;
				labelMonitorStatus.Text = "Folder to Drive monitor is running.";
				buttonStartStopWatcher.Text = "Stop";
				textBoxLocalPath.Enabled = false;
				textBoxMhwFolder.Enabled = false;
				buttonBrowseLocalPath.Enabled = false;
				buttonBrowseUploadPath.Enabled = false;
				buttonStartStopWatcher.Enabled = true;
			}
			else
			{
				pictureStartedStopped.Image = Resources.stopped;
				labelMonitorStatus.Text = "Folder monitor is stopped.";
				buttonStartStopWatcher.Text = "Start";
				textBoxLocalPath.Enabled = true;
				textBoxMhwFolder.Enabled = true;
				buttonBrowseLocalPath.Enabled = true;
				buttonBrowseUploadPath.Enabled = true;
				buttonStartStopWatcher.Enabled = isLocalPathSet && isUploadPathSet;
			}
		}

		private void ButtonStartStopMonitorClick( object sender, EventArgs e )
		{
			StartOrStopMonitor();
		}

		private void StartOrStopMonitor()
		{
			if( isWatcherRunning )
			{
				StopMonitoring();
				Settings.Default.FolderToDriveRunning = false;
				parentForm.SaveSettings( "Stopped Folder to Drive monitor" );
			}
			else
			{
				StartMonitoring();
				Settings.Default.FolderToDriveRunning = true;
				parentForm.SaveSettings( "Started Folder to Drive monitor" );
			}

			SetButtonState();
		}

		private void StartMonitoring()
		{
			if( !isLocalPathSet || !isUploadPathSet )
			{
				MessageBox.Show( "Please set local and Drive folders first.", "Error" );
				return;
			}

			// Create the folder watcher.
			localPathWatcher = new FileSystemWatcher();

			try
			{
				localPathWatcher.Path = textBoxLocalPath.Text;
			}
			catch( ArgumentException ex )
			{
				localPathWatcher.Dispose();
				localPathWatcher = null;
				isWatcherRunning = false;
				parentForm.ShowBalloonError( "Start Folder to Drive monitor failed: {0}", ex.Message );
				return;
			}

			localPathWatcher.EnableRaisingEvents = true;
			localPathWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.LastAccess;
			localPathWatcher.SynchronizingObject = this;
			localPathWatcher.Created += LocalPathWatcherCreated;

			isWatcherRunning = true;
		}

		private void StopMonitoring()
		{
			try
			{
				localPathWatcher.Dispose();
				localPathWatcher = null;
			}
			catch( ArgumentException ex )
			{
				parentForm.ShowBalloonError( "Stop Folder to Drive monitor error: {0}", ex.Message );
			}

			isWatcherRunning = false;
		}

		private void LocalPathWatcherCreated( object sender, FileSystemEventArgs e )
		{
			ProcessNewLocalFile( e.FullPath, e.Name );
		}

		// The Watcher calls this method when a new file shows up in the watched folder.
		private void ProcessNewLocalFile( string fullPath, string name )
		{
			if( !isLoggedIn )
			{
				StopMonitoring();
				parentForm.ShowBalloonError( "Please log in and try again. Folder to Drive file deleted." );
				File.Delete( fullPath );
				return;
			}

			if(
				parentForm.UploadFile( fullPath, name, uploadDriveItemId, Settings.Default.FolderToDriveDeleteFileAfterUpload ) ==
				null )
			{
				parentForm.ShowBalloonError( "Folder to Drive upload failed: {0}", name );
			}
		}

		public void LogOut()
		{
			isLoggedIn = false;
		}
	}
}
