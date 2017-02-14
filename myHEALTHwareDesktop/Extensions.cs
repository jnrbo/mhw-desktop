using System;
using System.Diagnostics.Contracts;
using System.Windows.Forms;

namespace myHEALTHwareDesktop
{
	public static class Extensions
	{
		/// <summary>Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.</summary>
		/// <param name="control">the control for which the update is required</param>
		/// <param name="action">action to be performed on the control</param>
		public static void InvokeOnUiThreadIfRequired( this Control control, Action action )
		{
			if( control.InvokeRequired )
			{
				control.BeginInvoke( action );
			}
			else
			{
				action.Invoke();
			}
		}

		public static string TrimWithEllipsis( this string source, int length )
		{
			if( length < 4 )
			{
				throw new ArgumentOutOfRangeException( "length" );
			}
			Contract.EndContractBlock();

			if( string.IsNullOrEmpty( source ) )
			{
				return source;
			}

			bool useEllipsis = source.Trim().Length > length;
			string result = source.Substring( 0, Math.Min( source.Length, length ) ).TrimEnd();
			if( result.Length >= 4 && useEllipsis )
			{
				result = result.Substring( 0, result.Length - 3 ).TrimEnd() + "...";
			}

			return result;
		}
	}
}