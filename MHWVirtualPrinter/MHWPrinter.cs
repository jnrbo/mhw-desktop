using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWVirtualPrinter
{
	// type-safe-enum pattern
	public sealed class MHWPrinter
	{
		private readonly int value;
		public const string AppName = @"myHEALTHwareDesktop";
		public readonly string PrinterName;
		public readonly string PortName;
		public readonly string MonitorName;
		public readonly string DriverName;

		// These are our usable types.
		public static readonly MHWPrinter PrintToDrive = new MHWPrinter(1, "myHEALTHware Drive", "MHWDrive:", "MHWDrive", "myHEALTHware");
		public static readonly MHWPrinter PrintToFax = new MHWPrinter(2, "myHEALTHware Fax", "MHWFax:", "MHWFax", "myHEALTHware");

		// Private constructor prevents new types.
		private MHWPrinter(int value, string printerName, string portName, string monitorName, string driverName)
		{
			this.value = value;
			PrinterName = printerName;
			PortName = portName;
			MonitorName = monitorName;
			DriverName = driverName;
		}
	}
}