using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace LibObjects
{
    public class User : IEquatable<User>
    {

        protected class UserJsonData
        {
            public string UserName;
            public bool IsValidated;
            public Guid Guid;
        }

        protected UserJsonData GetJsonDataFromUser()
        {
            UserJsonData json = new UserJsonData
            {
                UserName = _userName,
                IsValidated = _isValidated,
                Guid = _guid,
            };
            return json;
        }

        protected User(UserJsonData json)
        {
            _userName = json.UserName;
            _isValidated = json.IsValidated;
            _guid = json.Guid;
        }


        protected string _userName;
        protected bool _isValidated;
        protected Guid _guid = Guid.NewGuid();

        protected User() { }

        public static readonly int NumberOfUsersToSendInMessage = 20;

        public virtual bool Equals(User other)
        {
            if (other._isValidated.ToString() == _isValidated.ToString() && other.GetUserName() == _userName && other.GetUserGuid() == _guid)
            {
                return true;
            }
            else
            {
                return false;
            }

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
            return _userName;
        }

        //TODO: NOW SERIALISATION AND DESERIALIZATION IS HAPPENING HERE WE COULD:
        //1. CHANGE THE FORMAT
        //2. VALIDATE ALL IN ONE PLACE
        //3. RESTRICT DATA BEING PASSED
        //4. OPTIMISE FOR DATA SIZE
        //5. ENCRYPT?


        public static User GetUserFromJson(string jsonString)
        {
            return new User(JsonConvert.DeserializeObject<UserJsonData>(jsonString));
        }

        public static string GetJsonFromUser(User user)
        {
            return JsonConvert.SerializeObject(user.GetJsonDataFromUser(), Formatting.Indented);
        }

        public static List<User> GetUsersListFromJson(string jsonData)
        {
            List<UserJsonData> usersJsonDataList = JsonConvert.DeserializeObject<List<UserJsonData>>(jsonData);
            List<User> users = new List<User>();
            foreach (var data in usersJsonDataList)
            {
                users.Add(new User(data));
            }
            return users;
        }

        public static string GetJsonFromUsersList(List<User> users)
        {
            List<UserJsonData> userJsonDataList = new List<UserJsonData>();
            foreach (var user in users)
            {
                userJsonDataList.Add(user.GetJsonDataFromUser());
            }
            return JsonConvert.SerializeObject(userJsonDataList, Formatting.Indented);
        }

        public Guid GetUserGuid()
        {
            return _guid;
        }

    }
}