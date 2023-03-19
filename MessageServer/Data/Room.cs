namespace MessageServer;

public class Room
{
    private Guid RoomID = Guid.NewGuid();
    private List<User> usersInRoom;
    private bool isRoomLocked = false;
    private DateTime roomCreation = DateTime.Now;

    public Room(User creator)
    {
        
    }
}