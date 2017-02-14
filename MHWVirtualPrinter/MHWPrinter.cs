namespace MHWVirtualPrinter
{
	// type-safe-enum pattern
	public sealed class MhwPrinter
	{
		public const string APP_NAME = "myHEALTHwareDesktop";

		private readonly int value;
		public readonly string PrinterName;
		public readonly string PortName;
		public readonly string MonitorName;
		public readonly string DriverName;

		// These are our usable types.
		public static readonly MhwPrinter PRINT_TO_DRIVE = new MhwPrinter( 1,
		                                                                   "myHEALTHware Drive",
		                                                                   "MHWDrive:",
		                                                                   "MHWDrive",
		                                                                   "myHEALTHware" );

		public static readonly MhwPrinter PRINT_TO_FAX = new MhwPrinter( 2,
		                                                                 "myHEALTHware Fax",
		                                                                 "MHWFax:",
		                                                                 "MHWFax",
		                                                                 "myHEALTHware" );

		// Private constructor prevents new types.
		private MhwPrinter( int value, string printerName, string portName, string monitorName, string driverName )
		{
			this.value = value;
			PrinterName = printerName;
			PortName = portName;
			MonitorName = monitorName;
			DriverName = driverName;
		}
	}
}