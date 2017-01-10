using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SOAPware.PortalSdk;
using SOAPware.PortalApi.Models;
using MHWVirtualPrinter;

// Credit: Winform Icon tray code initially gen'd from template: http://saveontelephoneservices.com/modules.php?name=News&file=article&sid=8

namespace myHEALTHwareDesktop
{
	public partial class SettingsForm : Form
	{
		public const string appName = "myHEALTHware Desktop";

		// Get our initial Exit logic from our project user properties.
		private bool isTimeToQuit = false;

		// Was our last MouseClick a left or right MouseClick?
		private bool isLastMouseClickLeft = false;

		// Did our last MouseClick come from NotifyIcon?
		//private bool isClickFromNotifyIcon = false;

		public SettingsForm()
		{
			// Run windows generated form code.
			InitializeComponent();

			// Create our "FormClosing" event handler.
			this.FormClosing += MHWCloseRequested;

			// Load persisted settings values.
			LoadRunAtStartup();

			// Try the previous credentials.
			this.connectionId = Properties.Settings.Default.ConnectionId;
			this.accessToken = Properties.Settings.Default.AccessToken;
			this.isLoggedIn = ExecuteLogin();

			if (!this.isLoggedIn)
			{
				this.isLoggedIn = ChangeLogin();
			}

			if (!this.isLoggedIn)
			{
				LoggedOut();
			}
		}

		// **** Our Program Entry Point. ****
		// We added a line Under SettingsForm.Designer.cs to invoke this:
		// this.Load += new System.EventHandler(this.MHWFormInitialize);
		private void MHWFormInitialize(object sender, System.EventArgs e)
		{
			// Make sure that standard and standard double clicks are enabled,
			// if they are not enabled, enable them. This really is OverKill but better to be
			// safe than sorry.
			if (!this.GetStyle(ControlStyles.StandardClick))
				this.SetStyle(ControlStyles.StandardClick, true);

			if (!this.GetStyle(ControlStyles.StandardDoubleClick))
				this.SetStyle(ControlStyles.StandardDoubleClick, true);

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
			this.ContextMenuStrip = new ContextMenuStrip();

			// We can share this same this.contextMenuStrip1; on the fly, later with SettingsForm.
			// See the MHWSingleClick(); and 
			// _MouseDoubleClick(); event handlers for how and when.

			// Any changes to the ContextMenuStrips can be easily done using the Visual Studio
			// designer. this.contextMenuStrip1; is the normal shared
			// contextMenuStrip and contextMenuStrip2; is used as the
			// different shared ContextMenuStrip and menu items can easily be added or deleted
			// using the Visual Studio Designer to both of these ContextMenuStrips. 
			this.trayIcon.ContextMenuStrip = this.contextMenuStrip1;

			// All of our Mouse event handlers for SettingsForm and NotifyIcon are shared
			// to save code and space. It just is not possible to share single and double
			// click Mouse event handlers, otherwise we would have done that as well.

			// SettingsForm event handlers. Also See the SettingsForm Resize event below which needs to be
			// set later. Notice how these Mouse Events share the same event handler with
			// NotifyIcon; below.
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MHWSingleClick);
			this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MHWDoubleClick);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MHWMouseDown);

			// NotifyIcon; event handlers.
			this.trayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MHWSingleClick);
			this.trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MHWDoubleClick);
			this.trayIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MHWMouseDown);

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
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.menuAboutClicked);
			this.quitToolStripMenuItem.Click += new System.EventHandler(this.menuQuitClicked);
			this.settingsToolStripMenuItem.Click += new System.EventHandler(this.menuSettingsClicked);

			// Our Taskbar and NotifyIcon minimize or normal display startup
			// logic for SettingsForm.
			this.MHWMinimizeToTray();

			// We are single threaded and this is the easiest method for us not to create
			// any SettingsForm lag problems. Sleazy or not, it works :P
			System.Windows.Forms.Application.DoEvents();

			// SettingsForm Resize event handler. 
			// NOTE: Must be after our intital display logic
			// otherwise if it was before, it will cause logic problems.
			this.Resize += new System.EventHandler(this.FormResize);

			// We now at this point, are completely event driven.
		}

		// This event handler is fired by changes caused in WindowState by the "-" to
		// minimize on SettingsForm, as well as the display and minimize menu items in the
		// ContextMenuStrips.
		private void FormResize(object sender, System.EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized)
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
			this.trayIcon.Visible = true;

			this.Hide();  // this.Visible = false is the same thing.

			// Remove us from the Taskbar.
			this.ShowInTaskbar = false;

			this.WindowState = FormWindowState.Minimized;

			this.settingsToolStripMenuItem.Enabled = true;

			if (Properties.Settings.Default.IsFirstUse)
			{
				this.trayIcon.ShowBalloonTip
					(2000,  // Show for 2 seconds
					appName,
					"myHEALTHware has minimized here, as an icon in the System Tray."
					+ " Right-click this icon to see menu of actions.",
					ToolTipIcon.Info);

				Properties.Settings.Default.IsFirstUse = false;
				SaveSettings(null);
			}
		}

		// Display the settings form.
		private void MHWDisplaySettings()
		{
			this.Show();  // Equivalent to this.Visible = true.
			this.Activate();
			this.WindowState = FormWindowState.Normal;
			this.ShowInTaskbar = true;
			this.settingsToolStripMenuItem.Enabled = false;

			if (this.isLoggedIn == false)
			{
				this.isLoggedIn = ChangeLogin();
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

		private void DisplayNotifyIconContextMenu(object sender)
		{
			// This is a neat way for displaying a ContextMenuStrip from
			// NotifyIcon MouseClicks other than single right MouseClicks, it uses
			// System Reflection. Positioning is automatic as well this way.

			// Cast the event sender back to a NotifyIcon
			// for the sake of convienience.
			NotifyIcon eventSource = (NotifyIcon)sender;

			// Get the type instance from the NotifyIcon.
			Type niHandle = eventSource.GetType();

			// Invoke the private ShowContextMenu method.
			niHandle.InvokeMember(
					"ShowContextMenu",
					BindingFlags.Instance |
					BindingFlags.NonPublic |
					BindingFlags.InvokeMethod,
					null,
					eventSource,
					null
					);
		}

		// This event handler handles our MouseDown events for both SettingsForm and NotifyIcon.
		// We use this event handler to help us keep track of where the MouseClick came from,
		// SettingsForm or NotifyIcon as well as what Mouse Button was pressed, Left or Right,
		// and was this a double or single MouseDown event, which is only important
		// if we have single right MouseClicks disabled for NotifyIcon but have double right
		// MouseClicks enabled for NotifyIcon to display the ContextMenuStrip.
		private void MHWMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
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
			if (e.Button == MouseButtons.Left)
				this.isLastMouseClickLeft = true;
			else
				this.isLastMouseClickLeft = false;
		}

		// This event handler handles single MouseClicks for both SettingsForm and our NotifyIcon.
		// Note: This event handler fires twice for double MouseClicks.
		private void MHWSingleClick(object sender, MouseEventArgs e)
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
			if ( (sender == this) && (this.isLastMouseClickLeft == false) )
			{
				if (this.ContextMenuStrip.Visible == false)
				{
					// Display the ContextMenuStrip for SettingsForm.
					// Replace the empty ContextMenuStrip for SettingsForm with a real one. See 
					// MHWFormInitialize();
					// for more details on why we need to do this.
					this.ContextMenuStrip = this.contextMenuStrip2;
					this.ContextMenuStrip.Show(Control.MousePosition);

					// This puts an Empty ContextMenuStrip back for SettingsForm after we displayed
					// a real one. So that we do not lose MouseClick events for SettingsForm.
					this.ContextMenuStrip = new ContextMenuStrip();
				}
			}

			// When using NotifyIcon and dealing with MouseClicks,
			// we need to always make sure SettingsForm is in Focus after MouseClicks on NotifyIcon.
			if (this.WindowState == FormWindowState.Normal)
			{
				if ((this.Visible == true) && (!this.Focused))
				{
					this.Activate();
				}
			}
		}

		// This event handler handles both left and right doubleclicks for both Form
		// and NotifyIcon.
		private void MHWDoubleClick(object sender, MouseEventArgs e)
		{
			// Left double-click on notify icon.
			if (sender == this.trayIcon && this.isLastMouseClickLeft)
			{
				menuSettingsClicked(sender, e);
			}
		}

		private void LaunchSite(string url)
		{
			// We could be offline, so we use some try catch logic here and display a
			// message if needed about possibly being offline.
			try
			{
				System.Diagnostics.Process.Start(url);
			}
			catch
			{
				MessageBox.Show("Please check if you are connected to the Internet");
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

		private void menuSettingsClicked(object sender, EventArgs e)
		{
			this.MHWDisplaySettings();
		}

		private void menuAboutClicked(object sender, EventArgs e)
		{
			this.tabs.SelectedTab = tabPageAbout;
			this.MHWDisplaySettings();
		}

		private void menuQuitClicked(object sender, EventArgs e)
		{
			// It is reallly time to quit.
			this.isTimeToQuit = true;

			// Create a Close request.
			this.Close();
		}

		// This event handler gets called when we receive any Close(); request.
		private void MHWCloseRequested(object sender, FormClosingEventArgs e)
		{
			// If we do not honor a Close requests from SettingsForm using the "X" from
			// the top right of the SettingsForm Window do this. But this could be a system
			// shutdown request, or a process stop request, so, if this is NOT also a
			// UserClosing request, close as well, otherwise we could delay things like system
			// shutdown requests and stop requests.
			if ((!this.isTimeToQuit) && (e.CloseReason == CloseReason.UserClosing))
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
				this.trayIcon.Visible = false;
			}
		}

		public void SetTrayIcon(System.Drawing.Icon icon = null, string message = null)
		{
			if (icon == null)
			{
				this.trayIcon.Icon = Properties.Resources.myHEALTHware;
			}
			else
			{
				this.trayIcon.Icon = icon;
			}

			if (String.IsNullOrWhiteSpace(message))
			{
				this.trayIcon.Text = appName;
			}
			else
			{
				// Must limit message to less than 64 chars.
				if (message.Length > 63)
				{
					message = message.Substring(0, 60) + "...";
				}

				this.trayIcon.Text = message;
			}
		}

		private string FormatMessage(string format, params object[] list)
		{
			string message;
			if (list == null || list.Length == 0)
			{
				message = format;
			}
			else
			{
				message = string.Format(format, list);
			}

			return message;
		}

		public void ShowBalloonError(string format, params object[] list)
		{
			this.trayIcon.ShowBalloonTip(3000, "myHEALTHware", FormatMessage(format, list), ToolTipIcon.Error);
		}

		public void ShowBalloonWarning(string format, params object[] list)
		{
			this.trayIcon.ShowBalloonTip(3000, "myHEALTHware", FormatMessage(format, list), ToolTipIcon.Warning);
		}

		public void ShowBalloonInfo(string format, params object[] list)
		{
			this.trayIcon.ShowBalloonTip(500, "myHEALTHware", FormatMessage(format, list), ToolTipIcon.Info);
		}

		public void SaveSettings(string message = null)
		{
			Properties.Settings.Default.Save();

			if (!String.IsNullOrWhiteSpace(message))
			{
				ShowBalloonInfo(message);
			}
		}

		private void LoadRunAtStartup()
		{
			// Don't fire event.
			this.runAtSystemStartupCheckBox.CheckedChanged -= runAtSystemStartupCheckBox_CheckedChanged;

			this.runAtSystemStartupCheckBox.Checked = IsRunAtSystemStartupSet();

			// Fire events from now on.
			this.runAtSystemStartupCheckBox.CheckedChanged += runAtSystemStartupCheckBox_CheckedChanged;
		}

		private bool IsRunAtSystemStartupSet()
		{
			const string keyName = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
			RegistryKey regKey = null;

			// Open the registry key.
			try
			{
				regKey = Registry.LocalMachine.OpenSubKey(keyName);

				if (regKey != null)
				{
					Object value = regKey.GetValue(MHWPrinter.AppName);

					if (value != null)
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				this.ShowBalloonError("Error reading Run at System Startup setting: {0}", ex.Message);
				return false;
			}
			finally
			{
				if (regKey != null)
				{
					regKey.Close();
				}
			}

			return false;
		}

		private void runAtSystemStartupCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			bool isChecked = this.runAtSystemStartupCheckBox.Checked;

			if (isChecked)
			{
				InstallRunAtSystemStartup();
				Properties.Settings.Default.RunAtStartup = true;
				SaveSettings("myHEALTHware for Windows will automatically start with system (recommended).");
			}
			else
			{
				UninstallRunAtSystemStartup();
				Properties.Settings.Default.RunAtStartup = false;
				SaveSettings("myHEALTHware for Windows will not automatically start with system (not recommended).");
			}
		}

		public void InstallRunAtSystemStartup()
		{
			string setupArgs = "-s";

			// Launch setup process with args to install.
			var process = new Process
			{
				StartInfo = new ProcessStartInfo { FileName = "Setup.exe", Arguments = setupArgs }
			};

			process.Start();
			process.WaitForExit();
		}

		public void UninstallRunAtSystemStartup()
		{
			string setupArgs = "-s -u";

			// Launch setup process with args to uninstall.
			var process = new Process
			{
				StartInfo = new ProcessStartInfo { FileName = "Setup.exe", Arguments = setupArgs }
			};

			process.Start();
			process.WaitForExit();
		}

		private void tabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch ((sender as TabControl).SelectedIndex)
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
		
		private void tabs_Selecting(object sender, TabControlCancelEventArgs e)
		{
			// Are we logged in yet?
			if (this.isLoggedIn)
			{
				return;
			}

			MessageBox.Show("Please log in before proceeding", "Not logged in");
			e.Cancel = true;
		}

		private void buttonSwitchUser_Click(object sender, EventArgs e)
		{
			this.isLoggedIn = ChangeLogin();
		}

		private void pictureBoxUser_Click(object sender, EventArgs e)
		{
			this.isLoggedIn = ChangeLogin();
		}

		private void accountsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
		{
			ChangeActingAs();
		}

		private void labelDisplayName_Click(object sender, EventArgs e)
		{
			this.isLoggedIn = ChangeLogin();
		}

		public void CheckNetworkStatus()
		{
			if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
			{
				ShowBalloonError("Please check your network connection.");
			}
		}

		public string UploadFile(string fullPath, string name, string uploadFolderDriveItemId, bool isDeleteFileAfterUpload)
		{
			FileStream fileStream = null;

			// Open the new file as a stream.
			try
			{
				fileStream = this.OpenFile(fullPath);
			}
			catch (IOException ex)
			{
				ShowBalloonError(ex.Message);
				return null;
			}

			string fileType = Path.GetExtension(name);

			// Show user we're uploading.
			SetTrayIcon(Properties.Resources.Uploading, string.Format("Uploading {0}", name));

			string fileId;

			// Call MHW API to upload file.
			try
			{
				ApiFile driveItem = this.sdk.Account.UploadFile(this.selectedMHWAccountId,
								name, fileType, fileStream, uploadFolderDriveItemId);
				fileId = driveItem.FileId;
			}
			catch (Exception ex)
			{
				this.ShowBalloonError(ex.Message);
				CheckNetworkStatus();
				return null;
			}
			finally
			{
				// Change tray icon back to normal.
				SetTrayIcon();

				if (isDeleteFileAfterUpload)
				{
					File.Delete(fullPath);
				}
			}

			return fileId;
		}

		// Sometimes the FileWatcher can try to open the file before the print driver has finished writing it.
		// This method will loop waiting for it to close.
		private FileStream OpenFile(string filepath)
		{
			int retries = 20;
			const int delay = 100; // Max time spent here = retries*delay milliseconds

			if (!File.Exists(filepath))
				return null;

			do
			{
				try
				{
					// Attempts to open then close the file in RW mode, denying other users to place any locks.
					FileStream fs = File.Open(filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
					return fs;
				}
				catch (IOException)
				{ 
				}

				retries--;
				System.Threading.Thread.Sleep(delay);
			} while (retries > 0);

			ShowBalloonError("Could not open file {0}", filepath);
			return null;
		}
	}
}