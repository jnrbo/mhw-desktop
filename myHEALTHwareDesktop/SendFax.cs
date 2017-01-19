using System.Windows.Forms;
using CefSharp;
using myHEALTHwareDesktop.Properties;

namespace myHEALTHwareDesktop
{
	public partial class SendFax : Form
	{
		private ChromiumBrowserUsercontrol chromiumBrowser;
		private readonly string appId;
		private readonly string appSecret;

		public SendFax( string appId, string appSecret )
		{
			this.appId = appId;
			this.appSecret = appSecret;

			InitializeComponent();
		}

		public void InitBrowser( string connectionId, string accessToken, string accountId, string fileId )
		{
			if( Cef.IsInitialized == false )
			{
				Cef.Initialize( new CefSettings() );
			}

			string url =
				string.Format(
					"{0}/UI/Fax/Send?accountId={1}&fileId={2}&connection_id={3}&access_token={4}&app_key={5}&app_secret={6}",
					Settings.Default.myHEALTHwareDomain,
					accountId,
					fileId,
					connectionId,
					accessToken,
					appId,
					appSecret );

			chromiumBrowser = new ChromiumBrowserUsercontrol( url );
			Controls.Add( chromiumBrowser );
			chromiumBrowser.Dock = DockStyle.Fill;

			chromiumBrowser.PostMessageListener += ResultMessageHandler;

			//browser.Browser.KeyboardHandler = new MhwSendFaxWindowKeyboardHandler(this);
		}

		public void ResultMessageHandler( object sender, PostMessageListenerEventArgs args )
		{
			// Fire event.
			OnClick( args );
		}
	}
}
