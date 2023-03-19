namespace MessageServer;

public class RoomController
{
    private List<Room> privateRooms = new List<Room>();

    public int CreateNewRoom(User roomCreator)
    {
        Room tmpRoom = new Room(roomCreator);
        privateRooms.Add(tmpRoom);
        return privateRooms.IndexOf(tmpRoom);
    }

    public void DestroyRoom(int index)
    {
        privateRooms.RemoveAt(index);
    }
}