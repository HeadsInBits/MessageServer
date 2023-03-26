using MessageServer.Data;
using Newtonsoft.Json;
using System.Security.Principal;
using System.Text.Json;
using System.Xml;

namespace MessageServer.Models;

public class RoomController
{
	private List<Room> privateRooms = new List<Room>();

	public int CreateNewRoom(User roomCreator, string [] messageChunks)
	{
		Room tmpRoom = new Room(roomCreator, int.Parse(messageChunks [1]), messageChunks [2].ToUpper() == "PUBLIC", messageChunks [3]);
		privateRooms.Add(tmpRoom);
		return privateRooms.IndexOf(tmpRoom);
	}

	public List<User> GetUsersInRoom(int roomId)
	{
		return privateRooms [roomId].GetUsersInRoom();
	}

    

	public List<int> FindUserInRooms(User user)
	{
		int counter = 0;

		List<int> roomList = new List<int>();

		foreach (var room in privateRooms) {
			foreach (var usr in room.GetUsersInRoom()) {
				if (usr == user) {
					roomList.Add(item: counter);
				}
			}

			counter++;
		}

		return roomList;
	}

	public List<Room> GetRoomList()
	{
		return privateRooms;
	}

	public string JSONGetRoomList()
	{
		string output = JsonConvert.SerializeObject(privateRooms, Newtonsoft.Json.Formatting.Indented);
	//	Console.WriteLine(output);
		return output;
	}

	

	public Room.RoomStatusCodes AddUserToRoom(User userToAdd, int roomNumber)
	{
		return privateRooms [roomNumber].AddUserToRoom(userToAdd);
	}

	public void DestroyRoom(int index)
	{
		privateRooms.RemoveAt(index);
	}
}