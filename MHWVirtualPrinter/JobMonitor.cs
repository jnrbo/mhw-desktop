using System;
using System.IO;
using System.IO.Pipes;
using System.Reactive.Linq;
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
			var source = new CancellationTokenSource();

			Task.Factory.StartNew( () =>
			                       {
				                       while( subscribed )
				                       {
					                       server = new NamedPipeServerStream( PipeName,
					                                                           PipeDirection.InOut,
					                                                           2,
					                                                           PipeTransmissionMode.Byte,
					                                                           PipeOptions.Asynchronous,
					                                                           0,
					                                                           0,
					                                                           null,
					                                                           HandleInheritability.None,
					                                                           0 );

					                       ////			var accessControl = server.GetAccessControl();
					                       ////			var ruleCollection = accessControl.GetAccessRules(true, true, typeof(NTAccount));
					                       ////			accessControl.

					                       NamedPipeServerStream local = server;
					                       IAsyncResult result = server.BeginWaitForConnection( null, null );

					                       CancellationToken cancellationToken = source.Token;
					                       cancellationToken.Register( () =>
					                       {
						                       try
						                       {
							                       local.Dispose();
							                       local.EndWaitForConnection( result );
						                       }
						                       catch
						                       {
							                       // ignored
						                       }
					                       } );

					                       Task connectionTask = Task.Factory.FromAsync( result, server.EndWaitForConnection );
					                       connectionTask.ContinueWith( _ =>
					                                                    {
						                                                    MhwFile file;
						                                                    try
						                                                    {
							                                                    var stream = new MemoryStream();
							                                                    local.CopyTo( stream );

							                                                    PdfDocument pdf = PdfReader.Open( stream, PdfDocumentOpenMode.InformationOnly );
							                                                    stream.Seek( 0, SeekOrigin.Begin );
							                                                    file = new MhwFile( pdf.Info.Elements.GetString( MHW_FILENAME ), stream );
						                                                    }
						                                                    finally
						                                                    {
							                                                    local.Dispose();
						                                                    }
						                                                    observer.OnNext( file );
					                                                    },
					                                                    cancellationToken,
					                                                    TaskContinuationOptions.OnlyOnRanToCompletion,
					                                                    TaskScheduler.Current );

					                       connectionTask.Wait( cancellationToken );

					                       source = new CancellationTokenSource();
				                       }
			                       },
			                       TaskCreationOptions.LongRunning );

			return () =>
			{
				subscribed = false;
				source.Cancel();
			};
		}
	}
}