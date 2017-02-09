namespace myHEALTHwareDesktop
{
	public interface IUploadService
	{
		string UploadFile( string fullPath, string name, string uploadFolderDriveItemId );
	}
}