using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Setup
{
	public class Log
	{
		private ListView listViewLog;
		private ColumnHeader columnHeaderMessage;

		// Must always pass in ListView.
		private Log()
		{
		}

		public Log(ListView logView, ColumnHeader columnHeaderMessage)
		{
			listViewLog = logView;
			this.columnHeaderMessage = columnHeaderMessage;
		}

		public void Info(string format, params object[] list)
		{
			Add(String.Format(format, list), Color.Black);
		}

		public void Warning(string format, params object[] list)
		{
			Add(String.Format(format, list), Color.Orange);
		}

		public void Error(string format, params object[] list)
		{
			Add(String.Format(format, list), Color.Red);
		}

		public void Success(string format, params object[] list)
		{
			Add(String.Format(format, list), Color.Green);
		}

		// Avoid cross-thread exceptions by making sure we call back to UI thread from this thread properly.
		private void Add(string message, Color color)
		{
			// When the control isn't visible yet, InvokeRequired returns false,
			// resulting still in a cross-thread exception.
			while (!listViewLog.Visible)
			{
				System.Threading.Thread.Sleep(50);
			}

			if (listViewLog.InvokeRequired)
			{
				listViewLog.Invoke(new MethodInvoker(() => { Add(message, color); }));
			}
			else
			{
				ListViewItem item = listViewLog.Items.Add(message);
				item.ForeColor = color;
				item.EnsureVisible();
			}
		}

		public void AdjustLogWidth(int size)
		{
			if (listViewLog.InvokeRequired)
			{
				listViewLog.Invoke(new MethodInvoker(() => { AdjustLogWidth(size); }));
			}
			else
			{
				columnHeaderMessage.Width = size;
			}
		}
	}
}