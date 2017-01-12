using System.Drawing;

namespace myHEALTHwareDesktop
{
	public class MhwAccount
	{
		public string Name { get; set; }
		public string DelegateAccountId { get; set; }
		public string PictureFileId { get; set; }
		public Image ProfilePic { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}