using LibObjects;

namespace MessageServer.Models;

public class UserController
{
	private List<ServerUser> connectedClients = new List<ServerUser>();


	public User? GetUserProfileFromSocketId(int SocketId)
	{
		foreach (var usr in connectedClients) {
			if (usr.GetWebSocketID() == SocketId) {
				return usr;
			}
		}

		return null;
	}
	
	public User? GetUserProfileFromSocketGuid(Guid guid)
	{
		foreach (var usr in connectedClients) {
			if (usr.GetUserGuid() == guid) {
				return usr;
			}
		}
		return null;
	}

	public User? GetUserProfileFromUserName(string username)
	{
		foreach (var usr in connectedClients) {
			if (usr.GetUserName() == username) {
				return usr;
			}
		}

		return null;
	}

	public Guid GetGuidFromSocketId(int index)
	{
		foreach (var usr in connectedClients) {
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

		foreach (var client in connectedClients)
		{
			if (client.GetUserGuid() == user.GetUserGuid())
			{
				connectedClients.Remove(client);
			}
		}
	}

	public int GetWebSocketIdFromUser(User user)
	{
		foreach (var client in connectedClients)
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
		List<User> users = new List<User>();
		foreach (var client in connectedClients)
		{
			users.Add(client);
		}

		return users;
	}

	public bool CreateUser(string name, bool validated, int index, out User? user)
	{
		user = null;
		ServerUser serverUser = new ServerUser(name, validated, index);
		bool unique = userUnique(serverUser);
		if (unique)
		{
			connectedClients.Add(serverUser);
			user = serverUser;
		}
		return unique;
	}

	public bool userUnique(ServerUser user)
	{
		for (var index = 0; index < connectedClients.Count; index++)
		{
			var client = connectedClients[index];
			if (user.GetWebSocketID() == client.GetWebSocketID())
			{
				return false;
			}
		}

		return true;
	}
}