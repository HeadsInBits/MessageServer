using MessageServer.Data;
using System.Security.Principal;
using System.Text.Json;
using System.Xml;
using LibObjects;

namespace MessageServer.Models;

public class RoomController
{
	private Dictionary<Guid,ServerRoom> ServerRoomDictionary = new Dictionary<Guid, ServerRoom>();

	public ServerRoom CreateNewServerRoom(User ServerRoomCreator, string [] messageChunks)
	{
		ServerRoom tmpServerRoom = new ServerRoom(ServerRoomCreator, int.Parse(messageChunks [1]), messageChunks [2].ToUpper() == "PUBLIC", messageChunks [3],"");
		ServerRoomDictionary.Add(tmpServerRoom.GetGuid(), tmpServerRoom);
		return tmpServerRoom;
	}

	public List<User> GetUsersInServerRoom(Guid ServerRoomId)
	{
		return ServerRoomDictionary [ServerRoomId].GetUsersInRoom();
	}
	public List<Guid> FindAllServerRoomsWhereUserInServerRoom(User user)
	{
		int counter = 0;

		List<Guid> ServerRoomList = new List<Guid>();

		foreach (var pair in ServerRoomDictionary) {
			foreach (var usr in pair.Value.GetUsersInRoom()) {
				if (usr == user) {
					ServerRoomList.Add(pair.Key);
				}
			}
			counter++;
		}
		return ServerRoomList;
	}

	public Dictionary<Guid, ServerRoom> GetServerRoomDictionary()
	{
		return ServerRoomDictionary;
	}

	public string JSONGetServerRoomList()
	{
		List<Room> Rooms = GetRoomsList();
		string output = Room.GetJsonFromRoomList(Rooms);
		return output;
	}

	public List<ServerRoom> GetServerRoomsList()
	{
		List<ServerRoom> ServerRooms = new List<ServerRoom>();
		foreach (var pair in ServerRoomDictionary)
		{
			ServerRooms.Add(pair.Value);
		}
		return ServerRooms;
	}
	
	public List<Room> GetRoomsList()
	{
		List<Room> Rooms = new List<Room>();
		foreach (var pair in ServerRoomDictionary)
		{
			Rooms.Add(pair.Value);
		}

		return Rooms;
	}


	public ServerRoom.RoomStatusCodes AddUserToServerRoom(User userToAdd, Guid ServerRoomNumber)
	{
		if (ServerRoomDictionary.ContainsKey(ServerRoomNumber))
		{
			Console.WriteLine($"User {userToAdd.GetUserName()} added to ServerRoom {ServerRoomNumber}");
			ServerRoom ServerRoom = ServerRoomDictionary [ServerRoomNumber];
			var addUserToServerRoom = ServerRoom.AddUserToRoom(userToAdd);
			return addUserToServerRoom;
			
		}
		else
		{
			throw new Exception($"ServerRoom {ServerRoomNumber} doent exist");
		}
	}

	public void DestroyServerRoom(Guid index)
	{
		ServerRoomDictionary.Remove(index);
	}

	public ServerRoom GetServerRoomFromGUID(Guid guid)
	{
		if (ServerRoomDictionary.ContainsKey(guid))
		{
			return ServerRoomDictionary[guid];
		}
		else
		{
			throw new Exception("ServerRoom doesnt exist");
		}
	}

	public void RemoveUserFromServerRoom(User user, Room room)
	{
		ServerRoomDictionary[room.GetGuid()].RemoveUserFromRoom(user);
	}

	public List<User> GetUsersInRoom(Room room)
	{
		return ServerRoomDictionary[room.GetGuid()].GetUsersInRoom();
	}
}