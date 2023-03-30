using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageServer.Data;
using Newtonsoft.Json;

namespace LibObjects
{
	public class Room
	{
		public enum RoomStatusCodes
		{
			OK,
			BANNED,
			ROOMLOCKED,
			PASSWORDFAILED
		}
		public Guid RoomID = Guid.NewGuid();
		public List<User> usersInRoom = new List<User>();
		public List<User> bannedList = new List<User>();
		public string RoomName;
		public bool isRoomLocked = false;
		public string roomKey = string.Empty;
		public DateTime roomCreation = DateTime.Now;
		public User creator;
		public int _roomLimit = 0;
		public bool _isPublic;
		public string Meta = "";



		public Room(User creator, int roomLimit, bool isPublic, string meta)
		{
			this.creator = creator;
			_roomLimit = roomLimit;
			_isPublic = isPublic;
			usersInRoom.Add(creator);
			Meta = meta;
		}


		public List<User> GetUsersInRoom()
		{
			return usersInRoom;

		}

		public string GetRoomName()
		{
			return RoomName;
		}

		public Guid GetGuid()
		{
			return RoomID;
		}

		public RoomStatusCodes AddUserToRoom(User usrToAdd)
		{
			Console.WriteLine("AddUserToRoom");
			var ls = new StringBuilder();
				usersInRoom.ForEach(a => ls.Append(a._userName));
			Console.WriteLine(ls.ToString());
			Console.WriteLine(isRoomLocked);
			
			if (isRoomLocked || _roomLimit <= usersInRoom.Count)
			{
				Console.WriteLine("room locked");
				return RoomStatusCodes.ROOMLOCKED;
			}
			Console.WriteLine("Room Not Locked");

			if (!bannedList.Contains(usrToAdd)) {
				Console.WriteLine("user added");
				Console.WriteLine(usrToAdd.WebSocketID);
				usersInRoom.Add(usrToAdd);
				return RoomStatusCodes.OK;
			}
			else {
				Console.WriteLine("user banned");
				return RoomStatusCodes.BANNED;
			}
		}

		public RoomStatusCodes RemoveUserFromRoom(User usrToRemove)
		{
			usersInRoom.Remove(usrToRemove);
			return RoomStatusCodes.OK;
		}

		public RoomStatusCodes BanUserFromRoom(User usrToBan)
		{
			bannedList.Add(usrToBan);
			return RoomStatusCodes.OK;
		}
		
		//TODO: NOW SERIALISATION AND DESERIALIZATION IS HAPPENING HERE WE COULD:
		//1. CHANGE THE FORMAT
		//2. VALIDATE ALL IN ONE PLACE
		//3. RESTRICT DATA BEING PASSED
		//4. OPTIMISE FOR DATA SIZE
		//5. ENCRYPT?

		public static List<Room> GetRoomListFromJson(string jsonData)
		{
			return JsonConvert.DeserializeObject<List<Room>>(jsonData);
		}
		
		public static string GetJsonFromRoomList(List<Room> rooms)
		{
			return JsonConvert.SerializeObject(rooms, Formatting.Indented);
		}
		
		public static Room GetRoomFromJson(string JsonString)
		{
			return JsonConvert.DeserializeObject<Room>(JsonString);
		}
		
		public static string GetJsonFromRoom(Room room)
		{
			return JsonConvert.SerializeObject(room, Formatting.Indented);
		}

		public User GetUserByGuid(Guid userId)
		{
			foreach (User user in usersInRoom)
			{
				if (user.GetUserGuid() == userId)
				{
					return user;
				}
			}
			return new User("N/A", false, Guid.Empty, -1);
		}
	}
}