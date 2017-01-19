using System.IO;
using Microsoft.Win32;

namespace MHWVirtualPrinter
{
	public class PdfEngine
	{
		private readonly string rootPath;
		private readonly string binPath;
		private readonly string libPath;
		private readonly string dll;
		private readonly string exe;

		public string PathExe
		{
			get { return Path.Combine( binPath, exe ); }
		}

		private string PathDll
		{
			get { return Path.Combine( binPath, dll ); }
		}

		public PdfEngine( string currentDirectory, string platform )
		{
			if( platform == "x64" )
			{
				exe = "gswin64c.exe";
				dll = "gsdll64.dll";
			}
			else
			{
				exe = "gswin32c.exe";
				dll = "gsdll32.dll";
			}

			rootPath = Path.Combine( currentDirectory, platform, "GhostScript" );
			binPath = Path.Combine( rootPath, "bin" );
			libPath = Path.Combine( rootPath, "lib" );
		}

		// Configure GhostScript
		public PdfEngine Install()
		{
			var keyName = "SOFTWARE\\GPL Ghostscript\\9.19";
			RegistryKey regKey = Registry.LocalMachine.OpenSubKey( keyName, true ) ??
			                     Registry.LocalMachine.CreateSubKey( keyName );

			regKey.SetValue( "GS_DLL", PathDll );
			regKey.SetValue( "GS_LIB", string.Format( "{0};{1}", binPath, libPath ) );
			regKey.Close();

			keyName = "SOFTWARE\\Artifex\\GPL Ghostscript\\9.19";
			regKey = Registry.LocalMachine.OpenSubKey( keyName, true ) ?? Registry.LocalMachine.CreateSubKey( keyName );

			regKey.SetValue( "", rootPath );
			regKey.Close();

			return this;
		}
	}
}
