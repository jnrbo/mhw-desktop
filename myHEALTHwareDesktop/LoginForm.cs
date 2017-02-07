using System;
using System.Web;
using System.Windows.Forms;
using CefSharp;

namespace myHEALTHwareDesktop
{
	public partial class LoginForm : Form
	{
		private ChromiumBrowserUserControl chromiumBrowser;
		private readonly ActiveUserSession userSession;

		public string ConnectionId { get; set; }
		public string AccessToken { get; set; }
		public bool IsSuccess { get; set; }

		public LoginForm( ActiveUserSession userSession )
		{
			InitializeComponent();

			this.userSession = userSession;

			InitBrowser();
		}

		private void InitBrowser()
		{
			IsSuccess = false;

			if( Cef.IsInitialized == false )
			{
				Cef.Initialize( new CefSettings() );
			}

			var callbackURL = "https://localhost";
			string url = string.Format( "{0}/Login/Authenticate?callback=\"{1}\"&app_key={2}&app_secret={3}",
			                            userSession.Settings.myHEALTHwareDomain,
			                            callbackURL,
			                            Credentials.APP_ID,
			                            Credentials.APP_SECRET );

			chromiumBrowser = new ChromiumBrowserUserControl( url );
			chromiumBrowser.BrowserVisible += ChromiumBrowserVisible;
			chromiumBrowser.Browser.AddressChanged += ( sender, args ) =>
			{
				var myUri = new Uri( args.Address );

				ConnectionId = HttpUtility.ParseQueryString( myUri.Query ).Get( "connection" );
				if( ConnectionId == null )
				{
					return;
				}

				AccessToken = HttpUtility.ParseQueryString( myUri.Query ).Get( "token" );
				IsSuccess = true;

				this.InvokeOnUiThreadIfRequired( () => OnClick( EventArgs.Empty ) );
			};

			Controls.Add( chromiumBrowser );
		}

		private void ChromiumBrowserVisible( object sender, EventArgs e )
		{
			loadingControl.OnLoadingFinished();
		}
	}
}
