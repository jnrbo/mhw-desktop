using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using myHEALTHwareDesktop.Properties;
using MHWVirtualPrinter;
using Microsoft.Win32;
using SOAPware.PortalApi.Models;
using SOAPware.PortalSdk;
using SOAPware.PortalSDK;

// Credit: Winform Icon tray code initially gen'd from template: http://saveontelephoneservices.com/modules.php?name=News&file=article&sid=8

namespace myHEALTHwareDesktop
{
	public partial class MhwDesktopForm : Form
	{
		public const string appName = "myHEALTHware Desktop";
		public const string appId = "0CCED62C-808D-4D71-A8B2-AEA20C193263";
		public const string appSecret = "5B8CCD2F-03D6-4161-A7F2-C112EF79B1A1";

		public MhwSdk sdk;
		private ApiAccount myAccount;
		public string connectionId;
		public string accessToken;
		private bool isLoggedIn;
		private LoginForm loginForm;

		private readonly BindingSource accountsBindingSource;

		// Get our initial Exit logic from our project user properties.
		private bool isTimeToQuit;

		// Was our last MouseClick a left or right MouseClick?
		private bool isLastMouseClickLeft;

		// Did our last MouseClick come from NotifyIcon?
		//private bool isClickFromNotifyIcon = false;

		public MhwDesktopForm()
		{
			// Run windows generated form code.
			InitializeComponent();

			// Create our "FormClosing" event handler.
			FormClosing += MHWCloseRequested;

			// Load persisted settings values.
			LoadRunAtStartup();

			accountsBindingSource = new BindingSource();
		}

		// **** Our Program Entry Point. ****
		// We added a line Under SettingsForm.Designer.cs to invoke this:
		// this.Load += new System.EventHandler(this.MHWFormInitialize);
		private async void MHWFormInitialize( object sender, EventArgs e )
		{
			// Make sure that standard and standard double clicks are enabled,
			// if they are not enabled, enable them. This really is OverKill but better to be
			// safe than sorry.
			if( !GetStyle( ControlStyles.StandardClick ) )
				SetStyle( ControlStyles.StandardClick, true );

			if( !GetStyle( ControlStyles.StandardDoubleClick ) )
				SetStyle( ControlStyles.StandardDoubleClick, true );

			// Initialize both ContextMenuStrips for NotifyIcon and SettingsForm.
			// SettingsForm as being Empty, to avoid any problems later with null values.

			// We need to play some games with the SettingsForm assigned ContextMenuStrip. 
			// We need to assign it on the fly, and make it Empty afterwards because we lose
			// SettingsForm MouseClick events if we let a ContextMenuStrip stay assigned. Also, 
			// we can't leave it as a null because we use "if" logic on if it is visible,
			// which still works if it is Empty, but NOT if it is null and not assigned to
			// something, so we need to make it Empty 
			// this.ContextMenuStrip = new ContextMenuStrip(); so that we do not generate a
			// null exception when we query if it is visible or not in 
			// MHWSingleClick(); This is a great example showing
			// what can happen when things are not initialized, and how they can cause you
			// problems later on in your code under the right conditions, like checking if the
			// SettingsForm ContextMenuStrip is visible, this would build fine but create a null
			// exception later.
			ContextMenuStrip = new ContextMenuStrip();

			// We can share this same this.contextMenuStrip1; on the fly, later with SettingsForm.
			// See the MHWSingleClick(); and 
			// _MouseDoubleClick(); event handlers for how and when.

			// Any changes to the ContextMenuStrips can be easily done using the Visual Studio
			// designer. this.contextMenuStrip1; is the normal shared
			// contextMenuStrip and contextMenuStrip2; is used as the
			// different shared ContextMenuStrip and menu items can easily be added or deleted
			// using the Visual Studio Designer to both of these ContextMenuStrips. 
			trayIcon.ContextMenuStrip = contextMenuStrip1;

			// All of our Mouse event handlers for SettingsForm and NotifyIcon are shared
			// to save code and space. It just is not possible to share single and double
			// click Mouse event handlers, otherwise we would have done that as well.

			// SettingsForm event handlers. Also See the SettingsForm Resize event below which needs to be
			// set later. Notice how these Mouse Events share the same event handler with
			// NotifyIcon; below.
			MouseClick += MHWSingleClick;
			MouseDoubleClick += MHWDoubleClick;
			MouseDown += MHWMouseDown;

			// NotifyIcon; event handlers.
			trayIcon.MouseClick += MHWSingleClick;
			trayIcon.MouseDoubleClick += MHWDoubleClick;
			trayIcon.MouseDown += MHWMouseDown;

			// The contextMenuStrip1 item event handlers we need and use. 
			// See SettingsForm.Designer.cs you can delete any of these ContextMenuStrip menu items
			// easliy as well.

			// this.contextMenuStrip1.Opening; is used to trap a single
			// right MouseClick on NotifyIcon; and stop the normal
			// ContextMenuStrip from being displayed if the Project User Settings are set to
			// disable single right MouseClicks on NotifyIcon.
			//this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
			//this.conversationsToolStripMenuItem.Click += new System.EventHandler(this.menuConversationsClicked);
			//this.faxToolStripMenuItem.Click += new System.EventHandler(this.menuFaxClicked);
			aboutToolStripMenuItem.Click += menuAboutClicked;
			quitToolStripMenuItem.Click += menuQuitClicked;
			settingsToolStripMenuItem.Click += menuSettingsClicked;

			// Our Taskbar and NotifyIcon minimize or normal display startup
			// logic for SettingsForm.
			MHWMinimizeToTray();

			// We are single threaded and this is the easiest method for us not to create
			// any SettingsForm lag problems. Sleazy or not, it works :P
			Application.DoEvents();

			// SettingsForm Resize event handler. 
			// NOTE: Must be after our intital display logic
			// otherwise if it was before, it will cause logic problems.
			Resize += FormResize;

			accountsBindingSource.DataSource = new BindingList<MhwAccount>();
			pictureBoxActingAs.DataBindings.Add( "Image", accountsBindingSource, "ProfilePic", true );
			accountsComboBox.DataSource = accountsBindingSource;

			// Try the previous credentials.
			connectionId = Settings.Default.ConnectionId;
			accessToken = Settings.Default.AccessToken;
			isLoggedIn = await ExecuteLogin();

			if( !isLoggedIn )
			{
				await ChangeLogin();
			}

			if( !isLoggedIn )
			{
				LogOut();
			}
			// We now at this point, are completely event driven.
		}

		// This event handler is fired by changes caused in WindowState by the "-" to
		// minimize on SettingsForm, as well as the display and minimize menu items in the
		// ContextMenuStrips.
		private void FormResize( object sender, EventArgs e )
		{
			if( WindowState == FormWindowState.Minimized )
			{
				MHWMinimizeToTray();
			}

			// Not needed since invoked when menu is clicked.
			//else  // Normal or Maximized
			//{
			//	MHWDisplaySettings();
			//}
		}

		// Minimize the settings form to the system tray.
		private void MHWMinimizeToTray()
		{
			trayIcon.Visible = true;

			Hide(); // this.Visible = false is the same thing.

			// Remove us from the Taskbar.
			ShowInTaskbar = false;

			WindowState = FormWindowState.Minimized;

			settingsToolStripMenuItem.Enabled = true;

			if( Settings.Default.IsFirstUse )
			{
				trayIcon.ShowBalloonTip( 2000,
				                         // Show for 2 seconds
				                         appName,
				                         "myHEALTHware has minimized here, as an icon in the System Tray." +
				                         " Right-click this icon to see menu of actions.",
				                         ToolTipIcon.Info );

				Settings.Default.IsFirstUse = false;
				SaveSettings( null );
			}
		}

		// Display the settings form.
		private async Task MHWDisplaySettings()
		{
			Show(); // Equivalent to this.Visible = true.
			Activate();
			WindowState = FormWindowState.Normal;
			ShowInTaskbar = true;
			settingsToolStripMenuItem.Enabled = false;

			if( !isLoggedIn )
			{
				await ChangeLogin();
			}

			if( isLoggedIn )
			{
				await RefreshOnBehalfOfAccounts();
			}
		}

		// This event handler can disable NotifyIcon single right MouseClicks for NotifyIcon
		// here, which means the normal ContextMenuStrip will not display when we right single
		// MouseClick NotifyIcon.

		// This can be also used to disable all MouseClicks on NotifyIcon if all other click
		// logic is false for NotifyIcon, which can be used for a monitoring program
		// that you may want the user to know is running, but not allow them to stop, using
		// the GUI, of course, if they have permission, they can still stop the process
		// using programs like the TaskManager.

		// Double right MouseClick logic remains active even when a single right MouseClick
		// is disabled for both NotifyIcon as well as SettingsForm. So you can still select a
		// right double MouseClick project user setting for NotifyIcon and SettingsForm even when
		// single right MouseClick have been disabled.

		// We don't want to stop ALL displays of the ContextMenuStrip from say a left 
		// MouseClick or left/right double MouseClick, which is a selectable project user
		// setting in this example. We also do not want to limit the ability of SettingsForm to use a
		// single right MouseClick to display the ContextMenuStrip if or when needed.

		// We use some MouseClick information obtained from MHWMouseDown();
		// which is fired for both single and double MouseDown events for NotifyIcon and 
		// is used to help us determine where the MouseClick came from, SettingsForm or the NotifyIcon
		// as well as which mouse button was clicked, left or right, and was it a single
		// or a double MouseClick.
		//private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		//{
		//	 Make sure this was a single or double right MouseClick.
		//	 Was the last MouseClick on NotifyIcon? It could have been on SettingsForm.
		//	 Are single right MouseClicks disabled?
		//	 Are single double right MouseClicks for ContextMenuStrips enabled?
		//	if ((!this.isLastMouseClickLeft)
		//		 && (this.isClickFromNotifyIcon)
		//		 //&& (Properties.Settings.Default.NotifyIconMouseSingleRightClickDisabled)
		//		 && ((!this.isClicksNotifyIconDoubleClicks)
		//			 || ((this.isClicksNotifyIconDoubleClicks)
		//			 //&& (!Properties.Settings.Default.NotifyIconMouseDoubleRightClickShowsContextMenuStrip)
		//			)))
		//	{
		//		// Stop the ContextMenuStrip from being visible.
		//		this.contextMenuStrip1.Visible = false;

		//		// Don't honor this ContextMenuStrip display request.
		//		e.Cancel = true;

		//		// When using NotifyIcon and dealing with MouseClicks
		//		// we need to always make sure SettingsForm is in Focus
		//		// after MouseClicks on NotifyIcon.
		//		if (this.WindowState == FormWindowState.Normal)
		//			this.ActivateAndShow();
		//	}
		//}

		private void DisplayNotifyIconContextMenu( object sender )
		{
			// This is a neat way for displaying a ContextMenuStrip from
			// NotifyIcon MouseClicks other than single right MouseClicks, it uses
			// System Reflection. Positioning is automatic as well this way.

			// Cast the event sender back to a NotifyIcon
			// for the sake of convienience.
			NotifyIcon eventSource = (NotifyIcon) sender;

			// Get the type instance from the NotifyIcon.
			Type niHandle = eventSource.GetType();

			// Invoke the private ShowContextMenu method.
			niHandle.InvokeMember( "ShowContextMenu",
			                       BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
			                       null,
			                       eventSource,
			                       null );
		}

		// This event handler handles our MouseDown events for both SettingsForm and NotifyIcon.
		// We use this event handler to help us keep track of where the MouseClick came from,
		// SettingsForm or NotifyIcon as well as what Mouse Button was pressed, Left or Right,
		// and was this a double or single MouseDown event, which is only important
		// if we have single right MouseClicks disabled for NotifyIcon but have double right
		// MouseClicks enabled for NotifyIcon to display the ContextMenuStrip.
		private void MHWMouseDown( object sender, MouseEventArgs e )
		{
			// If this event was fired from NotifyIcon then do this.
			//if (sender == this.NotifyIcon)
			//{
			//	// Our Last Mouse Clicks came from NotifyIcon.
			//	this.isClickFromNotifyIcon = true;

			//	// Was this a single MouseClick or a double MouseClick for ContextMenuStrip
			//	// display purposes? We come here twice on double MouseClicks once as 
			//	// Clicks = 1 and once as Clicks = 2. 

			//	// The contextMenuStrip1_Opening(); is also called twice
			//	// for double right MouseClicks, but always after this event fires, so the
			//	// second time if Clicks = 2 and double right MouseClicks on NotifyIcon are
			//	// set to display the contextMenuStrip1; we need to set
			//	// this before contextMenuStrip1_Opening(); fires to
			//	// allow double right MouseClicks on NotifyIcon to display the 
			//	// ContextMenuStrip1; if needed. If you wish to use this
			//	// Clicks count for other things, you will need to verify the order in
			//	// which other Mouse events fire, which was also tested in this case.
			//	if (e.Clicks == 2)
			//		this.isClicksNotifyIconDoubleClicks = true;
			//	else
			//		this.isClicksNotifyIconDoubleClicks = false;
			//}
			//else
			//{
			//	// This MouseDown did not come from NotifyIcon.
			//	this.isClickFromNotifyIcon = false;
			//}

			// Was it a left or right MouseDown?
			if( e.Button == MouseButtons.Left )
				isLastMouseClickLeft = true;
			else
				isLastMouseClickLeft = false;
		}

		// This event handler handles single MouseClicks for both SettingsForm and our NotifyIcon.
		// Note: This event handler fires twice for double MouseClicks.
		private void MHWSingleClick( object sender, MouseEventArgs e )
		{
			// Left single click on notify icon.
			//if ( (sender == this) && (this.isLastMouseClickLeft) )
			//{
			//	this.MHWDisplaySettings();
			//}

			// Right single click on notify icon.
			// NOTE: These are automatically handled by the NotifyIcon class.
			//if ( (sender == this.NotifyIcon) &&
			//	 (this.isLastMouseClickLeft == false)
			//{
			//	// Display the ContextMenuStrip for NotifyIcon.
			//	this.NotifyIcon.ContextMenuStrip = this.contextMenuStrip1;
			//	this.DisplayNotifyIconContextMenu(sender);
			//}

			// Right single click on form.
			if( ( sender == this ) && ( isLastMouseClickLeft == false ) )
			{
				if( ContextMenuStrip.Visible == false )
				{
					// Display the ContextMenuStrip for SettingsForm.
					// Replace the empty ContextMenuStrip for SettingsForm with a real one. See 
					// MHWFormInitialize();
					// for more details on why we need to do this.
					ContextMenuStrip = contextMenuStrip2;
					ContextMenuStrip.Show( MousePosition );

					// This puts an Empty ContextMenuStrip back for SettingsForm after we displayed
					// a real one. So that we do not lose MouseClick events for SettingsForm.
					ContextMenuStrip = new ContextMenuStrip();
				}
			}

			// When using NotifyIcon and dealing with MouseClicks,
			// we need to always make sure SettingsForm is in Focus after MouseClicks on NotifyIcon.
			if( WindowState == FormWindowState.Normal )
			{
				if( Visible && ( !Focused ) )
				{
					Activate();
				}
			}
		}

		// This event handler handles both left and right doubleclicks for both Form
		// and NotifyIcon.
		private void MHWDoubleClick( object sender, MouseEventArgs e )
		{
			// Left double-click on notify icon.
			if( sender == trayIcon && isLastMouseClickLeft )
			{
				menuSettingsClicked( sender, e );
			}
		}

		private void LaunchSite( string url )
		{
			// We could be offline, so we use some try catch logic here and display a
			// message if needed about possibly being offline.
			try
			{
				Process.Start( url );
			}
			catch
			{
				MessageBox.Show( "Please check if you are connected to the Internet" );
			}
		}

		//private void menuConversationsClicked(object sender, EventArgs e)
		//{
		//	LaunchSite("https://myhealthware.com/#/Conversations");
		//}

		//private void menuContactsClicked(object sender, EventArgs e)
		//{
		//	LaunchSite("https://myhealthware.com/#/Contacts");
		//}

		//private void menuFaxClicked(object sender, EventArgs e)
		//{
		//	LaunchSite("https://myhealthware.com/#/Fax");
		//}

		private async void menuSettingsClicked( object sender, EventArgs e )
		{
			await MHWDisplaySettings();
		}

		private async void menuAboutClicked( object sender, EventArgs e )
		{
			tabs.SelectedTab = tabPageAbout;
			await MHWDisplaySettings();
		}

		private void menuQuitClicked( object sender, EventArgs e )
		{
			// It is reallly time to quit.
			isTimeToQuit = true;

			// Create a Close request.
			Close();
		}

		// This event handler gets called when we receive any Close(); request.
		private void MHWCloseRequested( object sender, FormClosingEventArgs e )
		{
			// If we do not honor a Close requests from SettingsForm using the "X" from
			// the top right of the SettingsForm Window do this. But this could be a system
			// shutdown request, or a process stop request, so, if this is NOT also a
			// UserClosing request, close as well, otherwise we could delay things like system
			// shutdown requests and stop requests.
			if( ( !isTimeToQuit ) && ( e.CloseReason == CloseReason.UserClosing ) )
			{
				// Override app "X" click and just minimize without app close.
				MHWMinimizeToTray();
				e.Cancel = true;
			}
			else
			{
				// We are going to honor this Close request.
				// Turn off our NotifyIcon before we go away. Some programs do not
				// do this. You can tell this is the case, when a program is gone
				// yet the NotifyIcon for that program still remains in the System Tray
				// until you hover your mouse over it and then it goes away. This will
				// make sure our NotifyIcon goes away when our program goes away.
				trayIcon.Visible = false;
			}
		}

		public void SetTrayIcon( Icon icon = null, string message = null )
		{
			trayIcon.Icon = icon ?? Resources.myHEALTHware;

			if( string.IsNullOrWhiteSpace( message ) )
			{
				trayIcon.Text = appName;
			}
			else
			{
				// Must limit message to less than 64 chars.
				if( message.Length > 63 )
				{
					message = message.Substring( 0, 60 ) + "...";
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
			Settings.Default.Save();

			if( !string.IsNullOrWhiteSpace( message ) )
			{
				ShowBalloonInfo( message );
			}
		}

		private void LoadRunAtStartup()
		{
			// Don't fire event.
			runAtSystemStartupCheckBox.CheckedChanged -= runAtSystemStartupCheckBox_CheckedChanged;

			runAtSystemStartupCheckBox.Checked = IsRunAtSystemStartupSet();

			// Fire events from now on.
			runAtSystemStartupCheckBox.CheckedChanged += runAtSystemStartupCheckBox_CheckedChanged;
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
					object value = regKey.GetValue( MHWPrinter.AppName );

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

		private void runAtSystemStartupCheckBox_CheckedChanged( object sender, EventArgs e )
		{
			bool isChecked = runAtSystemStartupCheckBox.Checked;

			if( isChecked )
			{
				InstallRunAtSystemStartup();
				Settings.Default.RunAtStartup = true;
				SaveSettings( "myHEALTHware for Windows will automatically start with system (recommended)." );
			}
			else
			{
				UninstallRunAtSystemStartup();
				Settings.Default.RunAtStartup = false;
				SaveSettings( "myHEALTHware for Windows will not automatically start with system (not recommended)." );
			}
		}

		public void InstallRunAtSystemStartup()
		{
			string setupArgs = "-s";

			// Launch setup process with args to install.
			var process = new Process { StartInfo = new ProcessStartInfo { FileName = "Setup.exe", Arguments = setupArgs } };

			process.Start();
			process.WaitForExit();
		}

		public void UninstallRunAtSystemStartup()
		{
			string setupArgs = "-s -u";

			// Launch setup process with args to uninstall.
			var process = new Process { StartInfo = new ProcessStartInfo { FileName = "Setup.exe", Arguments = setupArgs } };

			process.Start();
			process.WaitForExit();
		}

		private void tabs_SelectedIndexChanged( object sender, EventArgs e )
		{
			switch( ( sender as TabControl ).SelectedIndex )
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

		private void tabs_Selecting( object sender, TabControlCancelEventArgs e )
		{
			// Are we logged in yet?
			if( isLoggedIn )
			{
				return;
			}

			MessageBox.Show( "Please log in before proceeding", "Not logged in" );
			e.Cancel = true;
		}

		private async void buttonSwitchUser_Click( object sender, EventArgs e )
		{
			await ChangeLogin();
		}

		private async void pictureBoxUser_Click( object sender, EventArgs e )
		{
			await ChangeLogin();
		}

		private void accountsComboBox_SelectionChangeCommitted( object sender, EventArgs e )
		{
			ChangeActingAs();
		}

		private async void labelDisplayName_Click( object sender, EventArgs e )
		{
			await ChangeLogin();
		}

		public void CheckNetworkStatus()
		{
			if( !NetworkInterface.GetIsNetworkAvailable() )
			{
				ShowBalloonError( "Please check your network connection." );
			}
		}

		public string UploadFile( string fullPath, string name, string uploadFolderDriveItemId, bool isDeleteFileAfterUpload )
		{
			FileStream fileStream = null;

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
				var selected = GetSelectedAccount();
				ApiFile driveItem = sdk.Account.UploadFile( selected.AccountId, name, fileType, fileStream, uploadFolderDriveItemId );
				fileId = driveItem.FileId;
			}
			catch( Exception ex )
			{
				ShowBalloonError( ex.Message );
				CheckNetworkStatus();
				return null;
			}
			finally
			{
				// Change tray icon back to normal.
				SetTrayIcon();

				if( isDeleteFileAfterUpload )
				{
					File.Delete( fullPath );
				}
			}

			return fileId;
		}

		// Sometimes the FileWatcher can try to open the file before the print driver has finished writing it.
		// This method will loop waiting for it to close.
		private FileStream OpenFile( string filepath )
		{
			int retries = 20;
			const int delay = 100; // Max time spent here = retries*delay milliseconds

			if( !File.Exists( filepath ) )
				return null;

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
				Thread.Sleep( delay );
			}
			while( retries > 0 );

			ShowBalloonError( "Could not open file {0}", filepath );
			return null;
		}

		//===============================================================================

		private void SaveAccountCredentials( string connectionId, string accessToken )
		{
			Settings.Default.AccessToken = accessToken;
			Settings.Default.ConnectionId = connectionId;
			SaveSettings( string.Format( "Logged in as {0}", myAccount.DisplayName ) );
		}

		private async Task<bool> ExecuteLogin()
		{
			// Are credentials in valid format?
			if( ( string.IsNullOrWhiteSpace( connectionId ) ) || ( string.IsNullOrWhiteSpace( accessToken ) ) )
			{
				return false;
			}

			// SDK expects "/api" to already be on the end of the domain.
			string domainApi = string.Format( "{0}/api", Settings.Default.myHEALTHwareDomain );

			// If credentials we have are invalid, change login.
			sdk = new MhwSdk( false,
			                  // SDK logging has an issue. Keep off.
			                  Settings.Default.sdkTimeOutSeconds * 1000,
			                  domainApi,
			                  appId,
			                  appSecret,
			                  connectionId,
			                  accessToken );

			// Get our account ID that this app is installed on.
			try
			{
				myAccount = sdk.Account.Get();
			}
			catch( HttpException ex )
			{
				ShowBalloonError( "Log in attempt failed: {0}", ex.Message );
				return false;
			}

			// Save credentials.
			SaveAccountCredentials( connectionId, accessToken );

			// Set display name.
			labelDisplayName.Text = myAccount.DisplayName;

			// Get the account's profile pic.
			SetPictureBox( myAccount.PictureFileId, pictureBoxUser );

			await RefreshOnBehalfOfAccounts();

			accountsComboBox.Enabled = true;

			var selected = GetSelectedAccount();

			// Load all other settings according to the selected delegate.
			controlFolderToDrive.LoadSettings( this, sdk, selected.AccountId );
			controlPrintToDrive.LoadSettings( this, sdk, selected.AccountId );
			controlPrintToFax.LoadSettings( this, sdk, selected.AccountId );

			controlPrintToDrive.Enabled = true;
			controlFolderToDrive.Enabled = true;
			controlPrintToFax.Enabled = true;

			return true;
		}

		private async Task ChangeLogin()
		{
			LogOut();

			loginForm = new LoginForm( appId, appSecret );
			loginForm.InitBrowser();

			loginForm.Click += loginForm_Submitted;
			loginForm.ShowDialog( this );

			// If cancelled, don't change state.
			if( !loginForm.isSuccess )
			{
				return;
			}

			// Log in.
			isLoggedIn = await ExecuteLogin();

			if( !isLoggedIn )
			{
				LogOut();
				return;
			}

			SetTrayIcon();
		}

		// Called when the login dialog Okay button is clicked.
		private void loginForm_Submitted( object sender, EventArgs e )
		{
			// Retrieve the resulting connection ID and access token from the OAuth dialog.
			connectionId = loginForm.connectionId;
			accessToken = loginForm.accessToken;

			// Close the login form.
			BeginInvoke( (MethodInvoker) delegate { loginForm.Dispose(); } );
		}

		private async Task<IEnumerable<MhwAccount>> RefreshOnBehalfOfAccounts()
		{
			////// Clear list.
			////accountsComboBox.Items.Clear();

			////// Default to selecting personal account.
			////string selectedAccountName = myAccount.DisplayName;

			// Load the list of accounts that this account manages files for.

			var accounts = await GetMhwAccountsAsync();
			var bindingList = (BindingList<MhwAccount>) accountsBindingSource.DataSource;
			bindingList.Clear();

			foreach( var item in accounts )
			{
				bindingList.Add( item );
			}

			var account = accounts.FirstOrDefault( p => p.AccountId == Settings.Default.SelectedAccountId ) ??
			              accounts.FirstOrDefault( p => p.AccountId == myAccount.AccountId );

			accountsComboBox.SelectedItem = account;

			return accounts;

			////foreach( var item in accounts )
			////{
			////	// Add to list box.
			////	accountsComboBox.Items.Add( item );

			////	// Check if this was the previously selected account.
			////	if( item.DelegateAccountId == Settings.Default.SelectedAccountId )
			////	{
			////		selectedAccountName = item.Name;
			////	}
			////}

			////// Set the selected account in the list.
			////int index = accountsComboBox.FindString( selectedAccountName );
			////accountsComboBox.SelectedIndex = index;

			////var selected = (MhwAccount) accountsComboBox.SelectedItem;
			////SetPicture( pictureBoxActingAs, selected.ProfilePic );
		}

		private async Task<IEnumerable<MhwAccount>> GetMhwAccountsAsync()
		{
			var connections = await sdk.Account.GetConnectionsAsync( myAccount.AccountId, Constants.Permissions.ManageFiles );

			var accounts = new List<MhwAccount>
			{
				new MhwAccount
				{
					Name = myAccount.DisplayName,
					AccountId = myAccount.AccountId,
					PictureFileId = myAccount.PictureFileId
				}
			};

			accounts.AddRange(
				connections.Select(
					c =>
						new MhwAccount
						{
							Name = c.Yours.DisplayName,
							AccountId = c.Yours.AccountId,
							PictureFileId = c.Yours.PictureFileId
						} ).OrderBy( p => p.Name ) );

			Parallel.ForEach( accounts,
			                  p =>
			                  {
				                  if( p.PictureFileId != null )
				                  {
					                  var picStream = sdk.Account.GetFile( p.AccountId, p.PictureFileId );
					                  p.ProfilePic = new Bitmap( picStream, false );
				                  }
				                  else
				                  {
					                  p.ProfilePic = Resources.DefaultAvatar;
				                  }
			                  } );

			return accounts;
		}

		private void ChangeActingAs()
		{
			MhwAccount selected = GetSelectedAccount();

			////SetPicture( pictureBoxActingAs, selected.ProfilePic );
			////SetPictureBox( item.PictureFileId, pictureBoxActingAs );

			Settings.Default.SelectedAccountId = selected.AccountId;
			SaveSettings( /*string.Format( "Now acting as {0}", item.Name )*/ );

			// Reload all other settings according to the selected delegate.
			controlFolderToDrive.LoadSettings( this, sdk, selected.AccountId );
			controlPrintToDrive.LoadSettings( this, sdk, selected.AccountId );
			controlPrintToFax.LoadSettings( this, sdk, selected.AccountId );
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
				Stream profilePic = await sdk.Account.GetFileAsync( myAccount.AccountId, pictureFileId );
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

			isLoggedIn = false;
			connectionId = null;
			accessToken = null;

			labelDisplayName.Text = "Not logged in";

			SetPicture( pictureBoxUser, null );
			SetPicture( pictureBoxActingAs, null );

			////SetPictureBox( null, pictureBoxUser );
			////SetPictureBox( null, pictureBoxActingAs );

			// Clear delegates list.
			////accountsComboBox.Items.Clear();
			accountsComboBox.Enabled = false;

			// Disable tabs.
			controlPrintToDrive.Enabled = false;
			controlFolderToDrive.Enabled = false;
			controlPrintToFax.Enabled = false;

			// Stop any running monitors.
			controlPrintToDrive.LogOut();
			controlFolderToDrive.LogOut();
			controlPrintToFax.LogOut();
		}
	}
}