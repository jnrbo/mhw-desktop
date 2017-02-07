using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using myHEALTHwareDesktop.Properties;
using SOAPware.PortalApi.Models;
using SOAPware.PortalSdk;
using SOAPware.PortalSDK;

namespace myHEALTHwareDesktop
{
	public class ActiveUserSession
	{
		private static readonly ActiveUserSession INSTANCE = new ActiveUserSession();
		private List<MhwAccount> accounts;

		public event EventHandler ActingAsChanged;

		public MhwAccount LoggedInAccount { get; private set; }
		public MhwAccount ActingAsAccount { get; private set; }
		public Credentials Credentials { get; private set; }
		public MhwSdk Sdk { get; private set; }

		public bool IsLoggedIn
		{
			get { return LoggedInAccount != null; }
		}

		internal Settings Settings
		{
			get { return Settings.Default; }
		}

		private ActiveUserSession()
		{
		}

		public static ActiveUserSession GetInstance()
		{
			return INSTANCE;
		}

		public void SetActingAsAccount( MhwAccount account )
		{
			if( ActingAsAccount == account )
			{
				return;
			}

			ActingAsAccount = account;

			Settings.SelectedAccountId = account != null ? account.AccountId : null;
			Settings.Save();

			OnActingAsChanged();
		}

		public async void Login( string connectionId, string accessToken )
		{
			if( connectionId == null )
			{
				throw new ArgumentNullException( "connectionId", "Invalid login credential" );
			}
			if( accessToken == null )
			{
				throw new ArgumentNullException( "accessToken", "Invalid login credential" );
			}
			Contract.EndContractBlock();

			Logout();

			// SDK expects "/api" to already be on the end of the domain.
			string domainApi = string.Format( "{0}/api", Settings.myHEALTHwareDomain );

			// If credentials we have are invalid, change login.
			Sdk = new MhwSdk( false,
			                  // SDK logging has an issue. Keep off.
			                  Settings.sdkTimeOutSeconds * 1000,
			                  domainApi,
			                  Credentials.APP_ID,
			                  Credentials.APP_SECRET,
			                  connectionId,
			                  accessToken );

			LoggedInAccount = AuthenticateAccount();

			Credentials = new Credentials( connectionId, accessToken, Settings );
			Credentials.Save();

			await GetMhwAccountsAsync();

			var actingAs = accounts.FirstOrDefault( p => p.AccountId == Settings.SelectedAccountId ) ?? LoggedInAccount;
			SetActingAsAccount( actingAs );
		}

		private MhwAccount AuthenticateAccount()
		{
			ApiAccount apiAccount = Sdk.Account.Get();

			return new MhwAccount
					{
						Name = apiAccount.DisplayName,
						AccountId = apiAccount.AccountId,
						PictureFileId = apiAccount.PictureFileId,
						IsPersonalAccount = true
					};
		}

		public void Logout()
		{
			LoggedInAccount = null;
			SetActingAsAccount( null );
			Credentials = null;

			Settings.AccessToken = null;
			Settings.ConnectionId = null;
			Settings.Save();
		}

		public async Task<IEnumerable<MhwAccount>> GetMhwAccountsAsync()
		{
			if( !IsLoggedIn )
			{
				throw new InvalidOperationException( "A user must be logged in to retrieve acting-as account list" );
			}

			List<ApiAccountConnection> connections = await Sdk.Account.GetConnectionsAsync( LoggedInAccount.AccountId,
			                                                                                Constants.Permissions.ManageFiles );

			accounts = new List<MhwAccount> { LoggedInAccount };

			accounts.AddRange(
				connections.Select(
					c =>
						new MhwAccount
						{
							Name = c.Yours.DisplayName,
							AccountId = c.Yours.AccountId,
							PictureFileId = c.Yours.PictureFileId,
							IsPersonalAccount = false
						} ).OrderBy( p => p.Name ) );

			Parallel.ForEach( accounts,
			                  p =>
			                  {
				                  if( p.PictureFileId != null )
				                  {
					                  Stream picStream = Sdk.Account.GetFile( p.AccountId, p.PictureFileId );
					                  p.ProfilePic = new Bitmap( picStream, false );
				                  }
				                  else
				                  {
					                  p.ProfilePic = Resources.DefaultAvatar;
				                  }
			                  } );

			return accounts;
		}

		protected virtual void OnActingAsChanged()
		{
			EventHandler handler = ActingAsChanged;
			if( handler != null )
			{
				handler( this, EventArgs.Empty );
			}
		}

		////public class SelectedAccountChangedEventArgs : EventArgs
		////{
		////	public SelectedAccountChangedEventArgs( MhwAccount selectedAccount )
		////	{
		////		SelectedAccount = selectedAccount;
		////	}

		////	public MhwAccount SelectedAccount { get; private set; }
		////}
	}
}