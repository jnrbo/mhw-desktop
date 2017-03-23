using System;

namespace MHWVirtualPrinter
{
	// type-safe-enum pattern
	public sealed class MhwPrinter
	{
		public const string APP_NAME = "myHEALTHwareDesktop";

		private readonly int value;

		public string PrinterName { get; private set; }
		public string PortName { get; private set; }
		public string MonitorName { get; private set; }
		public string DriverName { get; private set; }
		public string PipeRoot { get; private set; }

		// These are our usable types.
		public static readonly MhwPrinter PRINT_TO_DRIVE = new MhwPrinter( 1,
		                                                                   "myHEALTHware Drive",
		                                                                   "MHWDrive:",
		                                                                   "MHWDrive",
		                                                                   "myHEALTHware",
		                                                                   "mhw\\drive" );

		public static readonly MhwPrinter PRINT_TO_FAX = new MhwPrinter( 2,
		                                                                 "myHEALTHware Fax",
		                                                                 "MHWFax:",
		                                                                 "MHWFax",
		                                                                 "myHEALTHware",
		                                                                 "mhw\\fax" );

		// Private constructor prevents new types.
		private MhwPrinter( int value,
		                    string printerName,
		                    string portName,
		                    string monitorName,
		                    string driverName,
		                    string pipeRoot )
		{
			this.value = value;

			PrinterName = printerName;
			PortName = portName;
			MonitorName = monitorName;
			DriverName = driverName;
			PipeRoot = pipeRoot;
		}

		public string GetUserPipe( string userName = null )
		{
			return PipeRoot + "\\" + ( userName ?? Environment.UserName );
		}
	}
}