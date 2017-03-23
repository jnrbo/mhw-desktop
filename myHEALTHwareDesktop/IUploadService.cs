using System.IO;
using System.Threading.Tasks;

namespace myHEALTHwareDesktop
{
	public interface IUploadService
	{
		Task<string> UploadFile( string fullPath, string name, string uploadFolderDriveItemId );
		Task<string> Upload( Stream stream, string name, string uploadFolderDriveItemId );
	}
}