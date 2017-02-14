using System;
using System.Windows.Forms;
using myHEALTHwareDesktop.Properties;

namespace myHEALTHwareDesktop
{
	public partial class MhwMessageForm : Form
	{
		public MhwMessageForm( string title, string message, bool isError = false )
		{
			InitializeComponent();

			Text = title;
			messageLabel.Text = message;

			if( isError )
			{
				mhwLogoPictureBox.Image = Resources.mhw_logo_bw_224;
			}
		}

		private void OkButtonClick( object sender, EventArgs e )
		{
			Close();
		}

		private void MhwMessageFormShown( object sender, EventArgs e )
		{
			if( Owner != null )
			{
				Owner.Activate();
			}
			else
			{
				Activate();
			}
		}
	}
}
