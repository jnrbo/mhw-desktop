using System;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace myHEALTHwareDesktop
{
	public partial class ChromiumBrowserUserControl : UserControl
	{
		public event EventHandler<PostMessageListenerEventArgs> PostMessageListener;

		public IWinFormsWebBrowser Browser { get; private set; }

		public object BoundScriptObject { get; set; }

		public ChromiumBrowserUserControl( string url )
		{
			InitializeComponent();

			var browser = new ChromiumWebBrowser( url ) { Dock = DockStyle.Fill };

			SetStyle( ControlStyles.ResizeRedraw, true );

			Dock = DockStyle.Fill;

			browserPanel.Controls.Add( browser );

			Browser = browser;

			browser.TitleChanged += OnBrowserTitleChanged;
			browser.HandleCreated += OnBrowserHandleCreated;
			Disposed += BrowserTabUserControlDisposed;

			//browser.MenuHandler = new MenuHandler();
			//browser.LifeSpanHandler = new LifeSpanHandler();

			browser.LoadingStateChanged += ( sender, args ) =>
			{
				if( args.CanReload )
				{
					string overridePostMessage =
						"window.postMessage = function(data, origin){ postMessageListener.received(JSON.stringify(data), origin); }";
					browser.ExecuteScriptAsync( overridePostMessage );
				}
			};
		}

		private void OnPostMessageReceived( object sender, PostMessageListenerEventArgs e )
		{
			if( PostMessageListener != null )
			{
				PostMessageListener( sender, e );
			}
		}

		private void OnBrowserHandleCreated( object sender, EventArgs e )
		{
			var postMessageListener = new PostMessageListener();
			postMessageListener.PostMessage += OnPostMessageReceived;

			Browser.RegisterJsObject( "postMessageListener", postMessageListener );
			Browser.RegisterJsObject( "bound", BoundScriptObject );
		}

		private void BrowserTabUserControlDisposed( object sender, EventArgs e )
		{
			Disposed -= BrowserTabUserControlDisposed;

			Browser.TitleChanged -= OnBrowserTitleChanged;

			Browser.Dispose();
		}

		private void OnBrowserTitleChanged( object sender, TitleChangedEventArgs args )
		{
			this.InvokeOnUiThreadIfRequired( () => Parent.Text = args.Title );
		}

		public void ExecuteScript( string script )
		{
			Browser.ExecuteScriptAsync( script );
		}

		public object EvaluateScript( string script )
		{
			var task = Browser.EvaluateScriptAsync( script );
			task.Wait();
			return task.Result;
		}

		public void LoadUrl( string url )
		{
			if( Uri.IsWellFormedUriString( url, UriKind.RelativeOrAbsolute ) )
			{
				Browser.Load( url );
			}
		}
	}

	//internal class LifeSpanHandler : ILifeSpanHandler
	//{
	//	public bool OnBeforePopup(IWebBrowser browser, string url, ref int x, ref int y, ref int width, ref int height)
	//	{
	//		if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
	//		{
	//			//Process.Start(url);
	//			browser.Load(url);
	//			return true;
	//		}

	//		return false;
	//	}

	//	public void OnBeforeClose(IWebBrowser browser)
	//	{

	//	}
	//}

	//internal class MenuHandler : IMenuHandler
	//{
	//	public bool OnBeforeContextMenu(IWebBrowser browser)
	//	{
	//		return false;
	//	}
	//}

	public static class ControlExtensions
	{
		/// <summary>
		/// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
		/// </summary>
		/// <param name="control">the control for which the update is required</param>
		/// <param name="action">action to be performed on the control</param>
		public static void InvokeOnUiThreadIfRequired( this Control control, Action action )
		{
			if( control.InvokeRequired )
			{
				control.BeginInvoke( action );
			}
			else
			{
				action.Invoke();
			}
		}
	}

	public class PostMessageListenerEventArgs : EventArgs
	{
		public MhwPostMessage Message { get; set; }
	}

	public class PostMessageListener
	{
		private MhwPostMessage message;
		//private string origin;

		public event EventHandler<PostMessageListenerEventArgs> PostMessage;

		public void Received( string data, string origin )
		{
			try
			{
				var serializer = new JavaScriptSerializer();
				message = serializer.Deserialize<MhwPostMessage>( data );
			}
			catch( Exception )
			{
				message = null;
			}

			OnPostMessageReceived( message );
		}

		private void OnPostMessageReceived( MhwPostMessage message )
		{
			if( PostMessage != null )
			{
				PostMessage( this, new PostMessageListenerEventArgs { Message = message } );
			}
		}
	}

	public class MhwPostMessage
	{
		public string eventType { get; set; }
		public MhwMessagedata data { get; set; }
	}

	public class MhwMessagedata
	{
		public string message { get; set; }
		public string driveItemId { get; set; }
		public string itemName { get; set; }
	}
}