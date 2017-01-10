using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Setup
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var options = new Options();

			// Parse the command line args.
			bool isArgs = CommandLine.Parser.Default.ParseArguments(args, options);

			// Create and run the new form passing it the parsed args.
			MainForm form = new MainForm(options);
			Application.Run(form);

			return form.returnCode;
		}
	}
}
