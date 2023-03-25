namespace MessageServer;

public class RoomController
{
    private List<Room> privateRooms = new List<Room>();

    public int CreateNewRoom(User roomCreator, string[] messageChunks)
    {
        Room tmpRoom = new Room(roomCreator, Int32.Parse(messageChunks[1]), messageChunks[2].ToUpper()=="PUBLIC");
        privateRooms.Add(tmpRoom);
        return privateRooms.IndexOf(tmpRoom);
    }

    public List<User> GetUsersInRoom(int roomId)
    {
        return privateRooms[roomId].GetUsersInRoom();
    }

    public List<int> FindUserInRooms(User user)
    {
        int counter = 0;

        List<int> roomList = new List<int>();

        foreach (var room in privateRooms)
        {
            foreach (var usr in room.GetUsersInRoom())
            {
                if (usr == user)
                {
                    roomList.Add(item: counter);
                }
            }

            counter++;
        }

        return roomList;
    }

    public List<Room> GetRoomList()
    {
        return privateRooms;
    }

    public Room.RoomStatusCodes AddUserToRoom(User userToAdd, int roomNumber)
    {
        return privateRooms[roomNumber].AddUserToRoom(userToAdd);
    }

    public void DestroyRoom(int index)
    {
        privateRooms.RemoveAt(index);
    }
}