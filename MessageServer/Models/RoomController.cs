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
		ServerRoom tmpServerRoom = new ServerRoom(ServerRoomCreator, int.Parse(messageChunks [1]), messageChunks [2].ToUpper() == "PUBLIC", messageChunks [3],messageChunks [4]);
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
				if (usr.Equals(user)) {
					ServerRoomList.Add(pair.Key);
				}
			}
			counter++;
		}
		return ServerRoomList;
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


	public Room.RoomStatusCodes AddUserToServerRoom(User userToAdd, Guid ServerRoomNumber, User requestedBy)
	{
		if (ServerRoomDictionary.ContainsKey(ServerRoomNumber))
		{
			Console.WriteLine($"User {userToAdd.GetUserName()} added to ServerRoom {ServerRoomNumber}");
			ServerRoom ServerRoom = ServerRoomDictionary [ServerRoomNumber];
			var addUserToServerRoom = ServerRoom.AddUserToRoom(userToAdd, requestedBy);
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
	
	public void RemoveUserFromBanListInServerRoom(User user, Room room)
	{
		ServerRoomDictionary[room.GetGuid()].RemoveBanFromUserFromRoom(user);
	}
	
	public void AddUserToBanListInServerRoom(User user, Room room)
	{
		ServerRoomDictionary[room.GetGuid()].BanUserFromRoom(user);
	}
	
	public void ApproveUserFromRoom(User user, Room room)
	{
		ServerRoomDictionary[room.GetGuid()].ApproveUserFromRoom(user);
	}
	
	public void RemoveUserFromApproveListInServerRoom(User user, Room room)
	{
		ServerRoomDictionary[room.GetGuid()].RemoveApproveFromUserFromRoom(user);
	}
	
	private void LockRoom(bool on, Room room)
	{
		ServerRoomDictionary[room.GetGuid()].LockRoom(on);
	}
	
	public void RenameServerRoom(string roomName, Room room)
	{
		ServerRoomDictionary[room.GetGuid()].SetRoomName(roomName);
	}

	public List<User> GetUsersInRoom(Room room)
	{
		
		List<User> usersInRoom = ServerRoomDictionary[room.GetGuid()].GetUsersInRoom();
		Console.WriteLine($"Get users in {room.GetRoomName()} = {usersInRoom.Count}");
		return usersInRoom;
	}
	
	public bool IsCreatorOfRoom(Room room, User user)
	{
		if (ServerRoomDictionary[room.GetGuid()].GetCreator() == user.GetUserName())
		{
			return true;
		}

		return false;
	}

	public bool TryLockRoom(Room room, User user, bool on)
	{
		if (IsCreatorOfRoom(room, user))
		{
			LockRoom(on, room);
			return true;
		}

		return false;
	}

	public bool IsInRoom(Room room, User user)
	{
		return ServerRoomDictionary[room.GetGuid()].UserInRoom(user);
	}

	public bool RoomExists(Guid guid)
	{
		return ServerRoomDictionary.ContainsKey(guid);
	}

	public List<User> GetBannedUserListInRoom(Room room)
	{
		return ServerRoomDictionary[room.GetGuid()].GetBannedList();
	}
	
	public List<User> GetApprovedUserListInRoom(Room room)
	{
		return ServerRoomDictionary[room.GetGuid()].GetApprovedList();
	}
}