using System.IO;

namespace MHWVirtualPrinter
{
	public class MhwFile
	{
		public MhwFile( string name, Stream content )
		{
			Name = name;
			Content = content;
		}

		public string Name { get; private set; }
		public Stream Content { get; private set; }
	}
}