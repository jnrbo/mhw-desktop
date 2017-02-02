using System;
using System.Windows.Forms;
using CefSharp;
using myHEALTHwareDesktop.Properties;

namespace myHEALTHwareDesktop
{
	public partial class DrivePicker : Form
	{
		private ChromiumBrowserUserControl chromiumBrowser;
		private readonly string appId;
		private readonly string appSecret;
		private string fileName;

		public DrivePicker( string appId, string appSecret )
		{
			this.appId = appId;
			this.appSecret = appSecret;
			InitializeComponent();
		}

		public void InitBrowser( string connectionId, string accessToken, string accountId, string itemName = null )
		{
			if( Cef.IsInitialized == false )
			{
				Cef.Initialize( new CefSettings() );
			}

			string url =
				string.Format(
					"{0}/UI/Drive/Select?accountId={1}&containersOnly=true&itemName={2}&connection_id={3}&access_token={4}&app_key={5}&app_secret={6}",
					Settings.Default.myHEALTHwareDomain,
					accountId,
					itemName,
					connectionId,
					accessToken,
					appId,
					appSecret );

			chromiumBrowser = new ChromiumBrowserUserControl( url );
			Controls.Add( chromiumBrowser );
			chromiumBrowser.Dock = DockStyle.Fill;

			chromiumBrowser.PostMessageListener += ResultMessageHandler;

			//browser.Browser.KeyboardHandler = new MhwSendFaxWindowKeyboardHandler(this);
		}

		public void EnableFileName( bool isEnabled, string defaultFileName = null )
		{
			////if( isEnabled )
			////{
			////	panelFileName.Show();
			////	textBoxFileName.Show();
			////	labelFileName.Show();
			////	textBoxFileName.Text = defaultFileName;
			////}
			////else
			////{

			panelFileName.Hide();
			textBoxFileName.Hide();
			labelFileName.Hide();

			////}
		}

		public string GetFileName()
		{
			return null;

			////return fileName;
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
