using LibObjects;

namespace MessageServer.Models;

public class UserController
{
	public List<User> connectedClients = new List<User>();


	public User? GetUserProfileFromSocketId(int SocketId)
	{
		foreach (var usr in connectedClients) {
			if (usr.WebSocketID == SocketId) {
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
			if (usr.WebSocketID == index) {
				return usr.GetUserGuid();
			}
		}

		return Guid.Empty;
	}

	public void RemoveUser(User user)
	{
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
				return client.WebSocketID;
			}
		}
		return -1;
	}
}