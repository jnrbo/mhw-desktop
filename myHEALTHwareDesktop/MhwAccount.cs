using System.Drawing;

namespace myHEALTHwareDesktop
{
	public class MhwAccount
	{
		public string Name { get; set; }
		public string AccountId { get; set; }
		public string PictureFileId { get; set; }
		public Image ProfilePic { get; set; }
		public bool IsPersonalAccount { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}