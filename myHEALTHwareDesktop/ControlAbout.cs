using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace myHEALTHwareDesktop
{
	public partial class ControlAbout : UserControl
	{
		private const string urlGPL = "https://www.gnu.org/licenses/old-licenses/gpl-2.0.html";
		private const string urlPortMonitor = "https://sourceforge.net/projects/mfilemon/";
		private const string urlGhostscript = "http://www.ghostscript.com/";
		private const string urlCommandLine = "https://commandline.codeplex.com/";
		private const string urlCEFSharp = "https://cefsharp.github.io/";
		private const string urlRestSharp = "http://restsharp.org/";

		public ControlAbout()
		{
			InitializeComponent();
		}

		private void ControlAbout_Load(object sender, EventArgs e)
		{
			this.toolTipPortMonitor.SetToolTip(this.linkLicense, urlGPL);
			this.toolTipPortMonitor.SetToolTip(this.linkmfilemon, urlPortMonitor);
			this.toolTipPortMonitor.SetToolTip(this.linkGostScript, urlGhostscript);
			this.toolTipPortMonitor.SetToolTip(this.linkCommandLine, urlCommandLine);
			this.toolTipPortMonitor.SetToolTip(this.linkLabelCEFSharp, urlCEFSharp);
			this.toolTipPortMonitor.SetToolTip(this.linkLabelRestSharp, urlRestSharp);
		}

		private void aboutGNULicense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(urlGPL);
		}

		private void aboutMFileMon_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(urlPortMonitor);
		}

		private void aboutGhostScript_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(urlGhostscript);
		}

		private void aboutCommandLine_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(urlCommandLine);
		}

		private void aboutCEFSharp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(urlCEFSharp);
		}

		private void aboutRestSharp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(urlRestSharp);
		}
	}
}
