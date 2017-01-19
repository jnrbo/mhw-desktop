using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Setup
{
	public class Log
	{
		private readonly ListView listViewLog;
		private readonly ColumnHeader columnHeaderMessage;

		public Log( ListView logView, ColumnHeader columnHeaderMessage )
		{
			listViewLog = logView;
			this.columnHeaderMessage = columnHeaderMessage;
		}

		public void Info( string format, params object[] list )
		{
			Add( string.Format( format, list ), Color.Black );
		}

		public void Warning( string format, params object[] list )
		{
			Add( string.Format( format, list ), Color.Orange );
		}

		public void Error( string format, params object[] list )
		{
			Add( string.Format( format, list ), Color.Red );
		}

		public void Success( string format, params object[] list )
		{
			Add( string.Format( format, list ), Color.Green );
		}

		// Avoid cross-thread exceptions by making sure we call back to UI thread from this thread properly.
		private void Add( string message, Color color )
		{
			// When the control isn't visible yet, InvokeRequired returns false,
			// resulting still in a cross-thread exception.
			while( !listViewLog.Visible )
			{
				Thread.Sleep( 50 );
			}

			if( listViewLog.InvokeRequired )
			{
				listViewLog.Invoke( new MethodInvoker( () => { Add( message, color ); } ) );
			}
			else
			{
				ListViewItem item = listViewLog.Items.Add( message );
				item.ForeColor = color;
				item.EnsureVisible();
			}
		}

		public void AdjustLogWidth( int size )
		{
			if( listViewLog.InvokeRequired )
			{
				listViewLog.Invoke( new MethodInvoker( () => { AdjustLogWidth( size ); } ) );
			}
			else
			{
				columnHeaderMessage.Width = size;
			}
		}
	}
}