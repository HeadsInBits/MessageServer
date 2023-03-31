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
		class RoomJsonData
		{
			public Guid RoomID;
			public List<User> usersInRoom;
			public List<User> bannedList;
			public string RoomName;
			public bool isRoomLocked;
			public string roomKey;
			public DateTime roomCreation;
			public User creator;
			public int _roomLimit;
			public bool _isPublic;
			public string Meta;
		}

		private RoomJsonData GetJsonDataFromRoom()
		{
			RoomJsonData json = new RoomJsonData();
			json.RoomID = RoomId;
			json.usersInRoom = usersInRoom;
			json.bannedList = bannedList;
			json.RoomName = RoomName;
			json.isRoomLocked = isRoomLocked;
			json.roomKey = roomKey;
			json.roomCreation = roomCreation;
			json.creator = creator;
			json._roomLimit = _roomLimit;
			json._isPublic = _isPublic;
			json.Meta = Meta;
			return json;
		}
		
		private Room(RoomJsonData json)
		{
			RoomId = json.RoomID;
			usersInRoom = json.usersInRoom;
			bannedList = json.bannedList;
			RoomName = json.RoomName;
			isRoomLocked = json.isRoomLocked;
			roomKey = json.roomKey;
			roomCreation = json.roomCreation;
			creator = json.creator;
			_roomLimit = json._roomLimit;
			_isPublic = json._isPublic;
			Meta = json.Meta;
		}
		
		public enum RoomStatusCodes
		{
			OK,
			BANNED,
			ROOMLOCKED,
			PASSWORDFAILED
		}
		
		public readonly Guid RoomId = Guid.NewGuid();
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
		public static int NumberOfRoomsToSendInMessage = 20;

		public sealed override int GetHashCode()
		{
			return RoomId.GetHashCode();
		}
		
		public static bool operator ==(Room lhs, Room rhs)
		{
			if (lhs is null)
			{
				if (rhs is null)
				{
					// null == null = true.
					return true;
				}

				// Only the left side is null.
				return false;
			}
			// Equals handles the case of null on right side.
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Room lhs, Room rhs)
		{
			return !(lhs == rhs);
		}
		
		public override bool Equals(object obj) => this.Equals(obj as Room);

		public bool Equals(Room p)
		{
			if (p is null)
			{
				return false;
			}

			// Optimization for a common success case.
			if (Object.ReferenceEquals(this, p))
			{
				return true;
			}

			// If run-time types are not exactly the same, return false.
			if (this.GetType() != p.GetType())
			{
				return false;
			}

			// Return true if the fields match.
			// Note that the base class is not invoked because it is
			// System.Object, which defines Equals as reference equality.
			return p.GetGuid() == GetGuid();
		}


		public Room(User creator, int roomLimit, bool isPublic, string meta)
		{
			this.creator = creator;
			_roomLimit = roomLimit;
			_isPublic = isPublic;
			usersInRoom.Add(creator);
			Meta = meta;
			RoomJsonData j = GetJsonDataFromRoom();
			Room room = new Room(j);
			Console.WriteLine($"{GetGuid()} == {room.GetGuid()}");
			Console.WriteLine($"{this.GetHashCode().ToString()} == {room.GetHashCode().ToString()}");
			Console.WriteLine(this == room);
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
			return RoomId;
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
			return new Room(JsonConvert.DeserializeObject<RoomJsonData>(JsonString));
		}
		
		public static string GetJsonFromRoom(Room room)
		{
			return JsonConvert.SerializeObject(room.GetJsonDataFromRoom(), Formatting.Indented);
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