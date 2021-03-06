﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MHWVirtualPrinter;

namespace Setup
{
	public partial class MainForm : Form
	{
		private Log log;
		private readonly VirtualPrinterManager virtualPrinterManager;
		private readonly Options options;
		private bool allowClose = true;
		private readonly MhwPrinter printer;

		public int ReturnCode { get; set; }

		public MainForm( Options options )
		{
			this.options = options;

			printer = options.PrintToFax ? MhwPrinter.PRINT_TO_FAX : MhwPrinter.PRINT_TO_DRIVE;

			virtualPrinterManager = new VirtualPrinterManager();
			InitializeComponent();
		}

		private void MainFormShown( object sender, EventArgs e )
		{
			allowClose = false;
			UseWaitCursor = true;

			ThreadStart starter = RunSetup;
			starter += SetupDone;
			var t = new Thread( starter ) { IsBackground = true };
			t.Start();
		}

		private void SetupDone()
		{
			// Adjust column width to longest message size.
			log.AdjustLogWidth( -1 );

			if( InvokeRequired )
			{
				Invoke( new MethodInvoker( SetupDone ) );
			}
			else
			{
				// Enable the button for user to dismiss.
				buttonExit.Enabled = true;
				allowClose = true;
				UseWaitCursor = false;
			}
		}

		private void RunSetup()
		{
			log = new Log( listViewLog, columnHeaderMessage );

			string currentDirectory = GetApplicationDirectory();

			// Determine platform we're running on.
			string platform = Environment.Is64BitOperatingSystem ? "x64" : "x86";

			if( options.Uninstall )
			{
				if( options.PrintToDrive || options.PrintToFax )
				{
					log.Info( "Beginning uninstall" );

					try
					{
						Uninstall( currentDirectory, platform );
					}
					catch( Win32Exception ex )
					{
						log.Error( "Error {0}: {1}", ex.NativeErrorCode, ex.Message );
						ReturnCode = -1;
						virtualPrinterManager.IsError = true;
					}
				}
			}
			else
			{
				if( options.PrintToDrive || options.PrintToFax )
				{
					log.Info( "Beginning install" );

					try
					{
						Install( currentDirectory, platform );
					}
					catch( Win32Exception ex )
					{
						log.Error( "Error {0}: {1}", ex.NativeErrorCode, ex.Message );
						ReturnCode = -1;
						virtualPrinterManager.IsError = true;
					}
				}
			}

			if( virtualPrinterManager.IsError )
			{
				log.Warning( "Finished with errors. See above." );
			}
			else
			{
				log.Success( "Finished successfully!" );

				if( !options.Uninstall && !options.RanFromApp )
				{
					log.Info( "Click OK to start myHEALTHware Desktop" );
				}
			}
		}

		private static string GetApplicationDirectory()
		{
			return Application.StartupPath;
		}

		private void Install( string currentDirectory, string platform )
		{
			log.Info( "Platform is {0}.", platform );

			log.Info( "OS is {0}.", Environment.OSVersion );

			if( virtualPrinterManager.IsMonitorAlreadyInstalled( printer.MonitorName ) )
			{
				log.Warning( "Monitor {0} already installed.", printer.MonitorName );
			}
			else
			{
				log.Info( "Installing monitor {0}.", printer.MonitorName );

				try
				{
					virtualPrinterManager.AddPrinterMonitor( currentDirectory, platform, printer.MonitorName );
					log.Success( "Monitor {0} successfully installed.", printer.MonitorName );
				}
				catch( Exception ex )
				{
					log.Error( "Install monitor failed: {0}", ex.Message );
				}
			}

			if( virtualPrinterManager.IsPortAlreadyInstalled( printer.PortName ) )
			{
				log.Warning( "Port {0} already installed.", printer.PortName );
			}
			else
			{
				log.Info( "Installing port {0}.", printer.PortName );
				virtualPrinterManager.AddPrinterPort( printer.PortName, printer.MonitorName );
				log.Success( "Port {0} successfully installed.", printer.PortName );
			}

			if( virtualPrinterManager.IsDriverAlreadyInstalled( printer.DriverName ) )
			{
				log.Warning( "Driver {0} already installed.", printer.DriverName );
			}
			else
			{
				log.Info( "Installing driver {0}.", printer.DriverName );
				virtualPrinterManager.AddPrinterDriver( currentDirectory, platform, printer.DriverName );
				log.Success( "Driver {0} successfully installed.", printer.DriverName );
			}

			if( virtualPrinterManager.IsPrinterAlreadyInstalled( printer.PrinterName ) )
			{
				log.Warning( "Printer {0} already installed.", printer.PrinterName );
			}
			else
			{
				log.Info( "Installing printer {0}.", printer.PrinterName );
				virtualPrinterManager.AddPrinter( printer );
				log.Success( "Printer {0} successfully installed.", printer.PrinterName );
			}

			log.Info( "Installing PDF engine." );
			var pdfEngine = new PdfEngine( currentDirectory, platform );
			pdfEngine.Install();

			log.Success( "PDF engine installed successfully." );

			log.Info( "Configuring port." );
			virtualPrinterManager.ConfigureVirtualPort( printer, pdfEngine );
			log.Success( "Port configured successfully." );

			log.Info( "Restarting spool service. Please wait..." );
			virtualPrinterManager.RestartSpoolService();
			log.Success( "Restart spool service complete." );
		}

		private void Uninstall( string currentDirectory, string platform )
		{
			if( virtualPrinterManager.IsPrinterAlreadyInstalled( printer.PrinterName ) )
			{
				log.Info( "Removing printer {0}.", printer.PrinterName );
				virtualPrinterManager.RemovePrinter( printer );
				log.Success( "Removing printer {0} complete.", printer.PrinterName );
			}
			else
			{
				log.Warning( "Printer {0} not installed.", printer.PrinterName );
			}

			if( virtualPrinterManager.IsPortAlreadyInstalled( printer.PortName ) )
			{
				log.Info( "Removing port {0}.", printer.PortName );
				virtualPrinterManager.RemovePrinterPort( Handle, printer.PortName );
				log.Success( "Removing port {0} complete.", printer.PortName );
			}
			else
			{
				log.Warning( "Port {0} not installed.", printer.PortName );
			}

			if( virtualPrinterManager.IsDriverAlreadyInstalled( printer.DriverName ) )
			{
				// Check to see if another printer is still using this driver.
				if( IsDriverBeingUsedByAnotherPrinter() )
				{
					log.Info( "Driver {0} is being used by another printer so not removing.", printer.DriverName );
				}
				else
				{
					log.Info( "Removing driver {0}.", printer.DriverName );
					virtualPrinterManager.RemovePrinterDriver( printer.DriverName );
					log.Success( "Removing driver {0} complete.", printer.DriverName );
				}
			}
			else
			{
				log.Warning( "Driver {0} not installed.", printer.DriverName );
			}

			if( virtualPrinterManager.IsMonitorAlreadyInstalled( printer.MonitorName ) )
			{
				log.Info( "Removing monitor {0}.", printer.MonitorName );
				virtualPrinterManager.RemovePrinterMonitor( currentDirectory, platform, printer.MonitorName );
				log.Success( "Removing monitor {0} complete.", printer.MonitorName );
			}
			else
			{
				log.Warning( "Monitor {0} not installed.", printer.MonitorName );
			}

			log.Info( "Restarting spool service. Please wait..." );
			virtualPrinterManager.RestartSpoolService();
			log.Success( "Restart spool service complete." );
		}

		private bool IsDriverBeingUsedByAnotherPrinter()
		{
			string otherPrinterName = printer == MhwPrinter.PRINT_TO_DRIVE
				? MhwPrinter.PRINT_TO_FAX.PrinterName
				: MhwPrinter.PRINT_TO_DRIVE.PrinterName;

			// Look for other printer.
			return virtualPrinterManager.IsPrinterAlreadyInstalled( otherPrinterName );
		}

		private void KeyDownHandler( object sender, KeyEventArgs e )
		{
			if( sender != listViewLog )
			{
				return;
			}

			if( e.Modifiers == Keys.Control && e.KeyCode == Keys.C || e.Modifiers == Keys.Control && e.KeyCode == Keys.Insert )
			{
				CopyToClipboard();
			}
		}

		private void ButtonCopyClick( object sender, EventArgs e )
		{
			foreach( ListViewItem item in listViewLog.Items )
			{
				item.Selected = true;
			}

			CopyToClipboard();
			MessageBox.Show( "Log copied to clipboard", "Copy Log" );
		}

		private void CopyToClipboard()
		{
			if( listViewLog.SelectedItems.Count <= 0 )
			{
				return;
			}

			var selectedItems = new StringBuilder();
			foreach( ListViewItem item in listViewLog.SelectedItems )
			{
				selectedItems.AppendLine( item.Text );
			}

			Clipboard.SetText( selectedItems.ToString() );
		}

		private void ButtonExitClick( object sender, EventArgs e )
		{
			if( allowClose && !options.RanFromApp && !options.Uninstall )
			{
				string currentDirectory = GetApplicationDirectory();

				// Start the main app.
				try
				{
					Process.Start( string.Format( "{0}\\{1}.exe", currentDirectory, MhwPrinter.APP_NAME ) );
				}
				catch( Exception ex )
				{
					log.Error( "Could not start main app: {0}", ex.Message );
				}
			}

			Dispose();
		}

		private void MainFormFormClosing( object sender, FormClosingEventArgs e )
		{
			e.Cancel = !allowClose;
		}
	}
}
