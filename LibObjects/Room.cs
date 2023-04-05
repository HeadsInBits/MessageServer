using System;
using System.Collections.Generic;
using System.Text;
using MessageServer.Data;
using Newtonsoft.Json;

namespace LibObjects
{
	public class Room
	{
		protected class RoomJsonData
		{
			public Guid RoomId;
			public string RoomName;
			public bool IsRoomLocked;
			public DateTime RoomCreation;
			public User Creator;
			public int RoomLimit;
			public bool IsPublic;
			public string Meta;
		}

		protected RoomJsonData GetJsonDataFromRoom()
		{
			RoomJsonData json = new RoomJsonData
			{
				RoomId = _roomId,
				RoomName = _roomName,
				IsRoomLocked = _isRoomLocked,
				RoomCreation = _roomCreation,
				Creator = _creator,
				RoomLimit = _roomLimit,
				IsPublic = _isPublic,
				Meta = _meta,
			};
			return json;
		}

		protected Room(RoomJsonData json)
		{
			_roomId = json.RoomId;
			_roomName = json.RoomName;
			_isRoomLocked = json.IsRoomLocked;
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
			RoomLocked,
			Full,
			AlreadyJoined,
			Private
		}
		
		
		
		protected Guid _roomId = Guid.NewGuid();
		protected string _roomName;
		protected bool _isRoomLocked;
		protected DateTime _roomCreation = DateTime.Now;
		protected User _creator;
		protected int _roomLimit;
		protected bool _isPublic;
		protected string _meta;
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

		protected Room()
		{
			
		}

		public string GetRoomName()
		{
			return _roomName;
		}

		public Guid GetGuid()
		{
			return _roomId;
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

		public User GetCreator()
		{
			return _creator;
		}
	}
}