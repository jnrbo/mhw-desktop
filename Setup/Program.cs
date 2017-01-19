using System;
using System.Windows.Forms;
using CommandLine;

namespace Setup
{
	internal static class Program
	{
		/// <summary>
		///     The main entry point for the application.
		/// </summary>
		[STAThread]
		private static int Main( string[] args )
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );

			var options = new Options();

			// Parse the command line args.
			Parser.Default.ParseArguments( args, options );

			// Create and run the new form passing it the parsed args.
			var form = new MainForm( options );
			Application.Run( form );

			return form.ReturnCode;
		}
	}
}
