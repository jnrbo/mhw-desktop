using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;


namespace myHEALTHwareDesktop
{
	public partial class SendFax : Form
	{
		private ChromiumBrowserUsercontrol chromiumBrowser;
		private string appId;
		private string appSecret;

		public SendFax(string appId, string appSecret)
		{
			this.appId = appId;
			this.appSecret = appSecret;
			InitializeComponent();
		}

		public void InitBrowser(string connectionId, string accessToken, string accountId, string fileId)
		{
			if (Cef.IsInitialized == false)
			{
				Cef.Initialize(new CefSettings());
			}

			string url = string.Format("{0}/UI/Fax/Send?accountId={1}&fileId={2}&connection_id={3}&access_token={4}&app_key={5}&app_secret={6}",
									Properties.Settings.Default.myHEALTHwareDomain, 
									accountId, fileId,
									connectionId, accessToken,
									appId, appSecret);

			chromiumBrowser = new ChromiumBrowserUsercontrol(url);
			this.Controls.Add(chromiumBrowser);
			chromiumBrowser.Dock = DockStyle.Fill;

			chromiumBrowser.PostMessageListener += ResultMessageHandler;

			//browser.Browser.KeyboardHandler = new MhwSendFaxWindowKeyboardHandler(this);
		}

		public void ResultMessageHandler(object sender, PostMessageListenerEventArgs args)
		{
			// Fire event.
			this.OnClick(args);
		}
	}
}
