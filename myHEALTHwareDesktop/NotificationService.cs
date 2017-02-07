namespace myHEALTHwareDesktop
{
	public interface INotificationService
	{
		void ShowBalloonError( string message, params object[] list );
		void ShowBalloonWarning( string message, params object[] list );
		void ShowBalloonInfo( string message, params object[] list );
		void NotifyIfNetworkUnavailable();
	}
}