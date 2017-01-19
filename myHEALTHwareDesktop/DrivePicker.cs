using System.Windows.Forms;
using CefSharp;
using myHEALTHwareDesktop.Properties;

namespace myHEALTHwareDesktop
{
	public partial class DrivePicker : Form
	{
		private ChromiumBrowserUsercontrol chromiumBrowser;
		private readonly string appId;
		private readonly string appSecret;
		private string fileName;

		public DrivePicker( string appId, string appSecret )
		{
			this.appId = appId;
			this.appSecret = appSecret;
			InitializeComponent();
		}

		public void InitBrowser( string connectionId, string accessToken, string accountId )
		{
			if( Cef.IsInitialized == false )
			{
				Cef.Initialize( new CefSettings() );
			}

			string url =
				string.Format(
					"{0}/UI/Drive/Select?accountId={1}&containersOnly=true&connection_id={2}&access_token={3}&app_key={4}&app_secret={5}",
					Settings.Default.myHEALTHwareDomain,
					accountId,
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

		public void EnableFileName( bool isEnabled, string defaultFileName = null )
		{
			if( isEnabled )
			{
				panelFileName.Show();
				textBoxFileName.Show();
				labelFileName.Show();
				textBoxFileName.Text = defaultFileName;
			}
			else
			{
				panelFileName.Hide();
				textBoxFileName.Hide();
				labelFileName.Hide();
			}
		}

		public string GetFileName()
		{
			return fileName;
		}

		public void ResultMessageHandler( object sender, PostMessageListenerEventArgs args )
		{
			// Fire event.
			OnClick( args );
		}
	}
}
