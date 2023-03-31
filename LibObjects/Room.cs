using System;
using System.Collections.Generic;
using System.Text;
using MessageServer.Data;
using Newtonsoft.Json;

namespace LibObjects
{
	public class Room
	{
		class RoomJsonData
		{
			public Guid RoomId;
			public List<User> UsersInRoom;
			public List<User> BannedList;
			public string RoomName;
			public bool IsRoomLocked;
			public string RoomKey;
			public DateTime RoomCreation;
			public User Creator;
			public int RoomLimit;
			public bool IsPublic;
			public string Meta;
		}

		private RoomJsonData GetJsonDataFromRoom()
		{
			RoomJsonData json = new RoomJsonData
			{
				RoomId = _roomId,
				UsersInRoom = UsersInRoom,
				BannedList = _bannedList,
				RoomName = _roomName,
				IsRoomLocked = _isRoomLocked,
				RoomKey = _roomKey,
				RoomCreation = _roomCreation,
				Creator = _creator,
				RoomLimit = _roomLimit,
				IsPublic = _isPublic,
				Meta = _meta
			};
			return json;
		}
		
		private Room(RoomJsonData json)
		{
			_roomId = json.RoomId;
			UsersInRoom = json.UsersInRoom;
			_bannedList = json.BannedList;
			_roomName = json.RoomName;
			_isRoomLocked = json.IsRoomLocked;
			_roomKey = json.RoomKey;
			_roomCreation = json.RoomCreation;
			_creator = json.Creator;
			_roomLimit = json.RoomLimit;
			_isPublic = json.IsPublic;
			_meta = json.Meta;
		}
		
		public enum RoomStatusCodes
		{
			Ok,
			Banned,
			RoomLocked
		}

		private readonly Guid _roomId = Guid.NewGuid();
		private List<User> UsersInRoom = new List<User>();
		private readonly List<User> _bannedList = new List<User>();
		private readonly string _roomName;
		private readonly bool _isRoomLocked;
		private readonly string _roomKey = string.Empty;
		private readonly DateTime _roomCreation = DateTime.Now;
		private readonly User _creator;
		private readonly int _roomLimit;
		private readonly bool _isPublic;
		private readonly string _meta;
		public const int NumberOfRoomsToSendInMessage = 20;

		public sealed override int GetHashCode()
		{
			return _roomId.GetHashCode();
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
			_creator = creator;
			_roomLimit = roomLimit;
			_isPublic = isPublic;
			UsersInRoom.Add(creator);
			_meta = meta;
			RoomJsonData j = GetJsonDataFromRoom();
			Room room = new Room(j);
			Console.WriteLine($"{GetGuid()} == {room.GetGuid()}");
			Console.WriteLine($"{this.GetHashCode().ToString()} == {room.GetHashCode().ToString()}");
			Console.WriteLine(this == room);
		}


		public List<User> GetUsersInRoom()
		{
			return UsersInRoom;

		}

		public string GetRoomName()
		{
			return _roomName;
		}

		public Guid GetGuid()
		{
			return _roomId;
		}

		public RoomStatusCodes AddUserToRoom(User usrToAdd)
		{
			Console.WriteLine("AddUserToRoom");
			var ls = new StringBuilder();
				UsersInRoom.ForEach(a => ls.Append(a._userName));
			Console.WriteLine(ls.ToString());
			Console.WriteLine(_isRoomLocked);
			
			if (_isRoomLocked || _roomLimit <= UsersInRoom.Count)
			{
				Console.WriteLine("room locked");
				return RoomStatusCodes.RoomLocked;
			}
			Console.WriteLine("Room Not Locked");

			if (!_bannedList.Contains(usrToAdd)) {
				Console.WriteLine("user added");
				Console.WriteLine(usrToAdd.WebSocketID);
				UsersInRoom.Add(usrToAdd);
				return RoomStatusCodes.Ok;
			}
			else {
				Console.WriteLine("user banned");
				return RoomStatusCodes.Banned;
			}
		}

		public RoomStatusCodes RemoveUserFromRoom(User usrToRemove)
		{
			UsersInRoom.Remove(usrToRemove);
			return RoomStatusCodes.Ok;
		}

		public RoomStatusCodes BanUserFromRoom(User usrToBan)
		{
			_bannedList.Add(usrToBan);
			return RoomStatusCodes.Ok;
		}
		
		//TODO: NOW SERIALISATION AND DESERIALIZATION IS HAPPENING HERE WE COULD:
		//1. CHANGE THE FORMAT
		//2. VALIDATE ALL IN ONE PLACE
		//3. RESTRICT DATA BEING PASSED
		//4. OPTIMISE FOR DATA SIZE
		//5. ENCRYPT?

		public static List<Room> GetRoomListFromJson(string jsonData)
		{
			List<RoomJsonData> roomJsonDataList = JsonConvert.DeserializeObject<List<RoomJsonData>>(jsonData);
			List<Room> rooms = new List<Room>();
			foreach (var data in roomJsonDataList)
			{
				rooms.Add(new Room(data));
			}
			return rooms;
		}
		
		public static string GetJsonFromRoomList(List<Room> rooms)
		{
			List<RoomJsonData> roomJsonDataList = new List<RoomJsonData>();
			foreach (var room in rooms)
			{
				roomJsonDataList.Add(room.GetJsonDataFromRoom());
			}
			return JsonConvert.SerializeObject(roomJsonDataList, Formatting.Indented);
		}
		
		public static Room GetRoomFromJson(string jsonString)
		{
			return new Room(JsonConvert.DeserializeObject<RoomJsonData>(jsonString));
		}
		
		public static string GetJsonFromRoom(Room room)
		{
			return JsonConvert.SerializeObject(room.GetJsonDataFromRoom(), Formatting.Indented);
		}

		public User GetUserByGuid(Guid userId)
		{
			foreach (User user in UsersInRoom)
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