using myHEALTHwareDesktop.Properties;

namespace myHEALTHwareDesktop
{
	public class Credentials
	{
		public const string APP_ID = "0CCED62C-808D-4D71-A8B2-AEA20C193263";
		public const string APP_SECRET = "5B8CCD2F-03D6-4161-A7F2-C112EF79B1A1";

		private readonly Settings settings;

		public string AppId
		{
			get { return APP_ID; }
		}

		public string AppSecret
		{
			get { return APP_SECRET; }
		}

		public string ConnectionId { get; private set; }
		public string AccessToken { get; private set; }

		internal Credentials( string connectionId, string accessToken, Settings settings )
		{
			ConnectionId = connectionId;
			AccessToken = accessToken;

			this.settings = settings;
		}

		public void Save()
		{
			settings.AccessToken = AccessToken;
			settings.ConnectionId = ConnectionId;
			settings.Save();
		}
	}
}