using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using myHEALTHwareDesktop.Properties;
using MHWVirtualPrinter;
using Microsoft.Win32;
using SOAPware.PortalApi.Models;
using SOAPware.PortalSdk;

// Credit: Winform Icon tray code initially gen'd from template: http://saveontelephoneservices.com/modules.php?name=News&file=article&sid=8

namespace myHEALTHwareDesktop
{
	public partial class MhwDesktopForm : Form, INotificationService, IUploadService
	{
		public const string APP_NAME = "myHEALTHware Desktop";

		private readonly ActiveUserSession userSession;

		private LoginForm loginForm;
		private int faxTabIndex;

		private readonly BindingSource accountsBindingSource;

		private bool isTimeToQuit;
		private bool isLastMouseClickLeft;

		private MhwSdk Sdk
		{
			get { return userSession.Sdk; }
		}

		public MhwDesktopForm()
		{
			// Run windows generated form code.
			InitializeComponent();

			// Create our "FormClosing" event handler.
			FormClosing += MhwCloseRequested;

			// Load persisted settings values.
			LoadRunAtStartup();

			accountsBindingSource = new BindingSource();
			userSession = ActiveUserSession.GetInstance();

			controlPrintToDrive.NotificationService = this;
			controlPrintToDrive.UploadService = this;

			controlFolderToDrive.NotificationService = this;
			controlFolderToDrive.UploadService = this;

			controlPrintToFax.NotificationService = this;
			controlPrintToFax.UploadService = this;
		}

		private async void MhwFormInitialize( object sender, EventArgs e )
		{
			// Make sure that standard and standard double clicks are enabled,
			// if they are not enabled, enable them. This really is OverKill but better to be
			// safe than sorry.
			if( !GetStyle( ControlStyles.StandardClick ) )
			{
				SetStyle( ControlStyles.StandardClick, true );
			}

			if( !GetStyle( ControlStyles.StandardDoubleClick ) )
			{
				SetStyle( ControlStyles.StandardDoubleClick, true );
			}

			faxTabIndex = tabs.TabPages.IndexOf( tabPagePrintToFax );

			// Main form event handlers.
			MouseClick += MhwSingleClick;
			MouseDoubleClick += MhwDoubleClick;
			MouseDown += MhwMouseDown;

			// NotifyIcon event handlers.
			trayIcon.MouseClick += MhwSingleClick;
			trayIcon.MouseDoubleClick += MhwDoubleClick;
			trayIcon.MouseDown += MhwMouseDown;

			aboutToolStripMenuItem.Click += MenuAboutClicked;
			quitToolStripMenuItem.Click += MenuQuitClicked;
			settingsToolStripMenuItem.Click += MenuSettingsClicked;

			MhwMinimizeToTray();

			Application.DoEvents();

			// SettingsForm Resize event handler. 
			// NOTE: Must be after our initial display logic
			// otherwise if it was before, it will cause logic problems.
			Resize += FormResize;

			accountsBindingSource.DataSource = new BindingList<MhwAccount>();
			pictureBoxActingAs.DataBindings.Add( "Image", accountsBindingSource, "ProfilePic", true );
			accountsComboBox.DataSource = accountsBindingSource;

			// Try the previous credentials.
			string connectionId = userSession.Settings.ConnectionId;
			string accessToken = userSession.Settings.AccessToken;

			if( !string.IsNullOrEmpty( connectionId ) && !string.IsNullOrEmpty( accessToken ) )
			{
				await ExecuteLogin( connectionId, accessToken );
			}
			if( !userSession.IsLoggedIn )
			{
				await ChangeLogin();
			}
			if( !userSession.IsLoggedIn )
			{
				LogOut();
			}

			UpdateLoginButtonText();
			RefreshTabControl();

			// We now at this point, are completely event driven.
		}

		// This event handler is fired by changes caused in WindowState by the "-" to
		// minimize on the main form, as well as the display and minimize menu items in the
		// ContextMenuStrips.
		private void FormResize( object sender, EventArgs e )
		{
			if( WindowState == FormWindowState.Minimized )
			{
				MhwMinimizeToTray();
			}
		}

		// Minimize the settings form to the system tray.
		private void MhwMinimizeToTray()
		{
			trayIcon.Visible = true;

			Hide(); // this.Visible = false is the same thing.

			// Remove us from the Taskbar.
			ShowInTaskbar = false;

			WindowState = FormWindowState.Minimized;

			settingsToolStripMenuItem.Enabled = true;

			if( !userSession.Settings.IsFirstUse )
			{
				return;
			}

			trayIcon.ShowBalloonTip( 2000,
			                         // Show for 2 seconds
			                         APP_NAME,
			                         "myHEALTHware has minimized here, as an icon in the System Tray." +
			                         " Right-click this icon to see menu of actions.",
			                         ToolTipIcon.Info );

			userSession.Settings.IsFirstUse = false;
			SaveSettings( null );
		}

		// Display the settings form.
		private async Task MhwDisplaySettings()
		{
			Show(); // Equivalent to this.Visible = true.
			Activate();

			WindowState = FormWindowState.Normal;
			ShowInTaskbar = true;
			settingsToolStripMenuItem.Enabled = false;

			if( !userSession.IsLoggedIn )
			{
				await ChangeLogin();
			}

			if( userSession.IsLoggedIn )
			{
				await RefreshOnBehalfOfAccounts();
				userSession.NotifyActingAccountRefresh();
			}
		}

		// This event handler handles our MouseDown events for both SettingsForm and NotifyIcon.
		// We use this event handler to help us keep track of where the MouseClick came from.
		private void MhwMouseDown( object sender, MouseEventArgs e )
		{
			// Was it a left or right MouseDown?
			isLastMouseClickLeft = e.Button == MouseButtons.Left;
		}

		// This event handler handles single MouseClicks for both main form and our NotifyIcon.
		// Note: This event handler fires twice for double MouseClicks.
		private void MhwSingleClick( object sender, MouseEventArgs e )
		{
			// When using NotifyIcon and dealing with MouseClicks,
			// we need to always make sure main form is in Focus after MouseClicks on NotifyIcon.
			if( WindowState == FormWindowState.Normal && Visible && !Focused )
			{
				Activate();
			}
		}

		// This event handler handles both left and right doubleclicks for both Form
		// and NotifyIcon.
		private void MhwDoubleClick( object sender, MouseEventArgs e )
		{
			// Left double-click on notify icon.
			if( sender == trayIcon && isLastMouseClickLeft )
			{
				MenuSettingsClicked( sender, e );
			}
		}

		private async void MenuSettingsClicked( object sender, EventArgs e )
		{
			await MhwDisplaySettings();
		}

		private async void MenuAboutClicked( object sender, EventArgs e )
		{
			tabs.SelectedTab = tabPageAbout;
			await MhwDisplaySettings();
		}

		private void MenuQuitClicked( object sender, EventArgs e )
		{
			// It is really time to quit.
			isTimeToQuit = true;

			// Create a Close request.
			Close();
		}

		// This event handler gets called when we receive any Close(); request.
		private void MhwCloseRequested( object sender, FormClosingEventArgs e )
		{
			if( !isTimeToQuit && e.CloseReason == CloseReason.UserClosing )
			{
				// Override app "X" click and just minimize without app close.
				MhwMinimizeToTray();
				e.Cancel = true;
			}
			else
			{
				// Make sure try icon is hidden before closing.
				trayIcon.Visible = false;
			}
		}

		public void SetTrayIcon( Icon icon = null, string message = null )
		{
			trayIcon.Icon = icon ?? Resources.myHEALTHware;

			if( string.IsNullOrWhiteSpace( message ) )
			{
				trayIcon.Text = APP_NAME;
			}
			else
			{
				// Must limit message to less than 64 chars.
				if( message.Length > 63 )
				{
					message = message.TrimWithEllipsis( 60 );
				}

				trayIcon.Text = message;
			}
		}

		private static string FormatMessage( string format, params object[] list )
		{
			string message;
			if( list == null || list.Length == 0 )
			{
				message = format;
			}
			else
			{
				message = string.Format( format, list );
			}

			return message;
		}

		public void ShowBalloonError( string format, params object[] list )
		{
			trayIcon.ShowBalloonTip( 3000, "myHEALTHware", FormatMessage( format, list ), ToolTipIcon.Error );
		}

		public void ShowBalloonWarning( string format, params object[] list )
		{
			trayIcon.ShowBalloonTip( 3000, "myHEALTHware", FormatMessage( format, list ), ToolTipIcon.Warning );
		}

		public void ShowBalloonInfo( string format, params object[] list )
		{
			trayIcon.ShowBalloonTip( 500, "myHEALTHware", FormatMessage( format, list ), ToolTipIcon.Info );
		}

		public void SaveSettings( string message = null )
		{
			userSession.Settings.Save();

			if( !string.IsNullOrWhiteSpace( message ) )
			{
				ShowBalloonInfo( message );
			}
		}

		private void LoadRunAtStartup()
		{
			// Don't fire event.
			runAtSystemStartupCheckBox.CheckedChanged -= RunAtSystemStartupCheckBoxCheckedChanged;

			runAtSystemStartupCheckBox.Checked = IsRunAtSystemStartupSet();

			// Fire events from now on.
			runAtSystemStartupCheckBox.CheckedChanged += RunAtSystemStartupCheckBoxCheckedChanged;
		}

		private bool IsRunAtSystemStartupSet()
		{
			const string KEY_NAME = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
			RegistryKey regKey = null;

			// Open the registry key.
			try
			{
				regKey = Registry.LocalMachine.OpenSubKey( KEY_NAME );

				if( regKey != null )
				{
					object value = regKey.GetValue( MhwPrinter.APP_NAME );

					if( value != null )
					{
						return true;
					}
				}
			}
			catch( Exception ex )
			{
				ShowBalloonError( "Error reading Run at System Startup setting: {0}", ex.Message );
				return false;
			}
			finally
			{
				if( regKey != null )
				{
					regKey.Close();
				}
			}

			return false;
		}

		private void RunAtSystemStartupCheckBoxCheckedChanged( object sender, EventArgs e )
		{
			bool isChecked = runAtSystemStartupCheckBox.Checked;

			if( isChecked )
			{
				InstallRunAtSystemStartup();
				userSession.Settings.RunAtStartup = true;
				SaveSettings( "myHEALTHware for Windows will automatically start with system (recommended)." );
			}
			else
			{
				UninstallRunAtSystemStartup();
				userSession.Settings.RunAtStartup = false;
				SaveSettings( "myHEALTHware for Windows will not automatically start with system (not recommended)." );
			}
		}

		public void InstallRunAtSystemStartup()
		{
			var setupArgs = "-s";

			// Launch setup process with args to install.
			var process = new Process { StartInfo = new ProcessStartInfo { FileName = "Setup.exe", Arguments = setupArgs } };

			process.Start();
			process.WaitForExit();
		}

		public void UninstallRunAtSystemStartup()
		{
			var setupArgs = "-s -u";

			// Launch setup process with args to uninstall.
			var process = new Process { StartInfo = new ProcessStartInfo { FileName = "Setup.exe", Arguments = setupArgs } };

			process.Start();
			process.WaitForExit();
		}

		private void TabsSelectedIndexChanged( object sender, EventArgs e )
		{
			switch( ( (TabControl) sender ).SelectedIndex )
			{
				case 0: // Folder to Drive
					break;
				case 1: // Print to Drive
					break;
				case 2: // Print to Fax
					break;
				case 3: // About
					break;
			}
		}

		private void TabsSelecting( object sender, TabControlCancelEventArgs e )
		{
			// Are we logged in yet?
			if( userSession.IsLoggedIn )
			{
				return;
			}

			MessageBox.Show( "Please log in before proceeding", "Not logged in" );
			e.Cancel = true;
		}

		private async void ButtonLoginClick( object sender, EventArgs e )
		{
			await ChangeLogin();
		}

		private void AccountsComboBoxSelectionChangeCommitted( object sender, EventArgs e )
		{
			ChangeActingAs();
		}

		public void NotifyIfNetworkUnavailable()
		{
			if( !NetworkInterface.GetIsNetworkAvailable() )
			{
				ShowBalloonError( "Please check your network connection." );
			}
		}

		public string UploadFile( string fullPath, string name, string uploadFolderDriveItemId )
		{
			FileStream fileStream;

			// Open the new file as a stream.
			try
			{
				fileStream = OpenFile( fullPath );
			}
			catch( IOException ex )
			{
				ShowBalloonError( ex.Message );
				return null;
			}

			string fileType = Path.GetExtension( name );

			// Show user we're uploading.
			SetTrayIcon( Resources.Uploading, string.Format( "Uploading {0}", name ) );

			string fileId;

			// Call MHW API to upload file.
			try
			{
				MhwAccount selected = GetSelectedAccount();
				ApiFile driveItem = Sdk.Account.UploadFile( selected.AccountId, name, fileType, fileStream, uploadFolderDriveItemId );
				fileId = driveItem.FileId;
			}
			catch( Exception ex )
			{
				ShowBalloonError( ex.Message );
				NotifyIfNetworkUnavailable();
				return null;
			}
			finally
			{
				// Change tray icon back to normal.
				SetTrayIcon();
			}

			return fileId;
		}

		// Sometimes the FileWatcher can try to open the file before the print driver has finished writing it.
		// This method will loop waiting for it to close.
		private FileStream OpenFile( string filepath )
		{
			var retries = 20;
			const int DELAY = 100; // Max time spent here = retries*delay milliseconds

			if( !File.Exists( filepath ) )
			{
				return null;
			}

			do
			{
				try
				{
					// Attempts to open then close the file in RW mode, denying other users to place any locks.
					FileStream fs = File.Open( filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.None );
					return fs;
				}
				catch( IOException )
				{
				}

				retries--;
				Thread.Sleep( DELAY );
			}
			while( retries > 0 );

			ShowBalloonError( "Could not open file {0}", filepath );
			return null;
		}

		private async Task ExecuteLogin( string connectionId, string accessToken )
		{
			try
			{
				userSession.Login( connectionId, accessToken );
			}
			catch( Exception ex )
			{
				ShowBalloonError( "Log in attempt failed: {0}", ex.Message );
				return;
			}

			MhwAccount account = userSession.LoggedInAccount;

			ShowBalloonInfo( string.Format( "Logged in as {0}", account.Name ) );
			labelDisplayName.Text = account.Name;
			SetPictureBox( account.PictureFileId, pictureBoxUser );

			await RefreshOnBehalfOfAccounts();

			accountsComboBox.Enabled = true;

			controlPrintToDrive.Enabled = true;
			controlFolderToDrive.Enabled = true;
			controlPrintToFax.Enabled = true;

			UpdateLoginButtonText();
		}

		private async Task ChangeLogin()
		{
			var loginSuccess = false;
			string connectionId = null;
			string accessToken = null;

			LogOut();

			if( loginForm == null )
			{
				loginForm = new LoginForm( userSession );
				loginForm.Click += ( s, e ) =>
				{
					// Retrieve the resulting connection ID and access token from the OAuth dialog.
					connectionId = loginForm.ConnectionId;
					accessToken = loginForm.AccessToken;

					// Close the login form.
					loginForm.Close();
				};
				loginForm.Closed += ( s, e ) =>
				{
					loginSuccess = loginForm.IsSuccess;

					loginForm.Dispose();
					loginForm = null;
				};

				loginForm.ShowDialog( this );
			}

			if( loginForm != null )
			{
				loginForm.Activate();
			}

			// If cancelled, don't change state.
			if( !loginSuccess )
			{
				return;
			}

			// Log in.
			await ExecuteLogin( connectionId, accessToken );

			if( !userSession.IsLoggedIn )
			{
				LogOut();
				return;
			}

			SetTrayIcon();
		}

		private async Task<IEnumerable<MhwAccount>> RefreshOnBehalfOfAccounts()
		{
			// Load the list of accounts that this account manages files for.
			IEnumerable<MhwAccount> accounts = await userSession.GetMhwAccountsAsync();

			var bindingList = (BindingList<MhwAccount>) accountsBindingSource.DataSource;
			bindingList.Clear();

			foreach( MhwAccount item in accounts )
			{
				bindingList.Add( item );
			}

			MhwAccount account = accounts.FirstOrDefault( p => p.AccountId == userSession.Settings.SelectedAccountId ) ??
			                     accounts.FirstOrDefault( p => p.AccountId == userSession.LoggedInAccount.AccountId );

			accountsComboBox.SelectedItem = account;

			return accounts;
		}

		private void ChangeActingAs()
		{
			MhwAccount selected = GetSelectedAccount();
			userSession.SetActingAsAccount( selected );

			RefreshTabControl();
		}

		private void RefreshTabControl()
		{
			MhwAccount selected = GetSelectedAccount();
			if( selected == null )
			{
				return;
			}

			tabs.SuspendLayout();

			if( selected.IsPersonalAccount && tabs.TabPages.Contains( tabPagePrintToFax ) )
			{
				tabs.TabPages.Remove( tabPagePrintToFax );
			}
			else if( !selected.IsPersonalAccount && !tabs.TabPages.Contains( tabPagePrintToFax ) )
			{
				tabs.TabPages.Insert( faxTabIndex, tabPagePrintToFax );
			}

			tabs.ResumeLayout();
		}

		private MhwAccount GetSelectedAccount()
		{
			return (MhwAccount) accountsComboBox.SelectedItem;
		}

		private static void SetPicture( PictureBox pictureBox, Image profilePic )
		{
			pictureBox.Image = profilePic ?? Resources.DefaultAvatar;
		}

		private async void SetPictureBox( string pictureFileId, PictureBox pictureBox )
		{
			if( !string.IsNullOrEmpty( pictureFileId ) )
			{
				Stream profilePic = await Sdk.Account.GetFileAsync( userSession.LoggedInAccount.AccountId, pictureFileId );
				pictureBox.Image = new Bitmap( profilePic, false );
			}
			else
			{
				// Use default sillouette.
				pictureBox.Image = Resources.DefaultAvatar;
			}
		}

		private void LogOut()
		{
			// Set tray icon to indicate not logged in.
			SetTrayIcon( Resources.LoggedOut, "Please log in" );

			userSession.Logout();

			labelDisplayName.Text = "Not logged in";

			SetPicture( pictureBoxUser, null );
			SetPicture( pictureBoxActingAs, null );

			// Clear delegates list.
			accountsBindingSource.Clear();
			accountsComboBox.Enabled = false;

			// Disable tabs.
			controlPrintToDrive.Enabled = false;
			controlFolderToDrive.Enabled = false;
			controlPrintToFax.Enabled = false;

			UpdateLoginButtonText();
		}

		private void UpdateLoginButtonText()
		{
			buttonLogin.Text = userSession.IsLoggedIn ? "Log Out" : "Log In";
		}
	}
}