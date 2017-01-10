using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SOAPware.PortalSdk;
using SOAPware.PortalApi.Models;
using SOAPware.PortalApi.Model.Conversations;
using SOAPware.PortalApi.Model.Drive;
using SOAPware.PortalApi.Model.Individuals;

namespace myHEALTHwareDesktop
{
	partial class SettingsForm
	{
		public MhwSdk sdk;
		private ApiAccount myAccount = null;
		public string selectedMHWAccountId;
		public string connectionId = null;
		public string accessToken = null;
		private bool isLoggedIn = false;
		private LoginForm loginForm;

		public const string appId = "0CCED62C-808D-4D71-A8B2-AEA20C193263";
		public const string appSecret = "5B8CCD2F-03D6-4161-A7F2-C112EF79B1A1";

		private void SaveAccountCredentials(string connecitonId, string accessToken)
		{
			Properties.Settings.Default.AccessToken = accessToken;
			Properties.Settings.Default.ConnectionId = connecitonId;
			SaveSettings(String.Format("Logged in as {0}", this.myAccount.DisplayName));
		}

		private bool ExecuteLogin()
		{
			// Are credentials in valid format?
			if ((String.IsNullOrWhiteSpace(connectionId)) ||
				 (String.IsNullOrWhiteSpace(accessToken)))
			{
				return false;
			}

			// SDK expects "/api" to already be on the end of the domain.
			string domainAPI = String.Format("{0}/api", Properties.Settings.Default.myHEALTHwareDomain);

			// If credentials we have are invalid, change login.
			this.sdk = new MhwSdk(  false,  // SDK logging has an issue. Keep off.
									Properties.Settings.Default.sdkTimeOutSeconds * 1000,
									domainAPI,
									appId, appSecret,
									connectionId, accessToken);

			// Get our account ID that this app is installed on.
			try
			{
				this.myAccount = sdk.Account.Get();
			}
			catch (HttpException ex)
			{
				this.ShowBalloonError("Log in attempt failed: {0}", ex.Message);
				return false;
			}

			// Save credentials.
			SaveAccountCredentials(connectionId, accessToken);

			// Set display name.
			this.labelDisplayName.Text = this.myAccount.DisplayName;

			// Get the account's profile pic.
			SetPictureBox(this.myAccount.PictureFileId, this.pictureBoxUser);

			PopulateOnBehalfOfAccounts();

			this.accountsComboBox.Enabled = true;

			// Load all other settings according to the selected delegate.
			controlFolderToDrive.LoadSettings(this, this.sdk, this.selectedMHWAccountId);
			controlPrintToDrive.LoadSettings(this, this.sdk, this.selectedMHWAccountId);
			controlPrintToFax.LoadSettings(this, this.sdk, this.selectedMHWAccountId);

			this.controlPrintToDrive.Enabled = true;
			this.controlFolderToDrive.Enabled = true;
			this.controlPrintToFax.Enabled = true;

			return true;
		}

		private bool ChangeLogin()
		{
			LoggedOut();

			loginForm = new LoginForm(SettingsForm.appId, SettingsForm.appSecret);
			loginForm.InitBrowser();

			loginForm.Click += new System.EventHandler(this.loginForm_Submitted);
			loginForm.ShowDialog(this);

			// If cancelled, don't change state.
			if (!loginForm.isSuccess)
			{
				return this.isLoggedIn;
			}

			// Log in.
			bool isLoggedIn = ExecuteLogin();

			if (!isLoggedIn)
			{
				LoggedOut();
				return false;
			}
			else
			{
				SetTrayIcon();
			}

			return isLoggedIn;
		}

		// Called when the login dialog Okay button is clicked.
		private void loginForm_Submitted(object sender, EventArgs e)
		{
			// Retrieve the resulting connection ID and access token from the OAuth dialog.
			this.connectionId = loginForm.connectionId;
			this.accessToken = loginForm.accessToken;

			// Close the login form.
			this.BeginInvoke((MethodInvoker)delegate { this.loginForm.Dispose(); });
		}

		private class ComboboxItem
		{
			public string name;
			public string delegateAccountId;
			public string pictureFileId;

			public override string ToString()
			{
				return name;
			}
		}

		private void PopulateOnBehalfOfAccounts()
		{
			this.selectedMHWAccountId = null;

			// Clear list.
			this.accountsComboBox.Items.Clear();

			// Add personal account as first in list.
			var item = new ComboboxItem()
			{
				name = this.myAccount.DisplayName,
				delegateAccountId = this.myAccount.AccountId,
				pictureFileId = this.myAccount.PictureFileId
			};

			this.accountsComboBox.Items.Add(item);

			// Default to selecting peronsal account.
			string selectedAccountName = item.name;

			// Load the list of accounts that this account manages files for.
			List<ApiAccountConnection> connections = sdk.Account.GetConnections(this.myAccount.AccountId, SOAPware.PortalSDK.Constants.Permissions.ManageFiles);

			foreach (ApiAccountConnection connection in connections)
			{
				// Get the connection's contact.
				//var contact = contacts.Find(x => x.AccountId == connection.Yours.AccountId);
				ApiIndividual contact = sdk.Account.GetIndividualForConnectedAccount(this.myAccount.AccountId, connection.Yours.AccountId);

				item = new ComboboxItem()
				{
					name = contact.DisplayName,
					delegateAccountId = connection.Yours.AccountId,
					pictureFileId = contact.PictureId
				};

				// Add to list box.
				this.accountsComboBox.Items.Add(item);

				// Check if this was the previously selected account.
				if (item.delegateAccountId == Properties.Settings.Default.SelectedAccountId)
				{
					selectedAccountName = item.name;
					this.selectedMHWAccountId = Properties.Settings.Default.SelectedAccountId;
				}
			}

			// Set the selected account in the list.
			int index = this.accountsComboBox.FindString(selectedAccountName);
			this.accountsComboBox.SelectedIndex = index;
			item = (ComboboxItem)this.accountsComboBox.SelectedItem;
			SetPictureBox(item.pictureFileId, this.pictureBoxActingAs);
		}

		private void ChangeActingAs()
		{
			ComboboxItem item = (ComboboxItem)this.accountsComboBox.SelectedItem;
			this.selectedMHWAccountId = item.delegateAccountId;
			SetPictureBox(item.pictureFileId, this.pictureBoxActingAs);
			Properties.Settings.Default.SelectedAccountId = this.selectedMHWAccountId;
			SaveSettings(string.Format("Now acting as {0}", item.name));

			// Reload all other settings according to the selected delegate.
			this.controlFolderToDrive.LoadSettings(this, this.sdk, this.selectedMHWAccountId);
			this.controlPrintToDrive.LoadSettings(this, this.sdk, this.selectedMHWAccountId);
			this.controlPrintToFax.LoadSettings(this, this.sdk, this.selectedMHWAccountId);
		}

		private void SetPictureBox(string pictureFileId, PictureBox pictureBox)
		{
			if (string.IsNullOrEmpty(pictureFileId) == false)
			{
				Stream profilePic = sdk.Account.GetFile(this.myAccount.AccountId, pictureFileId);
				pictureBox.Image = new Bitmap(profilePic, false);
			}
			else
			{
				// Use default sillouette.
				pictureBox.Image = Properties.Resources.DefaultAvatar;
			}
		}

		private void LoggedOut()
		{
			// Set tray icon to indicate not logged in.
			SetTrayIcon(Properties.Resources.LoggedOut, "Please log in");

			isLoggedIn = false;
			connectionId = null;
			accessToken = null;

			this.labelDisplayName.Text = "Not logged in";

			SetPictureBox(null, this.pictureBoxUser);
			SetPictureBox(null, this.pictureBoxActingAs);

			// Clear delegates list.
			this.accountsComboBox.Items.Clear();
			this.accountsComboBox.Enabled = false;

			// Disable tabs.
			this.controlPrintToDrive.Enabled = false;
			this.controlFolderToDrive.Enabled = false;
			this.controlPrintToFax.Enabled = false;

			// Stop any running monitors.
			this.controlPrintToDrive.LoggedOut();
			this.controlFolderToDrive.LoggedOut();
			this.controlPrintToFax.LoggedOut();
		}
	}
}
