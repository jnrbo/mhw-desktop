using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace myHEALTHwareDesktop
{
	internal static class Program
	{
		/// <summary>
		///     The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			// Make sure we are the only instance of this program running.
			Process instance = GetRunningInstance();

			// If we are the only running instance of this program then continue.
			if( instance == null )
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault( false );
				Application.Run( new MhwDesktopForm() );
			}
			else
			{
				// We are not the only running instance of this program so
				// Bring to focus the current running process with our name
				// and we will go away without doing anything more.
				ShowWindowAsync( instance.MainWindowHandle, WS_SHOWNORMAL );
				SetForegroundWindow( instance.MainWindowHandle );
			}
		}

		// Look at all currently running processes and see if there is already one of
		// us running with the name of myHEALTHwareDesktop.exe
		public static Process GetRunningInstance()
		{
			Process current = Process.GetCurrentProcess();
			Process[] processes = Process.GetProcessesByName( current.ProcessName );

			return
				processes.FirstOrDefault(
					p => p.Id != current.Id && Assembly.GetExecutingAssembly().Location.Replace( "/", "\\" ) == p.MainModule.FileName );
		}

		[DllImport( "User32.dll" )]
		private static extern bool ShowWindowAsync( IntPtr hWnd, int cmdShow );

		[DllImport( "User32.dll" )]
		private static extern bool SetForegroundWindow( IntPtr hWnd );

		private const int WS_SHOWNORMAL = 1;
	}
}
