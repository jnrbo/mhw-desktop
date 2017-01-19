using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;

// This class is full of old-world black magic. You have been warned.

namespace MHWVirtualPrinter
{
	public class WinSpool
	{
		private const int MAX_PORTNAME_LEN = 64;
		private const string SPOOLER_SERVICE_NAME = "Spooler";

		//http://pinvoke.net/default.aspx/winspool.EnumMonitors
		[DllImport( "winspool.drv", SetLastError = true, CharSet = CharSet.Auto )]
		private static extern bool EnumMonitors( string pName,
		                                         uint level,
		                                         IntPtr pMonitors,
		                                         uint cbBuf,
		                                         ref uint pcbNeeded,
		                                         ref uint pcReturned );

		public class Monitor
		{
			public string pName;
			public string pEnvironment;
			public string pDLLName;
		}

		public List<Monitor> GetInstalledMonitors()
		{
			var monitors = new List<Monitor>();
			uint pcbNeeded = 0;
			uint pcReturned = 0;

			if( EnumMonitors( null, 2, IntPtr.Zero, 0, ref pcbNeeded, ref pcReturned ) )
			{
				//succeeds, but must not, because buffer is zero (too small)!
				throw new Exception( "EnumMonitors should fail!" );
			}

			int lastWin32Error = Marshal.GetLastWin32Error();

			const int ERROR_INSUFFICIENT_BUFFER = 122;

			if( lastWin32Error != ERROR_INSUFFICIENT_BUFFER )
			{
				throw new Win32Exception( lastWin32Error );
			}

			IntPtr pMonitors = Marshal.AllocHGlobal( (int) pcbNeeded );

			if( !EnumMonitors( null, 2, pMonitors, pcbNeeded, ref pcbNeeded, ref pcReturned ) )
			{
				throw new Win32Exception( Marshal.GetLastWin32Error() );
			}

			IntPtr pIndex = pMonitors;

			for( var i = 0; i < pcReturned; i++ )
			{
				var monitorStruct = (MONITOR_INFO_2) Marshal.PtrToStructure( pIndex, typeof( MONITOR_INFO_2 ) );

				monitors.Add( new Monitor
				{
					pDLLName = monitorStruct.pDLLName,
					pEnvironment = monitorStruct.pEnvironment,
					pName = monitorStruct.pName
				} );

				// Increment index pointer to next struct.
				//pIndex = (IntPtr)(pIndex.ToInt32() + Marshal.SizeOf(typeof(MONITOR_INFO_2)));
				pIndex += Marshal.SizeOf( typeof( MONITOR_INFO_2 ) );
			}

			Marshal.FreeHGlobal( pMonitors );

			return monitors;
		}

		// http://pinvoke.net/default.aspx/winspool.AddMonitor
		[DllImport( "winspool.drv", SetLastError = true, CharSet = CharSet.Auto )]
		private static extern int AddMonitor( string pName, uint Level, ref MONITOR_INFO_2 pMonitors );

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
		private struct MONITOR_INFO_2
		{
			//[MarshalAs(UnmanagedType.LPTStr)]
			public string pName;

			//[MarshalAs(UnmanagedType.LPTStr)]
			public string pEnvironment;

			//[MarshalAs(UnmanagedType.LPTStr)]
			public string pDLLName;
		}

		public void AddMonitor( string monitorName, string environment, string dllName )
		{
			var mi2 = new MONITOR_INFO_2 { pName = monitorName, pEnvironment = environment, pDLLName = dllName };

			if( AddMonitor( null, 2, ref mi2 ) == 0 )
			{
				throw new Win32Exception( Marshal.GetLastWin32Error() );
			}
		}

		[DllImport( "winspool.drv", SetLastError = true, CharSet = CharSet.Auto )]
		private static extern int DeleteMonitor( string pName, string pEnvironment, string pMonitorName );

		public void DeleteMonitor( string monitorName )
		{
			if( DeleteMonitor( null, null, monitorName ) == 0 )
			{
				throw new Win32Exception( Marshal.GetLastWin32Error() );
			}
		}

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
		private struct PortData
		{
			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = MAX_PORTNAME_LEN )]
			public string sztPortName;
		}

		private enum PrinterAccess
		{
			Delete = 0x10000,
			ServerAdmin = 0x01,
			ServerEnum = 0x02,
			PrinterAdmin = 0x04,
			PrinterUse = 0x08,
			JobAdmin = 0x10,
			JobRead = 0x20,
			StandardRightsRequired = 0x000f0000,
			PrinterAllAccess = StandardRightsRequired | PrinterAdmin | PrinterUse
		}

		[StructLayout( LayoutKind.Sequential )]
		private struct PrinterDefaults
		{
			public IntPtr pDataType;
			public IntPtr pDevMode;
			public PrinterAccess DesiredAccess;
		}

		[DllImport( "winspool.drv", SetLastError = true )]
		private static extern bool OpenPrinter( string printerName, out IntPtr phPrinter, ref PrinterDefaults printerDefaults );

		[DllImport( "winspool.drv", SetLastError = true )]
		private static extern bool ClosePrinter( IntPtr phPrinter );

		[DllImport( "winspool.drv", SetLastError = true, CharSet = CharSet.Unicode )]
		private static extern bool XcvDataW( IntPtr hXcv,
		                                     string pszDataName,
		                                     IntPtr pInputData,
		                                     uint cbInputData,
		                                     out IntPtr pOutputData,
		                                     uint cbOutputData,
		                                     out uint pcbOutputNeeded,
		                                     out uint pdwStatus );

		public void AddPort( string portName, string monitorName )
		{
			IntPtr printerHandle;
			var defaults = new PrinterDefaults
			{
				pDataType = IntPtr.Zero,
				pDevMode = IntPtr.Zero,
				DesiredAccess = PrinterAccess.ServerAdmin
			};

			if( !OpenPrinter( ",XcvMonitor " + monitorName, out printerHandle, ref defaults ) )
			{
				////List<Monitor> monitors = GetInstalledMonitors();
				throw new Win32Exception( Marshal.GetLastWin32Error() );
			}

			var portData = new PortData { sztPortName = portName };
			var size = (uint) Marshal.SizeOf( portData );
			IntPtr pointer = Marshal.AllocHGlobal( (int) size );
			Marshal.StructureToPtr( portData, pointer, true );

			uint status;

			try
			{
				IntPtr outputData;
				uint outputNeeded;
				bool isSuccess = XcvDataW( printerHandle, "AddPort", pointer, size, out outputData, 0, out outputNeeded, out status );

				if( !isSuccess )
				{
					throw new Win32Exception( Marshal.GetLastWin32Error() );
				}
			}
			finally
			{
				ClosePrinter( printerHandle );
				Marshal.FreeHGlobal( pointer );
			}

			if( status == 0 )
			{
				return;
			}

			// HACK: Compensate for an incorrect error message from Windows.
			if( status == 183 )
			{
				// TODO: Rexamine this case closer.
				// Swallow this error and keep going.
				//throw new Win32Exception("Cannot create a file when that file already exists.");
				return;
			}

			throw new Win32Exception( Marshal.GetLastWin32Error() );
		}

		[DllImport( "winspool.drv", SetLastError = true, CharSet = CharSet.Auto )]
		private static extern int DeletePort( string pName, int hWnd, string pPortName );

		public void DeletePort( IntPtr parentWindow, string portName )
		{
			if( DeletePort( null, parentWindow.ToInt32(), portName ) == 0 )
			{
				throw new Win32Exception( Marshal.GetLastWin32Error() );
			}
		}

		// http://pinvoke.net/default.aspx/winspool.EnumPorts
		[DllImport( "winspool.drv", SetLastError = true, CharSet = CharSet.Auto )]
		private static extern bool EnumPorts( string pName,
		                                      uint level,
		                                      IntPtr pPorts,
		                                      uint cbBuf,
		                                      ref uint pcbNeeded,
		                                      ref uint pcReturned );

		[Flags]
		public enum PortType
		{
			Write = 0x1,
			Read = 0x2,
			Redirected = 0x4,
			NetAttached = 0x8
		}

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
		public struct PORT_INFO_2
		{
			[MarshalAs( UnmanagedType.LPTStr )]
			public string pPortName;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pMonitorName;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pDescription;

			public PortType fPortType;
			internal uint Reserved;
		}

		public class Port
		{
			public string name;
			public string monitorName;
			public string description;
		}

		public List<Port> GetInstalledPorts()
		{
			var ports = new List<Port>();
			uint pcbNeeded = 0;
			uint pcReturned = 0;

			if( EnumPorts( null, 2, IntPtr.Zero, 0, ref pcbNeeded, ref pcReturned ) )
			{
				//succeeds, but must not, because buffer is zero (too small)!
				throw new Exception( "EnumPorts should fail!" );
			}

			int lastWin32Error = Marshal.GetLastWin32Error();

			const int ERROR_INSUFFICIENT_BUFFER = 122;

			if( lastWin32Error != ERROR_INSUFFICIENT_BUFFER )
			{
				throw new Win32Exception( lastWin32Error );
			}

			IntPtr pPorts = Marshal.AllocHGlobal( (int) pcbNeeded );

			if( EnumMonitors( null, 2, pPorts, pcbNeeded, ref pcbNeeded, ref pcReturned ) )
			{
				IntPtr pIndex = pPorts;

				for( var i = 0; i < pcReturned; i++ )
				{
					var portStruct = (PORT_INFO_2) Marshal.PtrToStructure( pIndex, typeof( PORT_INFO_2 ) );

					ports.Add( new Port
					{
						name = portStruct.pPortName,
						monitorName = portStruct.pMonitorName,
						description = portStruct.pDescription
					} );

					// Increment index pointer to next struct.
					//pIndex = (IntPtr)(pIndex.ToInt32() + Marshal.SizeOf(typeof(MONITOR_INFO_2)));
					pIndex += Marshal.SizeOf( typeof( MONITOR_INFO_2 ) );
				}

				Marshal.FreeHGlobal( pPorts );

				return ports;
			}

			throw new Win32Exception( Marshal.GetLastWin32Error() );
		}

		// http://pinvoke.net/default.aspx/winspool.EnumPorts
		[DllImport( "winspool.drv", SetLastError = true, CharSet = CharSet.Auto )]
		private static extern bool EnumPrinterDrivers( string pName,
		                                               string pEnvironment,
		                                               uint level,
		                                               IntPtr pDriverInfo,
		                                               uint cbBuf,
		                                               ref uint pcbNeeded,
		                                               ref uint pcReturned );

		public struct DRIVER_INFO_2
		{
			public uint cVersion;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pName;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pEnvironment;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pDriverPath;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pDataFile;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pConfigFile;
		}

		public class Driver
		{
			public string name;
			public uint version;
			public string environment;
			public string driverPath;
			public string dataFile;
			public string configFile;
		}

		public List<Driver> GetInstalledDrivers()
		{
			var drivers = new List<Driver>();
			uint pcbNeeded = 0;
			uint pcReturned = 0;

			if( EnumPrinterDrivers( null, null, 2, IntPtr.Zero, 0, ref pcbNeeded, ref pcReturned ) )
			{
				// Succeeds, but must not, because buffer is zero (too small)!
				throw new Exception( "EnumDrivers should fail!" );
			}

			int lastWin32Error = Marshal.GetLastWin32Error();

			const int ERROR_INSUFFICIENT_BUFFER = 122;

			if( lastWin32Error != ERROR_INSUFFICIENT_BUFFER )
			{
				throw new Win32Exception( lastWin32Error );
			}

			IntPtr pDrivers = Marshal.AllocHGlobal( (int) pcbNeeded );

			if( !EnumPrinterDrivers( null, null, 2, pDrivers, pcbNeeded, ref pcbNeeded, ref pcReturned ) )
			{
				throw new Win32Exception( Marshal.GetLastWin32Error() );
			}

			IntPtr pIndex = pDrivers;

			for( var i = 0; i < pcReturned; i++ )
			{
				var driverStruct = (DRIVER_INFO_2) Marshal.PtrToStructure( pIndex, typeof( DRIVER_INFO_2 ) );

				drivers.Add( new Driver
				{
					name = driverStruct.pName,
					version = driverStruct.cVersion,
					environment = driverStruct.pEnvironment,
					driverPath = driverStruct.pDriverPath,
					dataFile = driverStruct.pDataFile,
					configFile = driverStruct.pConfigFile
				} );

				// Increment index pointer to next struct.
				//pIndex = (IntPtr)(pIndex.ToInt32() + Marshal.SizeOf(typeof(MONITOR_INFO_2)));
				pIndex += Marshal.SizeOf( typeof( DRIVER_INFO_2 ) );
			}

			Marshal.FreeHGlobal( pDrivers );

			return drivers;
		}

		[DllImport( "winspool.drv", SetLastError = true, CharSet = CharSet.Unicode )]
		private static extern bool GetPrinterDriverDirectory( StringBuilder pName,
		                                                      StringBuilder pEnv,
		                                                      int Level,
		                                                      [Out] StringBuilder outPath,
		                                                      int bufferSize,
		                                                      ref int Bytes );

		public string GetSystemDirectory()
		{
			var str = new StringBuilder( 1024 );
			var i = 0;

			GetPrinterDriverDirectory( null, null, 1, str, 1024, ref i );
			return str.ToString();
		}

		// API for Adding Printer Driver
		// http://msdn.microsoft.com/en-us/library/windows/desktop/dd183346(v=vs.85).aspx
		// http://pinvoke.net/default.aspx/winspool.DRIVER_INFO_2
		[DllImport( "winspool.drv", SetLastError = true, CharSet = CharSet.Auto )]
		private static extern int AddPrinterDriver( string pName, uint Level, ref DRIVER_INFO_3 pDriverInfo );

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
		private struct DRIVER_INFO_3
		{
			public uint cVersion;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pName;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pEnvironment;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pDriverPath;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pDataFile;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pConfigFile;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pHelpFile;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pDependentFiles;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pMonitorName;

			[MarshalAs( UnmanagedType.LPTStr )]
			public string pDefaultDataType;
		}

		public void AddPrinterDriver( string driverName,
		                              string driverFileMain,
		                              string driverFileData,
		                              string driverFileConfig,
		                              string driverFileHelp,
		                              string driverFileDependencies )
		{
			var di = new DRIVER_INFO_3
			{
				cVersion = 3,
				pName = driverName,
				pEnvironment = null,
				pDriverPath = driverFileMain,
				pDataFile = driverFileData,
				pConfigFile = driverFileConfig,
				pHelpFile = driverFileHelp,
				pDependentFiles = driverFileDependencies,
				pMonitorName = null,
				pDefaultDataType = "RAW"
			};

			if( AddPrinterDriver( null, 3, ref di ) == 0 )
			{
				throw new Win32Exception( Marshal.GetLastWin32Error() );
			}
		}

		[DllImport( "winspool.drv", SetLastError = true, CharSet = CharSet.Auto )]
		private static extern int DeletePrinterDriver( string pServerName, string pEnvironment, string pDriverName );

		public void DeletePrinterDriver( string driverName )
		{
			if( DeletePrinterDriver( null, null, driverName ) == 0 )
			{
				throw new Win32Exception( Marshal.GetLastWin32Error() );
			}
		}

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
		private struct PRINTER_INFO_2
		{
			public string pServerName;
			public string pPrinterName;
			public string pShareName;
			public string pPortName;
			public string pDriverName;
			public string pComment;
			public string pLocation;
			public IntPtr pDevMode;
			public string pSepFile;
			public string pPrintProcessor;
			public string pDatatype;
			public string pParameters;
			public IntPtr pSecurityDescriptor;
			public readonly uint Attributes;
			public readonly uint Priority;
			public readonly uint DefaultPriority;
			public readonly uint StartTime;
			public readonly uint UntilTime;
			public readonly uint Status;
			public readonly uint cJobs;
			public readonly uint AveragePPM;
		}

		[Flags]
		private enum PrinterEnumFlags
		{
			PRINTER_ENUM_DEFAULT = 0x00000001,
			PRINTER_ENUM_LOCAL = 0x00000002,
			PRINTER_ENUM_CONNECTIONS = 0x00000004,
			PRINTER_ENUM_FAVORITE = 0x00000004,
			PRINTER_ENUM_NAME = 0x00000008,
			PRINTER_ENUM_REMOTE = 0x00000010,
			PRINTER_ENUM_SHARED = 0x00000020,
			PRINTER_ENUM_NETWORK = 0x00000040,
			PRINTER_ENUM_EXPAND = 0x00004000,
			PRINTER_ENUM_CONTAINER = 0x00008000,
			PRINTER_ENUM_ICONMASK = 0x00ff0000,
			PRINTER_ENUM_ICON1 = 0x00010000,
			PRINTER_ENUM_ICON2 = 0x00020000,
			PRINTER_ENUM_ICON3 = 0x00040000,
			PRINTER_ENUM_ICON4 = 0x00080000,
			PRINTER_ENUM_ICON5 = 0x00100000,
			PRINTER_ENUM_ICON6 = 0x00200000,
			PRINTER_ENUM_ICON7 = 0x00400000,
			PRINTER_ENUM_ICON8 = 0x00800000,
			PRINTER_ENUM_HIDE = 0x01000000,
			PRINTER_ENUM_CATEGORY_ALL = 0x02000000,
			PRINTER_ENUM_CATEGORY_3D = 0x04000000
		}

		// http://pinvoke.net/default.aspx/winspool.EnumPrinters
		[DllImport( "winspool.drv", CharSet = CharSet.Auto, SetLastError = true )]
		private static extern bool EnumPrinters( PrinterEnumFlags Flags,
		                                         string Name,
		                                         uint Level,
		                                         IntPtr pPrinterEnum,
		                                         uint cbBuf,
		                                         ref uint pcbNeeded,
		                                         ref uint pcReturned );

		public class Printer
		{
			public string Name;
		}

		public List<Printer> GetInstalledPrinters()
		{
			var printers = new List<Printer>();
			uint pcbNeeded = 0;
			uint pcReturned = 0;
			var flags = PrinterEnumFlags.PRINTER_ENUM_LOCAL;

			if( EnumPrinters( flags, null, 2, IntPtr.Zero, 0, ref pcbNeeded, ref pcReturned ) )
			{
				//succeeds, but must not, because buffer is zero (too small)!
				throw new Exception( "EnumPrinters should fail!" );
			}

			int lastWin32Error = Marshal.GetLastWin32Error();

			const int ERROR_INSUFFICIENT_BUFFER = 122;

			if( lastWin32Error != ERROR_INSUFFICIENT_BUFFER )
			{
				throw new Win32Exception( lastWin32Error );
			}

			IntPtr pPrinters = Marshal.AllocHGlobal( (int) pcbNeeded );

			if( EnumPrinters( flags, null, 2, pPrinters, pcbNeeded, ref pcbNeeded, ref pcReturned ) )
			{
				IntPtr pIndex = pPrinters;

				for( var i = 0; i < pcReturned; i++ )
				{
					var printerStruct = (PRINTER_INFO_2) Marshal.PtrToStructure( pIndex, typeof( PRINTER_INFO_2 ) );

					printers.Add( new Printer { Name = printerStruct.pPrinterName } );

					// Increment index pointer to next struct.
					//pIndex = (IntPtr)(pIndex.ToInt32() + Marshal.SizeOf(typeof(MONITOR_INFO_2)));
					pIndex += Marshal.SizeOf( typeof( PRINTER_INFO_2 ) );
				}

				Marshal.FreeHGlobal( pPrinters );

				return printers;
			}

			throw new Win32Exception( Marshal.GetLastWin32Error() );
		}

		// API for Adding Printer
		// http://msdn.microsoft.com/en-us/library/windows/desktop/dd183343(v=vs.85).aspx
		[DllImport( "winspool.drv", SetLastError = true, CharSet = CharSet.Auto )]
		private static extern int AddPrinter( string pName, uint Level, [In] ref PRINTER_INFO_2 pPrinter );

		public void AddPrinter( string printerName, string portName, string driverName, string comment )
		{
			var pi = new PRINTER_INFO_2
			{
				pServerName = null,
				pPrinterName = printerName,
				pShareName = "",
				pPortName = portName,
				pDriverName = driverName, // "Apple Color LW 12/660 PS";
				pComment = comment,
				pLocation = "",
				pDevMode = new IntPtr( 0 ),
				pSepFile = "",
				pPrintProcessor = "WinPrint",
				pDatatype = "RAW", // NULL?
				pParameters = "",
				pSecurityDescriptor = new IntPtr( 0 )
			};

			if( AddPrinter( null, 2, ref pi ) == 0 )
			{
				throw new Win32Exception( Marshal.GetLastWin32Error() );
			}
		}

		////[DllImport( "winspool.drv", SetLastError = true, CharSet = CharSet.Auto )]
		////private static extern int SetPrinter( IntPtr phPrinter, uint level, ref PRINTER_INFO_2 pPrinter, uint command );

		////private const int PRINTER_CONTROL_PURGE = 3;

		[DllImport( "winspool.drv", SetLastError = true, CharSet = CharSet.Auto )]
		private static extern int DeletePrinter( IntPtr phPrinter );

		public void DeletePrinter( string printerName, string portName, string monitorName, string driverName, string comment )
		{
			IntPtr printerHandle;
			var defaults = new PrinterDefaults { DesiredAccess = PrinterAccess.Delete };

			//if (!OpenPrinter(",XcvMonitor " + monitorName, out printerHandle, ref defaults))
			if( !OpenPrinter( printerName, out printerHandle, ref defaults ) )
			{
				//throw new Win32Exception(Marshal.GetLastWin32Error(), string.Format("Could not open printer for the monitor port {0}", monitorName));
				//throw new Win32Exception(Marshal.GetLastWin32Error(), string.Format("Could not open printer {0}", printerName));
				throw new Win32Exception( Marshal.GetLastWin32Error() );
			}

			//PRINTER_INFO_2 pi = new PRINTER_INFO_2();

			//pi.pServerName = null;
			//pi.pPrinterName = printerName;
			//pi.pShareName = "";
			//pi.pPortName = portName;
			//pi.pDriverName = driverName;
			//pi.pComment = comment;
			//pi.pLocation = "";
			//pi.pDevMode = new IntPtr(0);
			//pi.pSepFile = "";
			//pi.pPrintProcessor = "WinPrint";
			//pi.pDatatype = "RAW";
			//pi.pParameters = "";
			//pi.pSecurityDescriptor = new IntPtr(0);

			//if (SetPrinter(printerHandle, 0, ref pi, PRINTER_CONTROL_PURGE) == 0)
			//{
			//	//throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not set printer");
			//  throw new Win32Exception(Marshal.GetLastWin32Error());
			//}

			if( DeletePrinter( printerHandle ) == 0 )
			{
				//throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not delete printer");
				throw new Win32Exception( Marshal.GetLastWin32Error() );
			}

			ClosePrinter( printerHandle );
			////if( !ClosePrinter( printerHandle ) )
			////{
			////	var result = Marshal.GetLastWin32Error();
			////}
		}

		public void StopSpoolService()
		{
			var sc = new ServiceController( SPOOLER_SERVICE_NAME );
			if( sc.Status != ServiceControllerStatus.Stopped || sc.Status != ServiceControllerStatus.StopPending )
			{
				sc.Stop();
			}

			sc.WaitForStatus( ServiceControllerStatus.Stopped );
		}

		public void StartSpoolService()
		{
			var sc = new ServiceController( SPOOLER_SERVICE_NAME );
			if( sc.Status != ServiceControllerStatus.Stopped && sc.Status != ServiceControllerStatus.StopPending )
			{
				return;
			}

			sc.Start();
			sc.WaitForStatus( ServiceControllerStatus.Running );
		}
	}
}
