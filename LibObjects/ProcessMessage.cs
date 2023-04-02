using System;
using System.Collections.Generic;
using System.Text;
using MessageServer.Data;
using Newtonsoft.Json;

namespace LibObjects;

public static class ProcessMessage
{
    private static readonly Dictionary<(bool remove, string removeKey), (string remove, string replace)> SafeConvert =
        new()
        {
            { (true,":"), (":", "*/*") },
            { (false,":"), ("*/*", ":") }
        };

    //"[STRING]:[ROOM_LIST_JSON]"
    public static List<Room> GetRoomsListFromMessageFormatStringJsonRooms(string message, string[] messageChunks)
    {
        string jsonData = message.Substring(messageChunks[0].Length + 1);
        
        List<Room> jsonDe = Room.GetRoomListFromJson(jsonData);
        return jsonDe;
    }

    //"[STRING]:[USER_JSON]:[STRING]"
    public static string GetUserMessageFromMessageFormatStringJsonRoomString(string message,
        string[] messageChunks, out User user)
    {
        var messageString = messageChunks[messageChunks.Length-1];
        string jsonStrUser = message.Substring(messageChunks[0].Length + 1,
            message.Length - (messageString.Length + messageChunks[0].Length + 2));
        user = User.GetUserFromJson(jsonStrUser);
        return messageString;
    }

    //"[STRING]:[USER_LIST_JSON]"
    public static List<User> GetUsersFromMessageFormatStringJsonUserList(string message, string[] messageChunks)
    {
        string jsonStrUsers = message.Substring(messageChunks[0].Length + 1);
        return User.GetUsersListFromJson(jsonStrUsers);
    }
    
    //"[STRING]:[USER_JSON]"
    public static User GetUserFromMessageFormatStringJsonUser(string message, string[] messageChunks)
    {
        string users = message.Substring(messageChunks[0].Length + 1);
        return User.GetUserFromJson(users);
    }

    //"[STRING]:[ROOM_JSON] 
    public static Room GetRoomFromMessageFormatStringRoom(string message, string[] messageChunks)
    {
        string jsonString = message.Substring(messageChunks[0].Length + 1);
        Room room = Room.GetRoomFromJson(jsonString);
        return room;
    }
    
    //"[STRING]:[UserID_GUID]:[ROOM_JSON]:[MESSAGE_STRING]"
    public static string GetRoomUserAndMessageFromFormatStringGuidRoomJsonMessage(string message,
        string[] messageChunks, out Room room, out User userFromRoom)
    {
        Guid userID = Guid.Parse(messageChunks[1]);
        string roomMessageString = messageChunks[messageChunks.Length-1];
        roomMessageString = ReadSafe(roomMessageString);
        string jsonStrRoom = message.Substring(messageChunks[0].Length + messageChunks[1].Length + 2,
            message.Length - ( messageChunks[messageChunks.Length-1].Length + messageChunks[1].Length  + messageChunks[0].Length + 3));
        room = Room.GetRoomFromJson(jsonStrRoom);
        userFromRoom = room.GetUserByGuid(userID);
        return roomMessageString;
    }

    public static string SendSafe(string messageToSend)
    {
        var key = (true,":");
        messageToSend = ReplaceByKey(messageToSend, key);
        return messageToSend;
    }

    public static string ReplaceByKey(string messageToSend, (bool remove, string removeKey) key)
    {
        while (messageToSend.Contains(SafeConvert[key].replace))
        {
            messageToSend = messageToSend.Replace(SafeConvert[key].replace, "");
        }
        messageToSend = messageToSend.Replace(SafeConvert[key].remove, SafeConvert[key].replace);
        return messageToSend;
    }

    public static string ReadSafe(string messageToSend)
    {
        var key = (false,":");
        messageToSend = ReplaceByKey(messageToSend, key);
        return messageToSend;
    }

    public static User GetUserAndGuidFromFormatStringGuidUserJson(string message, string[] messageChunks,
        out Guid guidJoined)
    {
        User joinedUser = User.GetUserFromJson(message.Substring(messageChunks[0].Length + messageChunks[1].Length + 2));
        guidJoined = Guid.Parse(messageChunks[1]);
        return joinedUser;
    }
}