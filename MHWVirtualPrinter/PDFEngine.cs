using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace MHWVirtualPrinter
{
	public class PDFEngine
	{
		public string rootPath;
		public string binPath;
		public string libPath;
		public string exe;
		public string pathExe { get { return Path.Combine(binPath, exe); } }
		public string dll;
		public string pathDll { get { return Path.Combine(binPath, dll); } }

		public PDFEngine(string currentDirectory, string platform)
		{
			if (platform == "x64")
			{
				exe = "gswin64c.exe";
				dll = "gsdll64.dll";
			}
			else
			{
				exe = "gswin32c.exe";
				dll = "gsdll32.dll";
			}

			rootPath = Path.Combine(currentDirectory, platform, "GhostScript");
			binPath = Path.Combine(rootPath, "bin");
			libPath = Path.Combine(rootPath, "lib");
		}

		// Configure GhostScript
		public PDFEngine Install()
		{
			string keyName = "SOFTWARE\\GPL Ghostscript\\9.19";
			RegistryKey regKey = Registry.LocalMachine.OpenSubKey(keyName, true);

			if (regKey == null)
			{
				// Create the registry entries.
				regKey = Registry.LocalMachine.CreateSubKey(keyName);
			}
			else
			{
				// Already installed.
				// TODO:
			}

			regKey.SetValue("GS_DLL", this.pathDll);
			regKey.SetValue("GS_LIB", String.Format("{0};{1}", this.binPath, this.libPath));
			regKey.Close();

			keyName = "SOFTWARE\\Artifex\\GPL Ghostscript\\9.19";
			regKey = Registry.LocalMachine.OpenSubKey(keyName, true);

			if (regKey == null)
			{
				// Create the registry entry.
				regKey = Registry.LocalMachine.CreateSubKey(keyName);
			}
			else
			{
				// Already installed.
				// TODO:
			}

			regKey.SetValue("", this.rootPath);
			regKey.Close();

			return this;
		}
	}
}
