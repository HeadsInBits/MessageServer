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
    
    public RoomStatusCodes AddUserToRoom(User usrToAdd)
    {
	    Console.WriteLine("AddUserToRoom");
	    var ls = new StringBuilder();
	    _usersInRoom.ForEach(a => ls.Append(a._userName));
	    Console.WriteLine(ls.ToString());
	    Console.WriteLine(_isRoomLocked);
			
	    if (_isRoomLocked || _roomLimit <= _usersInRoom.Count)
	    {
		    Console.WriteLine("room locked");
		    return RoomStatusCodes.RoomLocked;
	    }
	    Console.WriteLine("Room Not Locked");

	    if (!_bannedList.Contains(usrToAdd)) {
		    Console.WriteLine("user added");
		    Console.WriteLine(usrToAdd.WebSocketID);
		    _usersInRoom.Add(usrToAdd);
		    return RoomStatusCodes.Ok;
	    }
	    else {
		    Console.WriteLine("user banned");
		    return RoomStatusCodes.Banned;
	    }
    }
    
    public RoomStatusCodes RemoveUserFromRoom(User usrToRemove)
    {
	    _usersInRoom.Remove(usrToRemove);
	    return RoomStatusCodes.Ok;
    }

    public RoomStatusCodes BanUserFromRoom(User usrToBan)
    {
	    _bannedList.Add(usrToBan);
	    return RoomStatusCodes.Ok;
    }
    public RoomStatusCodes RemoveBanFromUserFromRoom(User usrToUnBan)
    {
	    _bannedList.Remove(usrToUnBan);
	    return RoomStatusCodes.Ok;
    }
    
    public RoomStatusCodes ApproveUserFromRoom(User usrToApprove)
    {
	    _approvedList.Add(usrToApprove);
	    return RoomStatusCodes.Ok;
    }
    public RoomStatusCodes RemoveApproveFromUserFromRoom(User usrToUnApprove)
    {
	    _approvedList.Remove(usrToUnApprove);
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
}