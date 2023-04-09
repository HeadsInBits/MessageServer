using LibObjects;
using System.Collections.Concurrent;

namespace MessageServer.Models;

public class UserController
{
    private ConcurrentDictionary<Guid, ServerUser> connectedClients = new ConcurrentDictionary<Guid, ServerUser>();


    public User? GetUserProfileFromSocketId(int SocketId)
    {
        Console.WriteLine($"Searching for user with SocketId: {SocketId}");

        foreach (var usr in connectedClients.Values)
        {
            Console.WriteLine($"Checking user {usr.GetUserName()} with SocketId: {usr.GetWebSocketID()}");

            if (usr.GetWebSocketID() == SocketId)
            {
                Console.WriteLine($"Found user: {usr.GetUserName()}");
                return usr;
            }
        }

        Console.WriteLine("No user found with the given SocketId.");
        return null;
    }
    public User? GetUserProfileFromSocketGuid(Guid guid)
	{
		foreach (var usr in connectedClients.Values) {
			if (usr.GetUserGuid() == guid) {
				return usr;
			}
		}
		return null;
	}

	public User? GetUserProfileFromUserName(string username)
	{
		foreach (var usr in connectedClients.Values) {
			if (usr.GetUserName() == username) {
				return usr;
			}
		}

		return null;
	}

	public Guid GetGuidFromSocketId(int index)
	{
		foreach (var usr in connectedClients.Values) {
			if (usr.GetWebSocketID() == index) {
				return usr.GetUserGuid();
			}
		}

		return Guid.Empty;
	}

    public void RemoveUser(User? user)
    {
        if (user == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Server Attempted To Remove a Null User");
            Console.ResetColor();
            return;
        }

        // Remove the user by their GUID
        connectedClients.TryRemove(user.GetUserGuid(), out _);
    }
    public int GetWebSocketIdFromUser(User user)
	{
		foreach (var client in connectedClients.Values)
		{
			if (client.GetUserGuid() == user.GetUserGuid())
			{
				return client.GetWebSocketID();
			}
		}
		return -1;
	}

    public List<User> GetConnectedClients()
    {
        return connectedClients.Values.Cast<User>().ToList();
    }

    public bool CreateUser(string name, bool validated, int index, out User? user)
	{
		user = null;
		ServerUser serverUser = new ServerUser(name, validated, index);
		bool unique = userUnique(serverUser);
		if (unique)
		{
            connectedClients.TryAdd(serverUser.GetUserGuid(), serverUser);
            user = serverUser;
		}
		return unique;
	}

    public bool userUnique(ServerUser user)
    {
        return !connectedClients.Values.Any(client => user.GetWebSocketID() == client.GetWebSocketID());
    }
}