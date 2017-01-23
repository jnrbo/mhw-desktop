using System;
using System.Web;
using System.Windows.Forms;
using CefSharp;
using myHEALTHwareDesktop.Properties;

namespace myHEALTHwareDesktop
{
	public partial class LoginForm : Form
	{
		private ChromiumBrowserUserControl chromiumBrowser;
		private readonly string appId;
		private readonly string appSecret;

		public string ConnectionId { get; set; }
		public string AccessToken { get; set; }
		public bool IsSuccess { get; set; }

		public LoginForm( string appId, string appSecret )
		{
			this.appId = appId;
			this.appSecret = appSecret;

			InitializeComponent();

			InitBrowser();
		}

		private void InitBrowser()
		{
			IsSuccess = false;

			if( Cef.IsInitialized == false )
			{
				Cef.Initialize( new CefSettings() );
			}

			string callbackURL = "https://localhost";
			string url = string.Format( "{0}/Login/Authenticate?callback=\"{1}\"&app_key={2}&app_secret={3}",
			                            Settings.Default.myHEALTHwareDomain,
			                            callbackURL,
			                            appId,
			                            appSecret );

			chromiumBrowser = new ChromiumBrowserUserControl( url );
			Controls.Add( chromiumBrowser );
			chromiumBrowser.Dock = DockStyle.Fill;

			chromiumBrowser.Browser.AddressChanged += ( sender, args ) =>
			{
				Uri myUri = new Uri( args.Address );
				
				ConnectionId = HttpUtility.ParseQueryString( myUri.Query ).Get( "connection" );
				if( ConnectionId == null )
				{
					return;
				}

				AccessToken = HttpUtility.ParseQueryString( myUri.Query ).Get( "token" );
				IsSuccess = true;

				this.InvokeOnUiThreadIfRequired( () => OnClick( EventArgs.Empty ) );
			};
		}
	}
}
