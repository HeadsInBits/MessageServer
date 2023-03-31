using MessageServer.Data;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text;
using LibObjects;

namespace MessageServer.Models;

public class WebSocketHandler
{
	private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
	private readonly WebSocket [] sockets = new WebSocket [10];

	private RoomController _roomController = new RoomController();
	private UserController _userController = new UserController();

	DBManager dbManager = new DBManager("rpi4", "MessageServer", "App", "app");

	private bool logginEnabled = true;

	public void AddSocket(WebSocket socket)
	{
		// Find an available slot in the sockets array
		int index = Array.IndexOf(sockets, null);
		if (index >= 0) {
			sockets [index] = socket;
			StartHandling(socket, index);
		}
		else {
			// No available slots, close the socket
			socket.Abort();
		}
	}

	public async Task Stop()
	{
		// Stop handling WebSocket messages
		cancellation.Cancel();
		foreach (var socket in sockets) {
			if (socket != null) {
				await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server shutting down",
					CancellationToken.None);
			}
		}
	}

	private async Task StartHandling(WebSocket socket, int index)
	{
		// Handle WebSocket messages in a separate thread
		var buffer = new byte [32768];
		while (!cancellation.IsCancellationRequested) {

			try {

				var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

				if (result.MessageType == WebSocketMessageType.Close) {
					// Close the socket
					sockets [index] = null;
					Console.WriteLine("Client Disconnected:" + index);
					_userController.connectedClients.Remove(_userController.GetUserProfileFromSocketId(index));
					await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected",
						CancellationToken.None);
				}
				else if (result.MessageType == WebSocketMessageType.Binary ||
						 result.MessageType == WebSocketMessageType.Text) {
					// Handle the message
					var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
					Console.WriteLine($"Received message from client {index}: {message}");

					ProcessMessage(index, message);
				}


			} catch (WebSocketException ex) {
				if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived) {
					await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected", CancellationToken.None);
				}
				// Handle the client disconnection here
			//	sockets [index] = null;
			} catch (Exception ex) {
				Console.WriteLine($"Error receiving message: {ex.Message}");
				// Handle the error here
			}


		}
	}

	private void CommsToUser(string jsonUserData, string message)
	{
		Console.WriteLine(jsonUserData);
		User com = User.GetUserFromJson(jsonUserData);
		foreach (var u in _userController.connectedClients) {
			if (u.GetUserName() == com.GetUserName()) {
				SendMessage(u.WebSocketID, $"RECIEVEMESSAGE:{jsonUserData}:{message}");
			}
		}
	}

	//TODO:validate message is not a server command / send with message i.e. "COMMSTOALLUSERS:USER:MESSAGE"
	private void CommsToAllButSender(int index, string message)
	{
		for (int i = 0; i < sockets.Length; i++) {
			if (sockets [i] != null && i != index) {
				SendMessage(index, message);
			}

		}
	}

	private void SendUsersOfRoom(int index, Guid roomID)
	{
		var msg = new StringBuilder();
		msg.Append("ROOMUSERLIST:");
		msg.Append(":" + _roomController.GetUsersInRoom(roomID).Count);
		if (_roomController.GetUsersInRoom(roomID).Count > 0) {
			foreach (var Usr in _roomController.GetUsersInRoom(roomID)) {
				msg.Append(":" + Usr.GetUserName());
			}
		}

		if (logginEnabled) {
			Console.WriteLine("Sending Room Users:" + msg.ToString());
		}

		SendMessage(index, msg.ToString());
	}
	
	
	private void SendListOfRoomGuids(int index)
	{
		List<Room> roomList = _roomController.GetRoomsList();
		int count = roomList.Count;
		int sentNumber = (int)MathF.Ceiling((float)count / Room.NumberOfRoomsToSendInMessage)-1;
		if (count > Room.NumberOfRoomsToSendInMessage)
		{
			while (count > 0)
			{
				var list = new List<Room>();
				int c = 0;
				for(; c < Room.NumberOfRoomsToSendInMessage && count > 0 ; c++)
				{
					list.Add(roomList[count-1]);
					count--;
				}
				SendRoomsGuidList(index, $"ROOMGUIDLISTPAGI:{sentNumber}:{count}-{count+c}", count, list);
				sentNumber--;
			}
		}
		else
		{
			SendRoomsGuidList(index, $"ROOMGUIDLIST:{count}", count, roomList);
		}
	}
	
	private void SendRoomsGuidList(int index, string command, int count, List<Room> roomList)
	{
		var msg = new StringBuilder();
		msg.Append($"{command}");
		if (count > 0)
		{
			for (var i = 0; i < roomList.Count; i++)
			{
				var room = roomList[i];
				msg.Append($"{room.GetGuid()}{(i+1==roomList.Count?"":"/")}");
			}
		}

		if (logginEnabled)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Sending Room List:" + msg.ToString());
			Console.ResetColor();
		}

		SendMessage(index, msg.ToString());
	}

	private void SendRoomListJson(int index, List<Room> rooms)
	{
		SendMessage(index, "ROOMLIST*JSON:"+Room.GetJsonFromRoomList(rooms));
	}
	
	private void SendListOfRoomJson(int index)
	{
		List<Room> roomList = _roomController.GetRoomsList();
		int count = roomList.Count;
		int sentNumber = (int)MathF.Ceiling((float)count / Room.NumberOfRoomsToSendInMessage)-1;
		if (count > Room.NumberOfRoomsToSendInMessage)
		{
			string command = "ROOMLISTPAGIJSON";
			while (count > 0)
			{
				var list = new List<Room>();
				int c = 0;
				for(; c < Room.NumberOfRoomsToSendInMessage && count > 0 ; c++)
				{
					list.Add(roomList[count-1]);
					count--;
				}
				SendMessage(index, $"{command}:{sentNumber}:{count}-{count+c}:{Room.GetJsonFromRoomList(list)}");
				sentNumber--;
			}
		}
		else
		{
			SendRoomListJson(index, roomList);
		}
	}
	
	private void GetUserListJson(int index)
	{
		List<User> clients = _userController.connectedClients;
		int count = clients.Count;
		int sentNumber = (int)MathF.Ceiling((float)count / User.NumberOfUsersToSendInMessage)-1;
		if (count > User.NumberOfUsersToSendInMessage)
		{
			string command = "USERLISTPAGIJSON";
			while (count > 0)
			{
				var list = new List<User>();
				int c = 0;
				for(; c < User.NumberOfUsersToSendInMessage && count > 0 ; c++)
				{
					list.Add(clients[count-1]);
					count--;
				}
				SendMessage(index, $"{command}:{sentNumber}:{count}-{count+c}:{User.GetJsonFromUsersList(list)}");
				sentNumber--;
			}
		}
		else
		{
			SendUserListJson(index);
		}
	}
	

	private void SendUserListJson(int myIndex)
	{
		var returnMessage = new StringBuilder();

		
		string users = User.GetJsonFromUsersList(_userController.connectedClients);
		returnMessage.Append($"USERLIST:{users}");
		Console.WriteLine("SENDING USER LIST@@ " + returnMessage.ToString());
		_ = Task.Run(() => SendMessage(myIndex, returnMessage.ToString()));
	}

	private bool SendMessage(int index, string message)
	{
		Console.WriteLine("Index:" + index +  "Socket State: " + sockets[index].State);
		// sockets[index].SendAsync();
		if (sockets [index] == null)
			return false;

		byte [] buffer = Encoding.UTF8.GetBytes(message);
		// Create a WebSocket message from the buffer
		var webSocketMessage = new ArraySegment<byte>(buffer);
		sockets[index].SendAsync(webSocketMessage, WebSocketMessageType.Text, true, CancellationToken.None);
		
		if (logginEnabled) {
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine($"Sent To Client {index} {_userController.GetUserProfileFromSocketId(index)} Message: {message}");
			Console.ResetColor();
		}

		
		return true;
	}

	private void ProcessMessage(int index, string message)
	{
		string [] messageChunks = message.Split(':');

		switch (messageChunks [0]) {
			case "AUTHENTICATE": //"AUTHENTICATE:{userName}:{passWord}"
				Authenticate(index, message);
				break;

			case "USERFROMGUID": //"USERFROMGUID:{guid.ToString()}"
				User user = _userController.GetUserProfileFromSocketGuid(Guid.Parse(messageChunks[1]));
				SendMessage(index, $"USERGUID:{User.GetJsonFromUser(user)}");
				break;
			
			case "GETMYID": //TODO: Not implemented on client
				SendMessage(index, "IDIS:" + index);
				break;
			
			case "GETMYGUID": //"GETMYGUID"
				User userGUID = _userController.GetUserProfileFromSocketId(index);
				SendMessage(index, "YOURGUID:" + userGUID.GetUserGuid());
				break;

			case "GETUSERLIST": //"GETUSERLIST"
				GetUserListJson(index);
				break;

			//todo:sender should be sent with message for validation here or CommsToUser, also should have a return format i.e "SENDMESGTOUSER:USER:01:MESSAGE:Hello"
			case "SENDMESGTOUSER": //"SENDMESGTOUSER:{userJson}:{Message}"
				string jsonStrUser = message.Substring(messageChunks[0].Length + 1, message.Length - (messageChunks [^1].Length + messageChunks [0].Length + 2));
				Console.WriteLine("Sending a Direct Message to:" + messageChunks [1]);
				CommsToUser(jsonStrUser, messageChunks [^1]);
				break;

			//TODO:sender should be sent with message for validation here or CommsToAllButSender, also should have a return format i.e "SENDMESGTOALL:USER:01:MESSAGE:Hello"
				case "SENDMESGTOALL": //TODO: Not implemented on client
				CommsToAllButSender(index, messageChunks [1]);
				break;

			case "CREATEROOM": //"CREATEROOM:{roomSize_int}:{(isPublic?"PUBLIC":"PRIVATE")_string}:{meta_string}"
				Room createdRoom = _roomController.CreateNewRoom(_userController.GetUserProfileFromSocketId(index), messageChunks);
				String createdRoomJason = Room.GetJsonFromRoom(createdRoom);
				SendMessage(index, $"ROOMCREATED:{createdRoomJason}");
				SendMessage(index, $"ROOMJOINED:{createdRoomJason}");
				break;

			case "ADDUSERTOROOM"://"ADDUSERTOROOM:[ROOM_GUID]:[UserName]"
				User userProfile = _userController.GetUserProfileFromUserName(messageChunks [^1]);
				string jsonUser = User.GetJsonFromUser(userProfile);
				Console.WriteLine(userProfile.WebSocketID);
				Console.WriteLine(Guid.Parse(messageChunks[1]).ToString());
				Console.WriteLine(User.GetJsonFromUser(userProfile));
				
				_roomController.AddUserToRoom(userProfile, Guid.Parse(messageChunks[1]));
				
				Console.WriteLine("still working");
				SendMessage(index, "USERJOINED:" + jsonUser);
				SendMessage(userProfile.WebSocketID, "ROOMJOINED:" + messageChunks [^1]);
				break;
			
			case "SENDMSGTOROOM": //"SENDMSGTOROOM:[ROOMID]:[MESSAGE]"
				Guid guid = Guid.Parse(messageChunks[1]);
				Room room = _roomController.GetRoomFromGUID(guid);
				User userMessage = _userController.GetUserProfileFromSocketId(index);
				Console.WriteLine();
				foreach (var usr in _roomController.GetUsersInRoom(room.RoomId))
				{
					SendMessage(usr.WebSocketID, $"ROOMMSG:{Room.GetJsonFromRoom(room)}:{userMessage.GetUserGuid()}:{messageChunks[2]}");
				}
				break;

			case "LISTUSERSINROOM"://TODO: Not implemented on client
				SendUsersOfRoom(index, Guid.Parse(messageChunks [1]));
			break;

			case "GETROOMLIST": //TODO: Removed on client deprecated? or implement on client
				SendListOfRoomGuids(index);
			break;

			case "GETROOMLIST*JSON": //"GETROOMLIST*JSON"
				SendListOfRoomJson(index);	
			break;

			default:
			break;
		}
	}

	private void Authenticate(int index, string message)
	{
		User? tmpUser = ValidateUser(message, index);
		if (tmpUser != null)
		{
			tmpUser.WebSocketID = index;

			bool Unique = true;

			foreach (var usr in _userController.connectedClients)
			{
				if (usr.WebSocketID == index)
					Unique = false;
			}

			if (Unique)
			{
				for (int i = 0; i < 61; i++)
				{
					_userController.connectedClients.Add(tmpUser);
				}
				_userController.connectedClients.Add(tmpUser);
				Console.WriteLine("Added User to Client list:" + tmpUser.WebSocketID + "User:" + tmpUser.GetUserName());
			}
			else
			{
				Console.WriteLine("User Already Authenticated: " + tmpUser.WebSocketID + "User:" + tmpUser.GetUserName());
			}

			SendMessage(index, "AUTH:OK");
		}
		else // not authenticated
		{
			SendMessage(index, "AUTH:FAILED");
		}
	}


	private User? ValidateUser(string message, int index)
	{
		
		var messageChunks = message.Split(":");
		if (messageChunks.Length < 3)
			throw new Exception();

		if (dbManager.ValidateAccount(messageChunks [1], messageChunks [2])) {
			User? tmpUser = new User(messageChunks [1], true, Guid.NewGuid(), index);
			return tmpUser;
		}

		return null;
	}
}