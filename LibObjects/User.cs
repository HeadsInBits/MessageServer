using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace MessageServer.Data
{
	public class User
	{
		public string _userName;
		public bool _isValidated;
		public int WebSocketID;
		public Guid id;

		public User(string userName, bool isValidated, Guid id)
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
		
		//TODO: NOW SERIALISATION AND DESERIALIZATION IS HAPPENING HERE WE COULD:
		//1. CHANGE THE FORMAT
		//2. VALIDATE ALL IN ONE PLACE
		//3. RESTRICT DATA BEING PASSED
		//4. OPTIMISE FOR DATA SIZE
		//5. ENCRYPT?


		public static User GetUserFromJson(string JsonString)
		{
			return JsonConvert.DeserializeObject<User>(JsonString);
		}
		
		public static string GetJsonFromUser(User user)
		{
			return JsonConvert.SerializeObject(user, Formatting.Indented);
		}
		
		public static List<User> GetUsersListFromJson(string jsonData)
		{
			return JsonConvert.DeserializeObject<List<User>>(jsonData);
		}
		
		public static string GetJsonFromUsersList(List<User> users)
		{
			return JsonConvert.SerializeObject(users, Formatting.Indented);
		}

		public Guid GetUserID()
		{
			throw new NotImplementedException();
		}
	}
}