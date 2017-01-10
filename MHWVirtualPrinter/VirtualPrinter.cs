using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

// This class is full of old-world black magic. You have been warned.

namespace MHWVirtualPrinter
{
	public class VirtualPrinter
	{
		private WinSpool winspool = new WinSpool();
		public bool isError = false;

		public bool IsMonitorAlreadyInstalled(string monitorName)
		{
			// Ensure spool service is running.
			winspool.StartSpoolService();

			// EnumMonitors to see if already installed.
			var monitors = winspool.GetInstalledMonitors();

			foreach (var monitor in monitors)
			{
				if (monitor.pName == monitorName)
				{
					return true;
				}
			}

			return false;
		}

		public void AddPrinterMonitor(string currentDirectory, string platform, string monitorName)
		{
			SetupMonitorFiles(currentDirectory, platform);

			const string monitorDllName = "mfilemon.dll";
			string exceptionMessage = null;

			try
			{
				exceptionMessage = string.Format("Adding monitor {0} in {1}", monitorName, monitorDllName);
				winspool.AddMonitor(monitorName, null, monitorDllName);
			}
			catch (Exception ex)
			{
				throw new Exception(exceptionMessage, ex);
			}
		}

		private void SetupMonitorFiles(string currentDirectory, string platform, bool isUninstall = false)
		{
			string exceptionMessage = null;

			try
			{
				const string monitorDllName = "mfilemon.dll";
				const string monitorUIName = "mfilemonUI.dll";
				const string monitorUIName2 = "_mfilemonUI.dll";
				string sourceDirectory = Path.Combine(currentDirectory, platform, "PortMonitor");
				string sourceDllName = Path.Combine(sourceDirectory, monitorDllName);
				string sourceUIName = Path.Combine(sourceDirectory, monitorUIName);
				string sourceUIName2 = Path.Combine(sourceDirectory, monitorUIName2);

				//string targetDirectory = Environment.SystemDirectory;
				//if (platform == "x64")
				//{
					// We are a 32 bit app so Window's file system redirection tries to change this
					// to SysWow64 for 32 bit files.  But we really want to place 64 bit files here
					// so we need to override to copy to "windows\system32"
					// See http://stackoverflow.com/questions/10100390/file-getting-copied-to-syswow64-instead-of-system32
					string targetDirectory = Environment.ExpandEnvironmentVariables("%windir%\\SysNative");
				//}

				string targetDllName = Path.Combine(targetDirectory, monitorDllName);

				if (File.Exists(targetDllName) && isUninstall)
				{
					// Uninstall files.
					exceptionMessage = string.Format("Delete {0}", targetDllName);
					File.Delete(targetDllName);
				}
				else if (!File.Exists(targetDllName) && !isUninstall)
				{
					// Install files.
					exceptionMessage = string.Format("Copying {0} to {1}", sourceDllName, targetDllName);
					File.Copy(sourceDllName, targetDllName, true);
				}

				string targetUIName = Path.Combine(targetDirectory, monitorUIName);

				if (File.Exists(targetUIName) && isUninstall)
				{
					exceptionMessage = string.Format("Delete {0}", targetUIName);
					File.Delete(targetUIName);
				}
				else if (!File.Exists(targetUIName) && !isUninstall)
				{
					exceptionMessage = string.Format("Copying {0} to {1}", sourceUIName, targetUIName);
					File.Copy(sourceUIName, targetUIName, true);
				}

				string targetUIName2 = Path.Combine(targetDirectory, monitorUIName2);

				if (File.Exists(targetUIName2) && isUninstall)
				{
					exceptionMessage = string.Format("Delete {0}", targetUIName2);
					File.Delete(targetUIName2);
				}
				else if (!File.Exists(targetUIName2) && !isUninstall)
				{
					exceptionMessage = string.Format("Copying {0} to {1}", sourceUIName2, targetUIName2);
					File.Copy(sourceUIName2, targetUIName2, true);
				}
			}
			catch (UnauthorizedAccessException)
			{
				// Files are in use likely by another monitor so just leave them.
			}
			catch (Exception ex)
			{
				throw new Exception(exceptionMessage, ex);
			}
		}

		public void RemovePrinterMonitor(string currentDirectory, string platform, string monitorName)
		{
			winspool.DeleteMonitor(monitorName);
			SetupMonitorFiles(currentDirectory, platform, true);
		}

		public bool IsPortAlreadyInstalled(string portName)
		{
			var ports = winspool.GetInstalledPorts();

			foreach (var port in ports)
			{
				if (port.name == portName)
				{
					return true;
				}
			}

			return false;
		}

		public void AddPrinterPort(string portName, string monitorName)
		{
			winspool.AddPort(portName, monitorName);
		}

		public void RemovePrinterPort(IntPtr parentWindow, string portName)
		{
			winspool.DeletePort(parentWindow, portName);
		}

		public bool IsDriverAlreadyInstalled(string driverName)
		{
			var drivers = winspool.GetInstalledDrivers();

			foreach (var driver in drivers)
			{
				if (driver.name == driverName)
				{
					return true;
				}
			}

			return false;
		}

		public void AddPrinterDriver(string currentDirectory, string platform, string driverName)
		{
			// NOTE: These file names are the same for both platforms.
			string driverFileMain = "PSCRIPT5.DLL";
			string driverFileData = "myHEALTHwarePS.PPD";
			string driverFileConfig = "PS5UI.DLL";
			string driverFileHelp = "PSCRIPT.HLP";
			string driverFileDependencies = ""; // "hpbafd32.dll\0hpbftm32.dll\0HPLJ8550.cfg\0hpcdmc32.dll\0hpbcfgre.dll\0hpdcmon.dll\0\0";

			string sourceDirectory = Path.Combine(currentDirectory, platform, "Driver");
			string sourceFileMain = Path.Combine(sourceDirectory, driverFileMain);
			string sourceFileData = Path.Combine(sourceDirectory, driverFileData);
			string sourceFileConfig = Path.Combine(sourceDirectory, driverFileConfig);
			string sourceFileHelp = Path.Combine(sourceDirectory, driverFileHelp);

			string targetSubdirectory = "";	// Can't have a subdirectory.
			string targetDirectory = Path.Combine(winspool.GetSystemDirectory(), targetSubdirectory);
			if (!Directory.Exists(targetDirectory))
			{
				Directory.CreateDirectory(targetDirectory);
			}
			string targetFileMain = Path.Combine(targetDirectory, driverFileMain);
			string targetFileData = Path.Combine(targetDirectory, driverFileData);
			string targetFileConfig = Path.Combine(targetDirectory, driverFileConfig);
			string targetFileHelp = Path.Combine(targetDirectory, driverFileHelp);
			
			// Copy driver files to the system.
			if (File.Exists(targetFileMain) == false)
			{
				File.Copy(sourceFileMain, targetFileMain, true);
			}

			if (File.Exists(targetFileData) == false)
			{
				File.Copy(sourceFileData, targetFileData, true);
			}

			if (File.Exists(targetFileConfig) == false)
			{
				File.Copy(sourceFileConfig, targetFileConfig, true);
			}

			if (File.Exists(targetFileHelp) == false)
			{
				File.Copy(sourceFileHelp, targetFileHelp, true);
			}

			winspool.AddPrinterDriver(driverName, targetFileMain, targetFileData,
									targetFileConfig, targetFileHelp, driverFileDependencies);
		}

		public void RemovePrinterDriver(string driverName)
		{
			winspool.DeletePrinterDriver(driverName);
		}

		public bool IsPrinterAlreadyInstalled(string printerName)
		{
			// Ensure spool service is running.
			winspool.StartSpoolService();

			var printers = winspool.GetInstalledPrinters();

			foreach (var printer in printers)
			{
				if (printer.name == printerName)
				{
					return true;
				}
			}

			return false;
		}

		public void AddPrinter(string printerName, string portName, string driverName)
		{
			const string comment = "A virtual printer that uploads print jobs to myHEALTHware Drive";
			winspool.AddPrinter(printerName, portName, driverName, comment);
		}

		public void RemovePrinter(string printerName, string portName, string monitorName, string driverName)
		{
			const string comment = "A virtual printer that uploads print jobs to myHEALTHware Drive";
			winspool.DeletePrinter(printerName, portName, monitorName, driverName, comment);
		}

		// I believe these virtual port parameters are specific to printmon
		public void ConfigureVirtualPort(string monitorName, string portName, PDFEngine pdfEngine)
		{
			//string outputPath = Path.Combine(currentDirectory, "Temp", monitorName);
			string outputPath = Path.Combine(Path.GetTempPath(), monitorName);
			Directory.CreateDirectory(outputPath);

			string filePattern = "%r-%u-%Y%m%d-%i.pdf";
			string userCommand = String.Format("{0} -dSAFER -dNOPAUSE -sDEVICE=pdfwrite -sOutputFile=\"%f\" -", pdfEngine.pathExe);
			string execPath = "";

			string keyName = string.Format(@"SYSTEM\CurrentControlSet\Control\Print\Monitors\{0}\{1}", monitorName, portName);
			Registry.LocalMachine.CreateSubKey(keyName);
			RegistryKey regKey = Registry.LocalMachine.OpenSubKey(keyName, true);

			regKey.SetValue("OutputPath", outputPath, RegistryValueKind.String);
			regKey.SetValue("FilePattern", filePattern, RegistryValueKind.String);
			regKey.SetValue("Overwrite", 0, RegistryValueKind.DWord);
			regKey.SetValue("UserCommand", userCommand, RegistryValueKind.String);
			regKey.SetValue("ExecPath", execPath, RegistryValueKind.String);
			regKey.SetValue("PipeData", 0x1, RegistryValueKind.DWord);
			regKey.SetValue("WaitTermination", 0, RegistryValueKind.DWord);
			regKey.Close();
		}

		public void RestartSpoolService()
		{
			winspool.StopSpoolService();
			winspool.StartSpoolService();
		}
	}
}
