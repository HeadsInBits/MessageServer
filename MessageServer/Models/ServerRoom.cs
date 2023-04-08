using System.Text;
using LibObjects;
using MessageServer.Data;

namespace MessageServer;

public class ServerRoom: Room
{
	protected List<User> _bannedList = new List<User>();
	protected List<User> _approvedList = new List<User>();
	protected string _roomKey = string.Empty;
	protected List<User> _usersInRoom = new List<User>(); 
	protected ServerRoom(RoomJsonData json) : base(json)
    {
        
    }
    
    public List<User> GetUsersInRoom()
    {
    	return _usersInRoom;
    
    }
    
    public ServerRoom(User creator, int roomLimit, bool isPublic, string meta, string roomName) : base()
    {
    	_creator = creator;
    	_roomLimit = roomLimit;
    	_isPublic = isPublic;
    	_usersInRoom.Add(creator);
    	_meta = meta;
        _roomName = roomName;
    }
    
    public RoomStatusCodes AddUserToRoom(User usrToAdd, User requestedBy)
    {
	    if (UserInRoom(usrToAdd))
	    {
		    return RoomStatusCodes.AlreadyJoined;
	    }

	    if (_roomLimit <= _usersInRoom.Count)
	    {
		    return RoomStatusCodes.Full;
	    }

	    if (_isRoomLocked)
	    {
		    return RoomStatusCodes.RoomLocked;
	    }

	    if (_bannedList.Contains(usrToAdd))
	    {
		    return RoomStatusCodes.Banned;
	    }
	    
	    if (!_isPublic && !_approvedList.Contains(usrToAdd) && requestedBy.GetUserName() != _creator.GetUserName())
	    {
		    return RoomStatusCodes.Private;
	    }
	    
	    _usersInRoom.Add(usrToAdd);
	    return RoomStatusCodes.Ok;
    }

    public bool UserInRoom(User usrToAdd)
    {
	    foreach (var user in _usersInRoom)
	    {
		    if (user.GetUserName() == usrToAdd.GetUserName())
		    {
			    return true;
		    }
	    }
	    return false;
    }

    public RoomStatusCodes RemoveUserFromRoom(User usrToRemove)
    {
        foreach(var user in _usersInRoom)
        {
            if (user.GetUserName() == usrToRemove.GetUserName())
            {
                _usersInRoom.Remove(user);
                return RoomStatusCodes.Ok;
            }
        }
        return RoomStatusCodes.Ok;
	    
    }

    public RoomStatusCodes BanUserFromRoom(User usrToBan)
    {
	    _bannedList.Add(usrToBan);
	    return RoomStatusCodes.Ok;
    }
    public RoomStatusCodes RemoveBanFromUserFromRoom(User usrToUnBan)
    {
	    foreach (var user in _usersInRoom)
	    {
		    if (user.GetUserName() == usrToUnBan.GetUserName())
		    {
			    _bannedList.Remove(user);
		    }
	    }
	    return RoomStatusCodes.Ok;
    }
    
    public RoomStatusCodes ApproveUserFromRoom(User usrToApprove)
    {
	    _approvedList.Add(usrToApprove);
	    return RoomStatusCodes.Ok;
    }
    public RoomStatusCodes RemoveApproveFromUserFromRoom(User usrToUnApprove)
    {
	    foreach (var user in _usersInRoom)
	    {
		    if (user.GetUserName() == usrToUnApprove.GetUserName())
		    {
			    _approvedList.Remove(user);
		    }
	    }
	    return RoomStatusCodes.Ok;
    }

    public void LockRoom(bool on)
    {
	    _isRoomLocked = on;
    }
    
    public void SetRoomName(string name)
    {
	    _roomName = name;
    }

    public List<User> GetBannedList()
    {
	    return _bannedList;
    }

    public List<User> GetApprovedList()
    {
	    return _approvedList;
    }
}