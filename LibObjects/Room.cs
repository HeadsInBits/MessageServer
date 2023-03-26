using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MessageServer.Data
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
		public List<User> bannedList;
		public string RoomName;
		public bool isRoomLocked;
		public string roomKey = string.Empty;
		public DateTime roomCreation = DateTime.Now;
		public User creator;
		public int _roomLimit;
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
			if (isRoomLocked || _roomLimit <= usersInRoom.Count)
				return RoomStatusCodes.ROOMLOCKED;

			if (!bannedList.Contains(usrToAdd)) {
				usersInRoom.Add(usrToAdd);
				return RoomStatusCodes.OK;
			}
			else {
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

	}
}