using CommandLine;
using CommandLine.Text;

namespace myHEALTHwareDesktop
{
	public class Options
	{
		[Option( 's', "Indicates auto-run at startup", DefaultValue = false, HelpText = "Used to indicate application was launched at startup."
		)]
		public bool AutoStarted { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild( this, current => HelpText.DefaultParsingErrorsHandler( this, current ) );
		}
	}
}
