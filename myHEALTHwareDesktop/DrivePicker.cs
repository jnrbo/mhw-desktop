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
	public partial class DrivePicker : Form
	{
		private ChromiumBrowserUsercontrol chromiumBrowser;
		private string appId;
		private string appSecret;
		private string fileName;

		public DrivePicker(string appId, string appSecret)
		{
			this.appId = appId;
			this.appSecret = appSecret;
			InitializeComponent();
		}

		public void InitBrowser(string connectionId, string accessToken, string accountId)
		{
			if (Cef.IsInitialized == false)
			{
				Cef.Initialize(new CefSettings());
			}

			string url = string.Format("{0}/UI/Drive/Select?accountId={1}&containersOnly=true&connection_id={2}&access_token={3}&app_key={4}&app_secret={5}", 
								Properties.Settings.Default.myHEALTHwareDomain, 
								accountId,
								connectionId, accessToken,
								appId, appSecret);
			chromiumBrowser = new ChromiumBrowserUsercontrol(url);
			this.Controls.Add(chromiumBrowser);
			chromiumBrowser.Dock = DockStyle.Fill;

			chromiumBrowser.PostMessageListener += ResultMessageHandler;

			//browser.Browser.KeyboardHandler = new MhwSendFaxWindowKeyboardHandler(this);
		}

		public void EnableFileName(bool isEnabled, string defaultFileName=null)
		{
			if (isEnabled)
			{
				this.panelFileName.Show();
				this.textBoxFileName.Show();
				this.labelFileName.Show();
				this.textBoxFileName.Text = defaultFileName;
			}
			else
			{
				this.panelFileName.Hide();
				this.textBoxFileName.Hide();
				this.labelFileName.Hide();
			}
		}

		public string GetFileName()
		{
			return fileName;
		}

		public void ResultMessageHandler(object sender, PostMessageListenerEventArgs args)
		{
			// Fire event.
			this.OnClick(args);
		}
	}
}
