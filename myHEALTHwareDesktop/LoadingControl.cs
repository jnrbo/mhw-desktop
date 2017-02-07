using System;
using System.Linq;
using System.Windows.Forms;

namespace myHEALTHwareDesktop
{
	public partial class LoadingControl : UserControl
	{
		private int count = 2;

		public LoadingControl()
		{
			InitializeComponent();
			UpdateLoadingMessage();
		}

		private void TimerTick( object sender, EventArgs e )
		{
			UpdateLoadingMessage();
		}

		private void UpdateLoadingMessage()
		{
			count = count % 3 + 1;
			loadingLabel.Text = "Loading" + string.Join( "", Enumerable.Repeat( '.', count ) );
		}

		public void OnLoadingFinished()
		{
			timer.Enabled = false;
			Visible = false;
		}
	}
}
