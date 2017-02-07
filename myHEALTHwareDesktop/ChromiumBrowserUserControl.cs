using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace myHEALTHwareDesktop
{
	public partial class ChromiumBrowserUserControl : UserControl
	{
		public event EventHandler<PostMessageListenerEventArgs> PostMessageListener;
		public event EventHandler BrowserVisible;

		public IWinFormsWebBrowser Browser { get; private set; }

		public object BoundScriptObject { get; set; }

		public Action<int> OnResponseHandler { get; set; }

		public ChromiumBrowserUserControl( string url, Action<int> responseHandler = null )
		{
			InitializeComponent();

			SetStyle( ControlStyles.ResizeRedraw, true );
			Dock = DockStyle.Fill;

			OnResponseHandler = responseHandler;

			var browser = new ChromiumWebBrowser( url ) { Dock = DockStyle.Fill };
			browser.TitleChanged += OnBrowserTitleChanged;
			browser.HandleCreated += OnBrowserHandleCreated;
			browser.LoadingStateChanged += BrowserLoadingStateChanged;
			browser.RequestHandler = new RequestHandler( () => OnResponseHandler, SynchronizationContext.Current );

			Disposed += BrowserTabUserControlDisposed;

			Browser = browser;
			browserPanel.Controls.Add( browser );

			HideBrowserControl();
		}

		private void BrowserLoadingStateChanged( object sender, LoadingStateChangedEventArgs e )
		{
			if( !e.Browser.IsLoading )
			{
				this.InvokeOnUiThreadIfRequired( ShowBrowserControl );
			}
			if( !e.CanReload )
			{
				return;
			}

			var overridePostMessage =
				"window.postMessage = function(data, origin){ postMessageListener.received(JSON.stringify(data), origin); }";
			Browser.ExecuteScriptAsync( overridePostMessage );
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
			Task<JavascriptResponse> task = Browser.EvaluateScriptAsync( script );
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

		public void HideBrowserControl()
		{
			Size = Size.Empty;
			Dock = DockStyle.None;
		}

		public void ShowBrowserControl()
		{
			BringToFront();
			Dock = DockStyle.Fill;

			OnBrowserVisible();
		}

		protected virtual void OnPostMessageReceived( object sender, PostMessageListenerEventArgs e )
		{
			EventHandler<PostMessageListenerEventArgs> handler = PostMessageListener;
			if( handler != null )
			{
				handler( sender, e );
			}
		}

		protected virtual void OnBrowserVisible()
		{
			EventHandler handler = BrowserVisible;
			if( handler != null )
			{
				handler( this, EventArgs.Empty );
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

	internal class RequestHandler : IRequestHandler
	{
		private readonly Func<Action<int>> responseHandlerProvider;
		private readonly SynchronizationContext synchronizationContext;

		public RequestHandler( Func<Action<int>> responseHandlerProvider, SynchronizationContext synchronizationContext )
		{
			if( responseHandlerProvider == null )
			{
				throw new ArgumentNullException( "responseHandlerProvider" );
			}
			Contract.EndContractBlock();

			this.responseHandlerProvider = responseHandlerProvider;
			this.synchronizationContext = synchronizationContext;
		}

		public bool OnResourceResponse( IWebBrowser browserControl,
		                                IBrowser browser,
		                                IFrame frame,
		                                IRequest request,
		                                IResponse response )
		{
			Action<int> responseHandler = responseHandlerProvider() ?? delegate { };
			synchronizationContext.Send( _ => responseHandler( response.StatusCode ), null );

			return false;
		}

		public bool OnBeforeBrowse( IWebBrowser browserControl,
		                            IBrowser browser,
		                            IFrame frame,
		                            IRequest request,
		                            bool isRedirect )
		{
			return false;
		}

		public bool OnOpenUrlFromTab( IWebBrowser browserControl,
		                              IBrowser browser,
		                              IFrame frame,
		                              string targetUrl,
		                              WindowOpenDisposition targetDisposition,
		                              bool userGesture )
		{
			return false;
		}

		public bool OnCertificateError( IWebBrowser browserControl,
		                                IBrowser browser,
		                                CefErrorCode errorCode,
		                                string requestUrl,
		                                ISslInfo sslInfo,
		                                IRequestCallback callback )
		{
			callback.Dispose();
			return false;
		}

		public void OnPluginCrashed( IWebBrowser browserControl, IBrowser browser, string pluginPath )
		{
			// NOTE: Not implemented
		}

		public CefReturnValue OnBeforeResourceLoad( IWebBrowser browserControl,
		                                            IBrowser browser,
		                                            IFrame frame,
		                                            IRequest request,
		                                            IRequestCallback callback )
		{
			callback.Dispose();
			return CefReturnValue.Continue;
		}

		public bool GetAuthCredentials( IWebBrowser browserControl,
		                                IBrowser browser,
		                                IFrame frame,
		                                bool isProxy,
		                                string host,
		                                int port,
		                                string realm,
		                                string scheme,
		                                IAuthCallback callback )
		{
			callback.Dispose();
			return false;
		}

		public void OnRenderProcessTerminated( IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status )
		{
			// NOTE: Not implemented
		}

		public bool OnQuotaRequest( IWebBrowser browserControl,
		                            IBrowser browser,
		                            string originUrl,
		                            long newSize,
		                            IRequestCallback callback )
		{
			callback.Dispose();
			return false;
		}

		public void OnResourceRedirect( IWebBrowser browserControl,
		                                IBrowser browser,
		                                IFrame frame,
		                                IRequest request,
		                                ref string newUrl )
		{
			// NOTE: Not implemented
		}

		public bool OnProtocolExecution( IWebBrowser browserControl, IBrowser browser, string url )
		{
			return false;
		}

		public void OnRenderViewReady( IWebBrowser browserControl, IBrowser browser )
		{
			// NOTE: Not implemented
		}

		public IResponseFilter GetResourceResponseFilter( IWebBrowser browserControl,
		                                                  IBrowser browser,
		                                                  IFrame frame,
		                                                  IRequest request,
		                                                  IResponse response )
		{
			return null;
		}

		public void OnResourceLoadComplete( IWebBrowser browserControl,
		                                    IBrowser browser,
		                                    IFrame frame,
		                                    IRequest request,
		                                    IResponse response,
		                                    UrlRequestStatus status,
		                                    long receivedContentLength )
		{
			// NOTE: Not implemented
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