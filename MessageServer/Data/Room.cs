namespace MessageServer;

public class Room
{
    public enum RoomStatusCodes
    {
        OK,
        BANNED,
        ROOMLOCKED,
        PASSWORDFAILED
    }
    
    private Guid RoomID = Guid.NewGuid();
    private List<User> usersInRoom;
    private List<User> bannedList;
    private String RoomName; 
    private bool isRoomLocked { get { return isRoomLocked; } }
    private String roomKey = String.Empty;
    private DateTime roomCreation = DateTime.Now;
    private User creator;
    private int _roomLimit;
    private bool _isPublic;

    public Room(User creator, int roomLimit, bool isPublic)
    { 
        this.creator = creator;
        _roomLimit = roomLimit;
        _isPublic = isPublic;

    }

    public List<User> GetUsersInRoom()
    {
        return usersInRoom;
        
    }

    public String GetRoomName()
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
        
        if (!bannedList.Contains(usrToAdd))
        {
            usersInRoom.Add(usrToAdd);
            return RoomStatusCodes.OK;
        }
        else
        {
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