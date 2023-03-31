using MessageServer.Data;
using System.Security.Principal;
using System.Text.Json;
using System.Xml;
using LibObjects;

namespace MessageServer.Models;

public class RoomController
{
	private Dictionary<Guid,Room> RoomDictionary = new Dictionary<Guid, Room>();

	public Room CreateNewRoom(User roomCreator, string [] messageChunks)
	{
		Room tmpRoom = new Room(roomCreator, int.Parse(messageChunks [1]), messageChunks [2].ToUpper() == "PUBLIC", messageChunks [3]);
		RoomDictionary.Add(tmpRoom.GetGuid(), tmpRoom);
		return tmpRoom;
	}

	public List<User> GetUsersInRoom(Guid roomId)
	{
		return RoomDictionary [roomId].GetUsersInRoom();
	}
	public List<Guid> FindAllRoomsWhereUserInRoom(User user)
	{
		int counter = 0;

		List<Guid> roomList = new List<Guid>();

		foreach (var pair in RoomDictionary) {
			foreach (var usr in pair.Value.GetUsersInRoom()) {
				if (usr == user) {
					roomList.Add(pair.Key);
				}
			}
			counter++;
		}
		return roomList;
	}

	public Dictionary<Guid, Room> GetRoomDictionary()
	{
		return RoomDictionary;
	}

	public string JSONGetRoomList()
	{
		var rooms = GetRoomsList();
		string output = Room.GetJsonFromRoomList(rooms);
		return output;
	}

	public List<Room> GetRoomsList()
	{
		List<Room> rooms = new List<Room>();
		foreach (var pair in RoomDictionary)
		{
			rooms.Add(pair.Value);
		}

		return rooms;
	}


	public Room.RoomStatusCodes AddUserToRoom(User userToAdd, Guid roomNumber)
	{
		if (RoomDictionary.ContainsKey(roomNumber))
		{
			Console.WriteLine($"User {userToAdd.GetUserName()} added to room {roomNumber}");
			Room room = RoomDictionary [roomNumber];
			var addUserToRoom = room.AddUserToRoom(userToAdd);
			return addUserToRoom;
			
		}
		else
		{
			throw new Exception($"Room {roomNumber} doent exist");
		}
	}

	public void DestroyRoom(Guid index)
	{
		RoomDictionary.Remove(index);
	}

	public Room GetRoomFromGUID(Guid guid)
	{
		if (RoomDictionary.ContainsKey(guid))
		{
			return RoomDictionary[guid];
		}
		else
		{
			throw new Exception("room doesnt exist");
		}
	}
}