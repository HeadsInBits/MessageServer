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

	public Dictionary<Guid, Room> GetRoomList()
	{
		return RoomDictionary;
	}

	public string JSONGetRoomList()
	{
		List<Room> rooms = new List<Room>();
		foreach (var pair in RoomDictionary)
		{
			rooms.Add(pair.Value);
		}
		string output = Room.GetJsonFromRoomList(rooms);
	//	Console.WriteLine(output);
		return output;
	}

	

	public Room.RoomStatusCodes AddUserToRoom(User userToAdd, Guid roomNumber)
	{
		return RoomDictionary [roomNumber].AddUserToRoom(userToAdd);
	}

	public void DestroyRoom(Guid index)
	{
		RoomDictionary.Remove(index);
	}
}