using System.Text;
using global::System;
using global::System.Collections.Generic;

namespace NetworkObjects
{

    public static class ProcessMessageData
    {
        private const string splitter = ":*:";
        private static readonly Dictionary<string, (string remove, string replace)> SafeConvert =
            new Dictionary<string, (string remove, string replace)>()
            {
                { splitter, (splitter, "") },
            };

        //"[STRING]:[ROOM_LIST_JSON]"
        public static List<Room> GetRoomsListFromMessageFormatStringJsonRooms(string[] messageChunks)
        {
            string jsonData = messageChunks[1];
            List<Room> jsonDe = Room.GetRoomListFromJson(jsonData);
            return jsonDe;
        }

        //"[STRING]:[USER_JSON]:[STRING]"
        public static string GetUserMessageFromMessageFormatStringJsonUserString(string[] messageChunks, out User user)
        {
            var messageString = messageChunks[2];
            string jsonStrUser = messageChunks[1];
            user = User.GetUserFromJson(jsonStrUser);
            return messageString;
        }
        
        //"[STRING]:[USER_JSON]:[ROOM_JSON]"
        public static Room GetUserRoomFromMessageFormatStringJsonUserJsonRoom(string[] messageChunks, out User user)
        {
            Room fromJson = Room.GetRoomFromJson(messageChunks[2]);
            string jsonStrUser = messageChunks[1];
            user = User.GetUserFromJson(jsonStrUser);
            return fromJson;
        }
        
        //"[STRING]:[ROOM_JSON]:[STRING]"
        public static string GetRoomAndMessageFromMessageFormatStringJsonRoomString(
            string[] messageChunks, out Room room)
        {
            var messageString = messageChunks[2];
            room = Room.GetRoomFromJson(messageChunks[1]);
            return messageString;
        }

        //"[STRING]:[USER_LIST_JSON]"
        public static List<User> GetUsersFromMessageFormatStringJsonUserList(string[] messageChunks)
        {
            string jsonStrUsers = messageChunks[1];
            return User.GetUsersListFromJson(jsonStrUsers);
        }
        
        //"[STRING]:[USER_JSON]"
        public static User GetUserFromMessageFormatStringJsonUser(string[] messageChunks)
        {
            string users = messageChunks[1];
            return User.GetUserFromJson(users);
        }

        //"[STRING]:[ROOM_JSON] 
        public static Room GetRoomFromMessageFormatStringRoomJson(string[] messageChunks)
        {
            string jsonString = messageChunks[1];
            Room room = Room.GetRoomFromJson(jsonString);
            return room;
        }
        
        //"[STRING]:[UserID_GUID]:[ROOM_JSON]:[MESSAGE_STRING]"
        public static string GetRoomUserAndMessageFromFormatStringUserJsonRoomJsonMessage( string[] messageChunks, out Room room, out User userFromRoom)
        {
            userFromRoom = User.GetUserFromJson(messageChunks[1]);
            string roomMessageString = messageChunks[3];
            string jsonStrRoom = messageChunks[2];
            room = Room.GetRoomFromJson(jsonStrRoom);
            return roomMessageString;
        }
        
        //"[STRING]:[ROOM_GUID]:[USER_JSON]"
        public static User GetUserAndGuidFromFormatStringGuidUserJson(string[] messageChunks,
            out Guid guidJoined)
        {
            User joinedUser = User.GetUserFromJson(messageChunks[2]);
            guidJoined = Guid.Parse(messageChunks[1]);
            return joinedUser;
        }
        
        //"[STRING]:[ROOM_GUID]:[USER_LIST_JSON]"
        public static List<User> GetUserListAndRoomGuidFromFormatStringGuidUserListJson(string[] messageChunks,
            out Guid guidRoom)
        {
            List<User> users = User.GetUsersListFromJson(messageChunks[2]);
            guidRoom = Guid.Parse(messageChunks[1]);
            return users;
        }
        
        //"[STRING]:[ROOM_JSON]:[USER_LIST_JSON]"
        public static List<User> GetUserListAndRoomFromFormatStringRoomJsonUserListJson(string[] messageChunks,
            out Room room)
        {
            List<User> users = User.GetUsersListFromJson(messageChunks[2]);
            room = Room.GetRoomFromJson(messageChunks[1]);
            return users;
        }
        
        //"[STRING]:[INT]:[STRING]:[USERS_LIST_JSON]"
        public static int GetPageAndUsersFromFormatStringIntStringUserListJson(string[] messageChunks, out List<User> tmUsers)
        {
            int page = Int32.Parse(messageChunks[1]);
            string json = messageChunks[3];
            tmUsers = User.GetUsersListFromJson(json);
            return page;
        }
        
        //"[STRING]:[INT]:[STRING]:[USERS_LIST_JSON][ROOM_JSON]"
        public static int GetPageRoomAndUsersFromFormatStringIntStringUserListJsonRoomJson(string[] messageChunks, 
            out List<User> tmUsers, out Room room)
        {
            int page = Int32.Parse(messageChunks[1]);
            string json = messageChunks[3];
            tmUsers = User.GetUsersListFromJson(json);
            room = Room.GetRoomFromJson(messageChunks[4]);
            return page;
        }
        
        //"[STRING]:[INT]:[STRING]:[ROOMS_LIST_JSON]"
        public static int GetPageAndUsersFromFormatStringIntStringRoomListJson(string[] messageChunks, out List<Room> tmRooms)
        {
            int page = Int32.Parse(messageChunks[1]);
            string json = messageChunks[3];
            tmRooms = Room.GetRoomListFromJson(json);
            return page;
        }

        private static string SendSafe(string messageToSend)
        {
            messageToSend = ReplaceByKey(messageToSend, splitter, false);
            return messageToSend;
        }

       public static string ReplaceByKey(string message, string key, bool undo)
    {
        if (string.IsNullOrEmpty(message))
        {
            return message;
        }

        // Get the values from the SafeConvert dictionary
        string newValue = undo ? SafeConvert[key].remove : SafeConvert[key].replace;
        string oldValue = undo ? SafeConvert[key].replace : SafeConvert[key].remove;

        // If oldValue is empty, return the original message
        if (string.IsNullOrEmpty(newValue))
        {
            return message;
        }

        // Replace oldValue with newValue in the message
        message = message.Replace(newValue, "");
        
        // If newValue is empty, return the original message
        if (string.IsNullOrEmpty(oldValue))
        {
            return message;
        }
        message = message.Replace(oldValue, newValue);

        return message;
    }

        private static string ReadSafe(string messageToSend)
        {
            messageToSend = ReplaceByKey(messageToSend, splitter, true);
            return messageToSend;
        }

        public static string BuildMessageSafe(string[] toSend)
        {
            StringBuilder complete = new StringBuilder();
            for (var index = 0; index < toSend.Length; index++)
            {
                var send = toSend[index];
                complete.Append($"{SendSafe(send)}{(index==toSend.Length-1?"":splitter)}");
            }
            return complete.ToString();
        }

        public static string[] UnpackMessageSafe(string message)
        {
            string[] toSend = message.Split(new string[] {splitter}, StringSplitOptions.None);
            for (var index = 0; index < toSend.Length; index++)
            {
                toSend[index] = ReadSafe(toSend[index]);
            }
            return toSend;
        }


        
    }
}
