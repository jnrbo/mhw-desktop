using System;
using System.Windows.Forms;
using CefSharp;

namespace myHEALTHwareDesktop
{
	public partial class SendFax : Form
	{
		private ChromiumBrowserUserControl chromiumBrowser;
		private readonly ActiveUserSession userSession;
		private readonly string fileId;

		public SendFax( ActiveUserSession userSession, string fileId )
		{
			InitializeComponent();

			this.userSession = userSession;
			this.fileId = fileId;
		}

		public void InitBrowser()
		{
			if( Cef.IsInitialized == false )
			{
				Cef.Initialize( new CefSettings() );
			}

			MhwAccount account = userSession.ActingAsAccount;
			Credentials creds = userSession.Credentials;

			string url =
				string.Format(
					"{0}/UI/Fax/Send?accountId={1}&fileId={2}&connection_id={3}&access_token={4}&app_key={5}&app_secret={6}",
					userSession.Settings.myHEALTHwareDomain,
					account.AccountId,
					fileId,
					creds.ConnectionId,
					creds.AccessToken,
					creds.AppId,
					creds.AppSecret );

			chromiumBrowser = new ChromiumBrowserUserControl( url );
			chromiumBrowser.PostMessageListener += ResultMessageHandler;
			chromiumBrowser.BrowserVisible += ChromiumBrowserVisible;

			Controls.Add( chromiumBrowser );
		}

		private void ChromiumBrowserVisible( object sender, EventArgs e )
		{
			loadingControl.OnLoadingFinished();
		}

		public void ResultMessageHandler( object sender, PostMessageListenerEventArgs args )
		{
			// Fire event.
			OnClick( args );
		}
	}
}
