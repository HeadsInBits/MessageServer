namespace MessageServer;

public class UserController
{
    public List<User> connectedClients = new List<User>();


    public User? GetUserProfileFromSocketId(int SocketId)
    {
        foreach (var usr in connectedClients)
        {
            if (usr.WebSocketID == SocketId)
            {
                return usr;
            }
        }

        return null;
    }

    public User? GetUserProfileFromUserName(string username)
    {
        foreach (var usr in connectedClients)
        {
            if (usr.GetUserName() == username)
            {
                return usr;
            }
        }

        return null;
    }
}