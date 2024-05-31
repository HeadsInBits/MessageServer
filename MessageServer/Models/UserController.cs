using System.Collections.Concurrent;
using System.Drawing;
using MySqlX.XDevAPI;
using NetworkObjects;
using TextManagement.Debug;

namespace MessageServer.Models;

public class UserController
{
    private ConcurrentDictionary<Guid, ServerUser> connectedClients = new ConcurrentDictionary<Guid, ServerUser>();


    public User? GetUserProfileFromSocketId(int SocketId)
    {
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        LogInfo.Log($"Searching for user with SocketId: {SocketId}");

        foreach (var usr in connectedClients.Values)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            LogInfo.Log($"Checking user {usr.GetUserName()} with SocketId: {usr.GetWebSocketID()}");

            if (usr.GetWebSocketID() == SocketId)
            {
                LogInfo.Log($"Found user: {usr.GetUserName()} with SocketId: { SocketId}");
                Console.ResetColor();
                return usr;
				
            }
        }

        LogInfo.Log($"No user found with the given SocketId {SocketId}.");
        Console.ResetColor();
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
            LogInfo.Log("Server Attempted To Remove a Null User");
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

    public ServerUser CreateUser(string name, bool validated, int index)
	{
		ServerUser serverUser = new ServerUser(name, validated, index);

		if (userUnique(serverUser))	{
            connectedClients.TryAdd(serverUser.GetUserGuid(), serverUser);
            return serverUser;
        }
        throw new ArgumentException();
	}

    public bool userUnique(ServerUser user)
    {
        foreach(var usr in connectedClients)
        {
            if (user.GetUserName() == usr.Value.GetUserName())
            {
                return false;
            }
        }
        return true;
    }
}