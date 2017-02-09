using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace myHEALTHwareDesktop
{
	public partial class ControlAbout : UserControl
	{
		private const string URL_GPL = "https://www.gnu.org/licenses/agpl-3.0.en.html";
		private const string URL_PORT_MONITOR = "https://sourceforge.net/projects/mfilemon/";
		private const string URL_GHOSTSCRIPT = "http://www.ghostscript.com/";
		private const string URL_COMMAND_LINE = "https://commandline.codeplex.com/";
		private const string URL_CEF_SHARP = "https://cefsharp.github.io/";
		private const string URL_REST_SHARP = "http://restsharp.org/";

		public ControlAbout()
		{
			InitializeComponent();
		}

		private void ControlAboutLoad( object sender, EventArgs e )
		{
			toolTipPortMonitor.SetToolTip( linkLabelLicense, URL_GPL );
			toolTipPortMonitor.SetToolTip( linkLabelMfilemon, URL_PORT_MONITOR );
			toolTipPortMonitor.SetToolTip( linkLLabelGhostScript, URL_GHOSTSCRIPT );
			toolTipPortMonitor.SetToolTip( linkLabelCommandLine, URL_COMMAND_LINE );
			toolTipPortMonitor.SetToolTip( linkLabelCefSharp, URL_CEF_SHARP );
			toolTipPortMonitor.SetToolTip( linkLabelRestSharp, URL_REST_SHARP );
		}

		private void AboutGnuLicenseLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			Process.Start( URL_GPL );
		}

		private void AboutMFilemonLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			Process.Start( URL_PORT_MONITOR );
		}

		private void AboutGhostScriptLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			Process.Start( URL_GHOSTSCRIPT );
		}

		private void AboutCommandLineLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			Process.Start( URL_COMMAND_LINE );
		}

		private void AboutCefSharpLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			Process.Start( URL_CEF_SHARP );
		}

		private void AboutRestSharpLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			Process.Start( URL_REST_SHARP );
		}
	}
}
