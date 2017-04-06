using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;

// This class is full of old-world black magic. You have been warned.

namespace MHWVirtualPrinter
{
	public class VirtualPrinterManager
	{
		private const string MONITOR_DLL_NAME = "mfilemon.dll";
		private const string MONITOR_UI_NAME = "mfilemonUI.dll";
		private const string COMMENT = "A virtual printer that uploads print jobs to myHEALTHware Drive";

		private readonly WinSpool winspool = new WinSpool();

		public VirtualPrinterManager()
		{
			IsError = false;
		}

		public bool IsError { get; set; }

		public bool IsMonitorAlreadyInstalled( string monitorName )
		{
			// Ensure spool service is running.
			winspool.StartSpoolService();

			// EnumMonitors to see if already installed.
			List<WinSpool.Monitor> monitors = winspool.GetInstalledMonitors();

			return monitors.Any( m => m.pName == monitorName );
		}

		public void AddPrinterMonitor( string currentDirectory, string platform, string monitorName )
		{
			SetupMonitorFiles( currentDirectory, platform );

			string exceptionMessage = null;

			try
			{
				exceptionMessage = string.Format( "Adding monitor {0} in {1}", monitorName, MONITOR_DLL_NAME );
				winspool.AddMonitor( monitorName, null, MONITOR_DLL_NAME );
			}
			catch( Exception ex )
			{
				throw new Exception( exceptionMessage, ex );
			}
		}

		private static void SetupMonitorFiles( string currentDirectory, string platform, bool isUninstall = false )
		{
			string exceptionMessage = null;

			try
			{
				string sourceDirectory = Path.Combine( currentDirectory, platform, "PortMonitor" );
				string sourceDllName = Path.Combine( sourceDirectory, MONITOR_DLL_NAME );
				string sourceUIName = Path.Combine( sourceDirectory, MONITOR_UI_NAME );
				////string sourceUIName2 = Path.Combine( sourceDirectory, MONITOR_UI_NAME2 );

				//string targetDirectory = Environment.SystemDirectory;
				//if (platform == "x64")
				//{
				// We are a 32 bit app so Window's file system redirection tries to change this
				// to SysWow64 for 32 bit files.  But we really want to place 64 bit files here
				// so we need to override to copy to "windows\system32"
				// See http://stackoverflow.com/questions/10100390/file-getting-copied-to-syswow64-instead-of-system32
				string targetDirectory = Environment.ExpandEnvironmentVariables( "%windir%\\SysNative" );
				//}

				string targetDllName = Path.Combine( targetDirectory, MONITOR_DLL_NAME );

				if( File.Exists( targetDllName ) && isUninstall )
				{
					// Uninstall files.
					exceptionMessage = string.Format( "Delete {0}", targetDllName );
					File.Delete( targetDllName );
				}
				else if( !File.Exists( targetDllName ) && !isUninstall )
				{
					// Install files.
					exceptionMessage = string.Format( "Copying {0} to {1}", sourceDllName, targetDllName );
					File.Copy( sourceDllName, targetDllName, true );
				}

				string targetUIName = Path.Combine( targetDirectory, MONITOR_UI_NAME );

				if( File.Exists( targetUIName ) && isUninstall )
				{
					exceptionMessage = string.Format( "Delete {0}", targetUIName );
					File.Delete( targetUIName );
				}
				else if( !File.Exists( targetUIName ) && !isUninstall )
				{
					exceptionMessage = string.Format( "Copying {0} to {1}", sourceUIName, targetUIName );
					File.Copy( sourceUIName, targetUIName, true );
				}
			}
			catch( UnauthorizedAccessException )
			{
				// Files are in use likely by another monitor so just leave them.
			}
			catch( Exception ex )
			{
				throw new Exception( exceptionMessage, ex );
			}
		}

		public void RemovePrinterMonitor( string currentDirectory, string platform, string monitorName )
		{
			winspool.DeleteMonitor( monitorName );
			SetupMonitorFiles( currentDirectory, platform, true );
		}

		public bool IsPortAlreadyInstalled( string portName )
		{
			List<WinSpool.Port> ports = winspool.GetInstalledPorts();

			return ports.Any( p => p.name == portName );
		}

		public void AddPrinterPort( string portName, string monitorName )
		{
			winspool.AddPort( portName, monitorName );
		}

		public void RemovePrinterPort( IntPtr parentWindow, string portName )
		{
			winspool.DeletePort( parentWindow, portName );
		}

		public bool IsDriverAlreadyInstalled( string driverName )
		{
			List<WinSpool.Driver> drivers = winspool.GetInstalledDrivers();

			return drivers.Any( d => d.name == driverName );
		}

		public void AddPrinterDriver( string currentDirectory, string platform, string driverName )
		{
			// NOTE: These file names are the same for both platforms.
			var driverFileMain = "PSCRIPT5.DLL";
			var driverFileData = "myHEALTHwarePS.PPD";
			var driverFileConfig = "PS5UI.DLL";
			var driverFileHelp = "PSCRIPT.HLP";
			var driverFileDependencies = "";
			// "hpbafd32.dll\0hpbftm32.dll\0HPLJ8550.cfg\0hpcdmc32.dll\0hpbcfgre.dll\0hpdcmon.dll\0\0";

			string sourceDirectory = Path.Combine( currentDirectory, platform, "Driver" );
			string sourceFileMain = Path.Combine( sourceDirectory, driverFileMain );
			string sourceFileData = Path.Combine( sourceDirectory, driverFileData );
			string sourceFileConfig = Path.Combine( sourceDirectory, driverFileConfig );
			string sourceFileHelp = Path.Combine( sourceDirectory, driverFileHelp );

			var targetSubdirectory = ""; // Can't have a subdirectory.
			string targetDirectory = Path.Combine( winspool.GetSystemDirectory(), targetSubdirectory );
			if( !Directory.Exists( targetDirectory ) )
			{
				Directory.CreateDirectory( targetDirectory );
			}
			string targetFileMain = Path.Combine( targetDirectory, driverFileMain );
			string targetFileData = Path.Combine( targetDirectory, driverFileData );
			string targetFileConfig = Path.Combine( targetDirectory, driverFileConfig );
			string targetFileHelp = Path.Combine( targetDirectory, driverFileHelp );

			// Copy driver files to the system.
			if( !File.Exists( targetFileMain ) )
			{
				File.Copy( sourceFileMain, targetFileMain, true );
			}

			if( !File.Exists( targetFileData ) )
			{
				File.Copy( sourceFileData, targetFileData, true );
			}

			if( !File.Exists( targetFileConfig ) )
			{
				File.Copy( sourceFileConfig, targetFileConfig, true );
			}

			if( !File.Exists( targetFileHelp ) )
			{
				File.Copy( sourceFileHelp, targetFileHelp, true );
			}

			winspool.AddPrinterDriver( driverName,
			                           targetFileMain,
			                           targetFileData,
			                           targetFileConfig,
			                           targetFileHelp,
			                           driverFileDependencies );
		}

		public void RemovePrinterDriver( string driverName )
		{
			winspool.DeletePrinterDriver( driverName );
		}

		public bool IsPrinterAlreadyInstalled( string printerName )
		{
			// Ensure spool service is running.
			winspool.StartSpoolService();

			List<WinSpool.Printer> printers = winspool.GetInstalledPrinters();

			return printers.Any( p => p.Name == printerName );
		}

		public void AddPrinter( MhwPrinter mhwPrinter )
		{
			winspool.AddPrinter( mhwPrinter.PrinterName, mhwPrinter.PortName, mhwPrinter.DriverName, COMMENT );
		}

		public void RemovePrinter( MhwPrinter mhwPrinter )
		{
			winspool.DeletePrinter( mhwPrinter.PrinterName,
			                        mhwPrinter.PortName,
			                        mhwPrinter.MonitorName,
			                        mhwPrinter.DriverName,
			                        COMMENT );
		}

		// I believe these virtual port parameters are specific to printmon
		public void ConfigureVirtualPort( MhwPrinter mhwPrinter, PdfEngine pdfEngine )
		{
			var filePattern = "%r-%u-%Y%m%d-%H%n%s.pdf";
			string userCommand =
				string.Format(
					@"{0} -dSAFER -dNOPAUSE -sDEVICE=pdfwrite -sOutputFile=""\\.\pipe\{1}\%u"" -c ""[ /MhwFilename (%f) /DOCINFO pdfmark"" -f -",
					pdfEngine.PathExe,
					mhwPrinter.PipeRoot );

			string keyName = string.Format( @"SYSTEM\CurrentControlSet\Control\Print\Monitors\{0}\{1}", mhwPrinter.MonitorName, mhwPrinter.PortName );
			Registry.LocalMachine.CreateSubKey( keyName );
			RegistryKey regKey = Registry.LocalMachine.OpenSubKey( keyName, true );

			// ReSharper disable once PossibleNullReferenceException
			regKey.SetValue( "OutputPath", string.Empty, RegistryValueKind.String );

			regKey.SetValue( "FilePattern", filePattern, RegistryValueKind.String );
			regKey.SetValue( "Overwrite", 0, RegistryValueKind.DWord );
			regKey.SetValue( "UserCommand", userCommand, RegistryValueKind.String );
			regKey.SetValue( "ExecPath", string.Empty, RegistryValueKind.String );
			regKey.SetValue( "PipeData", 0x1, RegistryValueKind.DWord );
			regKey.SetValue( "WaitTermination", 0, RegistryValueKind.DWord );
			regKey.Close();
		}

		public void RestartSpoolService()
		{
			winspool.StopSpoolService();
			winspool.StartSpoolService();
		}
	}
}
