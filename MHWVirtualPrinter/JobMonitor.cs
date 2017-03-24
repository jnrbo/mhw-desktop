using System;
using System.IO;
using System.IO.Pipes;
using System.Reactive.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace MHWVirtualPrinter
{
	public abstract class JobMonitor
	{
		private const string MHW_FILENAME = "/MhwFilename";

		private IObservable<MhwFile> incomingJobs;
		private string PipeName { get; set; }
		private IDisposable Subscription { get; set; }

		protected JobMonitor( MhwPrinter mhwPrinter )
		{
			PipeName = mhwPrinter.GetUserPipe();
		}

		public void Start( Action<MhwFile> onNextItem /*, Action<Exception> onError */,
		                   SynchronizationContext synchronizationContext = null )
		{
			Stop();

			incomingJobs = Observable.Create<MhwFile>( p => Subscribe( p ) );
			if( synchronizationContext != null )
			{
				incomingJobs = incomingJobs.ObserveOn( synchronizationContext );
			}

			Subscription = incomingJobs.Subscribe( onNextItem /*, onError */ );
		}

		public void Stop()
		{
			if( Subscription != null )
			{
				Subscription.Dispose();
				Subscription = null;
			}

			incomingJobs = null;
		}

		private Action Subscribe( IObserver<MhwFile> observer )
		{
			var subscribed = true;
			NamedPipeServerStream server;
			var cancellationSource = new CancellationTokenSource();

			Task.Factory.StartNew( () =>
			                       {
				                       while( subscribed )
				                       {
					                       server = CreateNewPipeServer();
					                       IAsyncResult asyncResult = server.BeginWaitForConnection( null, null );

					                       CancellationToken cancellationToken = GetCancellationToken( cancellationSource, server, asyncResult );

					                       NamedPipeServerStream localServerRef = server;
					                       Task connectionTask = Task.Factory.FromAsync( asyncResult, server.EndWaitForConnection );
					                       connectionTask.ContinueWith( _ => HandleIncomingFile( localServerRef, observer ),
					                                                    cancellationToken,
					                                                    TaskContinuationOptions.OnlyOnRanToCompletion,
					                                                    TaskScheduler.Current );

					                       connectionTask.Wait( cancellationToken );

					                       cancellationSource = new CancellationTokenSource();
				                       }
			                       },
			                       TaskCreationOptions.LongRunning );

			return () =>
			{
				subscribed = false;
				cancellationSource.Cancel();
			};
		}

		private static CancellationToken GetCancellationToken( CancellationTokenSource source,
		                                                       NamedPipeServerStream localServerRef,
		                                                       IAsyncResult result )
		{
			CancellationToken cancellationToken = source.Token;
			cancellationToken.Register( () =>
			{
				try
				{
					localServerRef.Dispose();
					localServerRef.EndWaitForConnection( result );
				}
				catch
				{
					// ignored
				}
			} );
			return cancellationToken;
		}

		private NamedPipeServerStream CreateNewPipeServer()
		{
			var server = new NamedPipeServerStream( PipeName,
			                                        PipeDirection.InOut,
			                                        2,
			                                        PipeTransmissionMode.Byte,
			                                        PipeOptions.Asynchronous,
			                                        0,
			                                        0,
			                                        null,
			                                        HandleInheritability.None,
			                                        0 );

			PipeSecurity accessControl = server.GetAccessControl();

			// Prevent 'Everyone' account from accessing pipe
			var securityIdentifier = new SecurityIdentifier( WellKnownSidType.WorldSid, null );
			var rule = new PipeAccessRule( securityIdentifier, PipeAccessRights.Read, AccessControlType.Allow );
			accessControl.RemoveAccessRule( rule );

			// Prevent 'Anonymous' account from accessing pipe
			securityIdentifier = new SecurityIdentifier( WellKnownSidType.AnonymousSid, null );
			rule = new PipeAccessRule( securityIdentifier, PipeAccessRights.Read, AccessControlType.Allow );
			accessControl.RemoveAccessRule( rule );

			return server;
		}

		private static void HandleIncomingFile( Stream pipeStream, IObserver<MhwFile> observer )
		{
			MhwFile file;
			try
			{
				var seekable = new MemoryStream();
				pipeStream.CopyTo( seekable );
				PdfDocument pdf = PdfReader.Open( seekable );

				// Remove title/file-name metadata
				string mhwFilename = pdf.Info.Elements.GetString( MHW_FILENAME );
				pdf.Info.Title = null;
				pdf.Info.Elements.Remove( MHW_FILENAME );

				// Save changes
				var stream = new MemoryStream();
				pdf.Save( stream, false );
				file = new MhwFile( mhwFilename, stream );
			}
			finally
			{
				pipeStream.Dispose();
			}
			observer.OnNext( file );
		}
	}
}