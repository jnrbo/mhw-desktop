using System;
using System.Net;
using System.Windows.Forms;
using CefSharp;

namespace myHEALTHwareDesktop
{
	public partial class DrivePicker : Form
	{
		private const string UNAUTHORIZED_ERROR_TEMPLATE =
			"You aren't allowed to print to {0}'s Drive folder. Please contact an administrator to get this corrected.";

		private ChromiumBrowserUserControl chromiumBrowser;
		private readonly ActiveUserSession userSession;
		private readonly string filename;

		public DrivePicker( ActiveUserSession userSession, string filename = null )
		{
			InitializeComponent();

			this.userSession = userSession;
			this.filename = filename;

			InitBrowser();
		}

		private void InitBrowser()
		{
			if( Cef.IsInitialized == false )
			{
				Cef.Initialize( new CefSettings() );
			}

			MhwAccount account = userSession.ActingAsAccount;
			Credentials creds = userSession.Credentials;

			string url =
				string.Format(
					"{0}/UI/Drive/Select?accountId={1}&containersOnly=true&itemName={2}&connection_id={3}&access_token={4}&app_key={5}&app_secret={6}",
					userSession.Settings.myHEALTHwareDomain,
					account.AccountId,
					filename,
					creds.ConnectionId,
					creds.AccessToken,
					creds.AppId,
					creds.AppSecret );

			// Initially hide the control until the response is complete
			chromiumBrowser = new ChromiumBrowserUserControl( url, ResponseHandler );
			chromiumBrowser.PostMessageListener += ResultMessageHandler;
			chromiumBrowser.BrowserVisible += ChromiumBrowserVisible;

			Controls.Add( chromiumBrowser );
		}

		private void ChromiumBrowserVisible( object sender, EventArgs e )
		{
			loadingControl.OnLoadingFinished();
		}

		private void ResponseHandler( int httpStatusCode )
		{
			if( httpStatusCode < 400 )
			{
				// Response wasn't an error
				return;
			}

			string message = httpStatusCode == (int) HttpStatusCode.Forbidden
				? string.Format( UNAUTHORIZED_ERROR_TEMPLATE, userSession.ActingAsAccount.Name )
				: "We were unable to load the Print to Drive folder selector.";

			Dispose();

			var messageDialog = new MhwMessageForm( "Print to Drive", message, true );
			messageDialog.ShowDialog();

			chromiumBrowser.OnResponseHandler = null;
		}

		public void ResultMessageHandler( object sender, PostMessageListenerEventArgs args )
		{
			// Fire event.
			OnClick( args );
		}

		private void DrivePickerShown( object sender, EventArgs e )
		{
			Activate();
		}
	}
}
