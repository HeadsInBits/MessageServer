using System;
using System.Collections.Generic;
using System.Text;
using MessageServer.Data;
using Newtonsoft.Json;

namespace LibObjects
{
    
    public static class ProcessMessageData
    {
        private const string splitter = ":*:";
        private static readonly Dictionary<string, (string remove, string replace)> SafeConvert =
            new()
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
        public static string GetUserMessageFromMessageFormatStringJsonRoomString(string[] messageChunks, out User user)
        {
            var messageString = messageChunks[2];
            string jsonStrUser = messageChunks[1];
            user = User.GetUserFromJson(jsonStrUser);
            return messageString;
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
        public static Room GetRoomFromMessageFormatStringRoom(string[] messageChunks)
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
        
        //"[STRING]:[INT]:[STRING]:[USERS_LIST_JSON]"
        public static int ExtractPageAndUsersFromFormatStringIntStringUserListJson(string[] messageChunks, out List<User> tmUsers)
        {
            int page = Int32.Parse(messageChunks[1]);
            string json = messageChunks[3];
            tmUsers = User.GetUsersListFromJson(json);
            return page;
        }
        
        //"[STRING]:[INT]:[STRING]:[ROOMS_LIST_JSON]"
        public static int ExtractPageAndUsersFromFormatStringIntStringRoomListJson(string[] messageChunks, out List<Room> tmRooms)
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
        string oldValue = undo ? SafeConvert[key].remove : SafeConvert[key].replace;
        string newValue = undo ? SafeConvert[key].replace : SafeConvert[key].remove;

        // If oldValue is empty, return the original message
        if (string.IsNullOrEmpty(oldValue))
        {
            return message;
        }

        // Replace oldValue with newValue in the message
        message = message.Replace(oldValue, "");
        
        // If newValue is empty, return the original message
        if (string.IsNullOrEmpty(newValue))
        {
            return message;
        }
        message = message.Replace(newValue, oldValue);

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
