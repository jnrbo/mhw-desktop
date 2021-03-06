﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using myHEALTHwareDesktop.Properties;
using MHWVirtualPrinter;
using Setup;
using SOAPware.PortalApi.Model.Faxes;
using SOAPware.PortalSdk;

namespace myHEALTHwareDesktop
{
	public partial class ControlPrintToFax : UserControl
	{
		private const string FAX_APP_ID = "FAC5FAC5-45E9-434A-AF34-C9E070729299";
		private readonly VirtualPrinterManager virtualPrinterManager = new VirtualPrinterManager();

		private JobMonitor printJobMonitor;
		private SendFax sendFax;
		private readonly ActiveUserSession userSession;

		public ControlPrintToFax()
		{
			InitializeComponent();

			userSession = ActiveUserSession.GetInstance();
			userSession.ActingAsChanged += SelectedUserChanged;
		}

		private MhwSdk Sdk
		{
			get { return userSession.Sdk; }
		}

		public bool IsMhwFaxInstalled { get; set; }

		public bool IsPrinterInstalled
		{
			get { return virtualPrinterManager.IsPrinterAlreadyInstalled(MhwPrinter.PRINT_TO_FAX.PrinterName); }
		}

		public INotificationService NotificationService { get; set; }
		public IUploadService UploadService { get; set; }

		private bool IsWatcherRunning
		{
			get { return printJobMonitor != null; }
		}

		public void SelectedUserChanged( object sender, EventArgs e )
		{
			LoadPrintToFaxState();
		}

		private void LoadPrintToFaxState()
		{
			// Is MHW Fax installed on MHW account?
			try
			{
				Sdk.Application.GetConnection( userSession.ActingAsAccount.AccountId, FAX_APP_ID );
				IsMhwFaxInstalled = true;
				buttonPrintToFaxInstall.Enabled = true;
			}
			catch( Exception )
			{
				// Check network connection status.
				NotificationService.NotifyIfNetworkUnavailable();

				IsMhwFaxInstalled = false;
				labelStatus.Text = "myHEALTHware Fax is not installed on selected account.";
				labelFaxLink.Text = "Learn more about myHEALTHware Fax";
				buttonPrintToFaxInstall.Enabled = false;
				radioButtonPrompt.Enabled = false;
				radioButtonSaveDraft.Enabled = false;

				StopMonitoring();
				return;
			}

			labelFaxLink.Text = "Open myHEALTHware Fax";

			if( IsPrinterInstalled )
			{
				StartMonitoring();
				buttonPrintToFaxInstall.Text = "Uninstall Printer";
				labelStatus.Text = "Print to Fax is ready.";
			}
			else
			{
				StopMonitoring();
				buttonPrintToFaxInstall.Text = "Install Printer";
				labelStatus.Text = "Print to Fax printer is not installed.";
			}

			// Set the initial state of the radio buttons.
			radioButtonPrompt.Checked = userSession.Settings.PrintToFaxPrompt;
			radioButtonSaveDraft.Checked = !userSession.Settings.PrintToFaxPrompt;

			radioButtonPrompt.Enabled = IsPrinterInstalled;
			radioButtonSaveDraft.Enabled = IsPrinterInstalled;
		}

		private void RadioButtonsCheckedChanged( object sender, EventArgs e )
		{
			var radioButton = sender as RadioButton;
			if( radioButton == null || radioButton.Checked == false )
			{
				return;
			}

			////var message = "";

			if( radioButton == radioButtonSaveDraft )
			{
				userSession.Settings.PrintToFaxPrompt = false;
				//message = "Print to Fax will automatically save as draft fax";
			}
			else
			{
				userSession.Settings.PrintToFaxPrompt = true;
				//message = "Print to Fax will prompt you to send fax with each print";
			}

			userSession.Settings.Save();
			////NotificationService.ShowBalloonInfo( message );
		}

		private void ButtonPrintToFaxInstallClick( object sender, EventArgs e )
		{
			buttonPrintToFaxInstall.Enabled = false;

			if( IsPrinterInstalled )
			{
				Uninstall();
			}
			else
			{
				Install();
			}

			buttonPrintToFaxInstall.Enabled = true;
		}

		public void Uninstall()
		{
			// Args for uninstall
			var setupArgs = "-a -u -f";

			MhwSetup.LaunchAndWaitForExit( setupArgs );

			// See if the printer is now uninstalled.
			LoadPrintToFaxState();
		}

		public void Install()
		{
			// Args for install
			var setupArgs = "-a -f";

			MhwSetup.LaunchAndWaitForExit( setupArgs );

			// See if the printer is now installed.
			LoadPrintToFaxState();
		}

		private void StartMonitoring()
		{
			if( IsWatcherRunning )
			{
				// Already running.
				return;
			}

			if( !IsPrinterInstalled || !IsMhwFaxInstalled )
			{
				// Printer or app not installed.
				return;
			}

			// Create the folder watcher.
			printJobMonitor = new PrintToFaxMonitor();

#pragma warning disable 4014
			printJobMonitor.Start( p => ProcessNewPrintJob( p ), SynchronizationContext.Current );
#pragma warning restore 4014

			pictureStartedStopped.Image = Resources.started;
		}

		private void StopMonitoring()
		{
			if( !IsWatcherRunning )
			{
				return;
			}

			try
			{
				printJobMonitor.Stop();
				printJobMonitor = null;
			}
			catch( ArgumentException ex )
			{
				NotificationService.ShowBalloonError( "Stop Print to Fax monitor error: {0}", ex.Message );
			}

			pictureStartedStopped.Image = Resources.stopped;
		}

		// The Watcher calls this method when a new file shows up in the watched folder.
		// ReSharper disable once UnusedMethodReturnValue.Local
		private async Task ProcessNewPrintJob( MhwFile mhwFile )
		{
			if( !userSession.IsLoggedIn )
			{
				StopMonitoring();
				NotificationService.ShowBalloonError( "Please log in and try again. Print job deleted." );
				return;
			}

			// Upload the printed PDF file.
			string fileId = await UploadService.Upload( mhwFile.Content, mhwFile.Name, null );

			if( fileId == null )
			{
				NotificationService.ShowBalloonError( "Print to Fax failed: Unable to upload {0}", mhwFile.Name );
				return;
			}

			if( userSession.Settings.PrintToFaxPrompt )
			{
				LaunchSendFax( fileId );
			}
			else
			{
				// Create draft fax.
				var draftFax = new ApiFax
				{
					AccountId = userSession.ActingAsAccount.AccountId,
					FileId = fileId,
					To = new List<ApiFaxRecipient>()
				};

				try
				{
					Sdk.Fax.Create( draftFax, true, false, null, null );
					NotificationService.ShowBalloonInfo( "Print to Fax Draft succeeded: {0}", mhwFile.Name );
				}
				catch( Exception ex )
				{
					string details = mhwFile.Name;

					var httpEx = ex as HttpException;
					if( httpEx != null && httpEx.GetHttpCode() == (int) HttpStatusCode.Forbidden )
					{
						details = "missing fax permission";
					}

					NotificationService.ShowBalloonError( "Print to Fax Draft failed: {0}", details );
				}
			}
		}

		private void LaunchSendFax( string fileId )
		{
			sendFax = new SendFax( userSession, fileId );

			// Register a method to recieve click event callback.
			sendFax.Click += SendFaxOnClick;

			try
			{
				sendFax.ShowDialog( this );
			}
			finally
			{
				sendFax.Dispose();
			}
		}

		private void SendFaxOnClick( object sender, EventArgs e )
		{
			GetSendFaxResults( (PostMessageListenerEventArgs) e );
		}

		// This method is called when SendFax fires the Click event so that we can
		// retrieve the results from it.
		private void GetSendFaxResults( PostMessageListenerEventArgs args )
		{
			if( args.Message.eventType == "mhw.fax.send.success" )
			{
				NotificationService.ShowBalloonInfo( "Print to Fax succeeded" );
			}
			else if( args.Message.eventType == "mhw.fax.send.cancelled" )
			{
				// Cancelled.
				////NotificationService.ShowBalloonWarning( "Print to Fax was cancelled." );
			}
			else
			{
				NotificationService.ShowBalloonInfo( "Send Fax Error: {0}", args.Message.data.message );
			}

			// Close the Drive picker form in a cross thread acceptable way.
			BeginInvoke( (MethodInvoker) ( () => sendFax.Dispose() ) );
		}

		private void LabelFaxLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			string accountId = userSession.ActingAsAccount.AccountId;
			string mhwLink = IsMhwFaxInstalled
				? string.Format( "{0}/#/Fax?accountId={1}", userSession.Settings.myHEALTHwareDomain, accountId )
				: string.Format( "{0}/#/Marketplace?accountId={1}", userSession.Settings.myHEALTHwareDomain, accountId );

			Process.Start( mhwLink );
		}
	}
}
