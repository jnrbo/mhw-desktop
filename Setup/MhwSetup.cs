using System.Diagnostics;
using System.Windows.Forms;

namespace Setup
{
	public class MhwSetup
	{
		public static void LaunchAndWaitForExit( string setupArgs )
		{
			// Launch setup process.
			var process = new Process
			{
				StartInfo =
					new ProcessStartInfo
					{
						FileName = "Setup.exe",
						Arguments = setupArgs,
						WorkingDirectory = GetApplicationDirectory()
					}
			};

			process.Start();
			process.WaitForExit();
		}

		private static string GetApplicationDirectory()
		{
			return Application.StartupPath;
		}
	}
}