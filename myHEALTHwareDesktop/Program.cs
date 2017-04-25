using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using CommandLine;

namespace myHEALTHwareDesktop
{
	internal static class Program
	{
        private static Mutex _machineLocalAppInstanceMutex;

		/// <summary>
		///     The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main( string[] args )
		{

            //allow one instance per user.  
            string globalMutexName = string.Format(
            CultureInfo.InvariantCulture,
            "Global\\mhw-desktop~{0}~{1}~369198B1-4CDE-41F1-8E4C-F24336F0DD58",
            Environment.UserDomainName,
            Environment.UserName);
		    bool mutexIsNew;

            _machineLocalAppInstanceMutex = new System.Threading.Mutex(true, globalMutexName, out mutexIsNew);

            if (!mutexIsNew)
            {
                return;
            }


			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );

			var options = new Options();

			// Parse the command line args.
			Parser.Default.ParseArguments(args, options);
			Application.Run( new MhwDesktopForm( options ) );


		}

	}
}
