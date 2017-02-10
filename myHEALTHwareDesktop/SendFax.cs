using System;
using System.Net;
using System.Windows.Forms;
using CefSharp;

namespace myHEALTHwareDesktop
{
	public partial class SendFax : Form
	{
		private const string UNAUTHORIZED_ERROR_TEMPLATE =
			"You aren't allowed to send a fax using {0}'s account. Please contact an administrator to get this corrected.";

		private ChromiumBrowserUserControl chromiumBrowser;
		private readonly ActiveUserSession userSession;
		private readonly string fileId;

		public SendFax( ActiveUserSession userSession, string fileId )
		{
			InitializeComponent();

			this.userSession = userSession;
			this.fileId = fileId;

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
					"{0}/UI/Fax/Send?accountId={1}&fileId={2}&connection_id={3}&access_token={4}&app_key={5}&app_secret={6}&responsive={7}",
					userSession.Settings.myHEALTHwareDomain,
					account.AccountId,
					fileId,
					creds.ConnectionId,
					creds.AccessToken,
					creds.AppId,
					creds.AppSecret,
					true );

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
				: "We were unable to load the Print to Fax window.";

			Dispose();

			var messageDialog = new MhwMessageForm( "Print to Fax", message, true );
			messageDialog.ShowDialog();

			chromiumBrowser.OnResponseHandler = null;
		}

		public void ResultMessageHandler( object sender, PostMessageListenerEventArgs args )
		{
			// Fire event.
			OnClick( args );
		}

		private void SendFaxShown( object sender, EventArgs e )
		{
			Activate();
		}
	}
}
