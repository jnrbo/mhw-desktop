using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace myHEALTHwareDesktop
{
	public partial class LoginForm : Form
	{
		private ChromiumBrowserUsercontrol chromiumBrowser;
		private string appId;
		private string appSecret;
		public string connectionId;
		public string accessToken;
		public bool isSuccess;

		public LoginForm(string appId, string appSecret)
		{
			this.appId = appId;
			this.appSecret = appSecret;
			InitializeComponent();
		}

		public void InitBrowser()
		{
			isSuccess = false;

			if (Cef.IsInitialized == false)
			{
				Cef.Initialize(new CefSettings());
			}

			string callbackURL = "https://localhost";
			string url = string.Format("{0}/Login/Authenticate?callback=\"{1}\"&app_key={2}&app_secret={3}",
									Properties.Settings.Default.myHEALTHwareDomain,
									callbackURL, appId, appSecret);

			chromiumBrowser = new ChromiumBrowserUsercontrol(url);
			this.Controls.Add(chromiumBrowser);
			chromiumBrowser.Dock = DockStyle.Fill;

			chromiumBrowser.Browser.AddressChanged += (sender, args) =>
			{
				Uri myUri = new Uri(args.Address);
				connectionId = HttpUtility.ParseQueryString(myUri.Query).Get("connection");

				if (connectionId != null)
				{
					accessToken = HttpUtility.ParseQueryString(myUri.Query).Get("token");
					isSuccess = true;
					this.OnClick(null);
				}
			};
		}
	}
}
