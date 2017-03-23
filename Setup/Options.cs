using CommandLine;
using CommandLine.Text;

namespace Setup
{
	public class Options
	{
		[Option( 'a', "Ran from App", DefaultValue = false, HelpText = "Should only be set by app and not when ran manually."
		)]
		public bool RanFromApp { get; set; }

		[Option( 'u', "Uninstall", DefaultValue = false,
			HelpText =
				"Perform uninstall of selected flags. Defaults to false which is perform install. Requires other flags to be specified."
		)]
		public bool Uninstall { get; set; }

		[Option( 's', "Run at System Startup", DefaultValue = false,
			HelpText = "Install or uninstall 'run at system startup' flag." )]
		public bool RunAtSystemStartup { get; set; }

		[Option( 'd', "Print to Drive", DefaultValue = true,
			HelpText = "Install or uninstall the print to Drive print driver." )]
		public bool PrintToDrive { get; set; }

		[Option( 'f', "Print to Fax", DefaultValue = false,
			HelpText = "Install or uninstall the print to Fax print driver. Will be ingnored if -d specified." )]
		public bool PrintToFax { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild( this, current => HelpText.DefaultParsingErrorsHandler( this, current ) );
		}
	}
}
