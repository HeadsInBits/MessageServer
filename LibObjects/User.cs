using System;
using System.Data.SqlClient;

namespace MessageServer.Data
{
	public class User
	{
		private string _userName;
		private bool _isValidated;
		public int WebSocketID;

		public User(string userName, bool isValidated)
		{
			_userName = userName;
		}

		public string GetUserName()
		{
			return _userName;
		}

		public bool isValidateAccount()
		{
			return _isValidated;
		}

		public override string ToString()
		{
			return this._userName;
		}

	}
}