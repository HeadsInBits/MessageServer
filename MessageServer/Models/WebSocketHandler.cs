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

	private async Task StartHandling(WebSocket socket, int index)
	{
		// Handle WebSocket messages in a separate thread
		var buffer = new byte [32768];
		while (!cancellation.IsCancellationRequested) {

			try {

				var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

				if (result.MessageType == WebSocketMessageType.Close) {
					// Close the socket
					User userProfileFromSocketId = _userController.GetUserProfileFromSocketId(index);
					var findAllRoomsWhereUserInRoom = _roomController.FindAllRoomsWhereUserInRoom(userProfileFromSocketId);
					if (findAllRoomsWhereUserInRoom.Count > 0)
					{
						string userJson = User.GetJsonFromUser(userProfileFromSocketId);
						foreach (var inRoom in findAllRoomsWhereUserInRoom)
						{
							Room room = _roomController.GetRoomFromGUID(inRoom);
							var usersInRoom = room.GetUsersInRoom();
							bool isOwner = room.GetCreator() == userProfileFromSocketId;
							if (usersInRoom.Contains(userProfileFromSocketId))
							{
								foreach (var user in usersInRoom)
								{
									if (inRoom != userProfileFromSocketId.GetUserGuid())
									{
										SendUserLeftRoom(user, inRoom, userJson);
										if (isOwner)
										{
											SendRoomDestroyed(user, room);
										}
									}
								}
								room.RemoveUserFromRoom(userProfileFromSocketId);
								_roomController.DestroyRoom(inRoom);
							}
						}
					}
					_userController.RemoveUser(userProfileFromSocketId);
					sockets [index] = null;
					Console.WriteLine("Client Disconnected:" + index);
					_userController.connectedClients.Remove(userProfileFromSocketId);
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

	

	private void ProcessMessage(int index, string message)
	{
		string [] messageChunks = message.Split(':');

		var s = (CommunicationTypeEnum)(int.Parse(messageChunks [0]));
		switch (s) {
			case CommunicationTypeEnum.ServerReceiveAuthenticate: //"[ServerReceiveAuthenticate]:{userName}:{passWord}"
				Authenticate(index, message);
				break;
			
			case CommunicationTypeEnum.ServerReceiveMessageReceivedSuccessfully: //"[ServerReceiveMessageReceivedSuccessfully]:{User.GetJsonFromUser(user)}:{Message}"
				string messageFromUser = ProcessMessageData.GetUserMessageFromMessageFormatStringJsonRoomString(message, messageChunks,
					out User sender);
				SendUserReceivedMessage(index, sender, messageFromUser);
				break;

			case CommunicationTypeEnum.ServerReceiveRequestUserFromGuid: //"[ServerReceiveRequestUserFromGuid]:{guid.ToString()}"
				User user = _userController.GetUserProfileFromSocketGuid(Guid.Parse(messageChunks[1]));
				SendUser(index, user);
				break;
			
			case CommunicationTypeEnum.ServerReceiveRequestClientWebSocketId: //TODO: Not implemented on client maybe not needed
				SendClientWebSocketId(index);
				break;
			
			case CommunicationTypeEnum.ServerReceiveRequestClientGuid: //"[ServerReceiveRequestClientGuid]"
				User userGUID = _userController.GetUserProfileFromSocketId(index);
				SendClientGuid(index, userGUID);
				break;

			case CommunicationTypeEnum.ServerReceiveRequestUserListJson: //"[ServerReceiveRequestUserListJson]"
				GetUserListJson(index);
				break;
			
			case CommunicationTypeEnum.ServerReceiveRequestSendMessageToUser: //"[ServerReceiveRequestSendMessageToUser]:{userJson}:{Message}"
				var m = messageChunks [^1];
				string jsonStrUser = message.Substring(messageChunks[0].Length + 1, message.Length - (m.Length + messageChunks [0].Length + 2));
				Console.WriteLine("Sending a Direct Message to:" + m);
				SendCommsToUser(jsonStrUser, m);
				break;

				//"[ServerReceiveRequestSendMessageToAll]:[USERNAME_STRING]:[MESSAGE]"
				case CommunicationTypeEnum.ServerReceiveRequestSendMessageToAll: //TODO: Not implemented on client
				SendMessageToAll(index, message, messageChunks);
				break;

			case CommunicationTypeEnum.ServerReceiveRequestCreateRoom: //"[ServerReceiveRequestCreateRoom]:{roomSize_int}:{(isPublic?"PUBLIC":"PRIVATE")_string}:{meta_string}"
				Room createdRoom = _roomController.CreateNewRoom(_userController.GetUserProfileFromSocketId(index), messageChunks);
				String createdRoomJason = Room.GetJsonFromRoom(createdRoom);
				SendRoomCreated(index, createdRoomJason);
				SendRoomJoined(index, createdRoomJason);
				break;

			//TODO: NEEDS VALIDATION AND ERROR HANDLING
			case CommunicationTypeEnum.ServerReceiveRequestAddUserRoom://"[ServerReceiveRequestAddUserRoom]:[ROOM_GUID]:[UserName]"
				User userProfile = _userController.GetUserProfileFromUserName(messageChunks [2]);
				var roomGUID = messageChunks[1];
				Room userAddedToRoom = _roomController.GetRoomFromGUID(Guid.Parse(roomGUID));
				string jsonUser = User.GetJsonFromUser(userProfile);
				Console.WriteLine(userProfile.WebSocketID);
				Console.WriteLine(Guid.Parse(messageChunks[1]).ToString());
				Console.WriteLine(User.GetJsonFromUser(userProfile));
				_roomController.AddUserToRoom(userProfile, Guid.Parse(roomGUID));
				SendUserJoinedRoom(index, roomGUID, jsonUser);
				SendRoomJoined(userProfile, userAddedToRoom);
				break;
			
			case CommunicationTypeEnum.ServerReceiveSendMessageToRoom: //"[ServerReceiveSendMessageToRoom]:[ROOMID]:[MESSAGE]"
				Guid guid = Guid.Parse(messageChunks[1]);
				Room room = _roomController.GetRoomFromGUID(guid);
				User userMessage = _userController.GetUserProfileFromSocketId(index);
				string messageToSend = message.Substring(messageChunks[0].Length + messageChunks[1].Length + 2);
				messageToSend = LibObjects.ProcessMessageData.SendSafe(messageToSend);
				Console.WriteLine();
				foreach (var usr in _roomController.GetUsersInRoom(room.GetGuid()))
				{
					SendMessageToRoom(usr, userMessage, room, messageToSend);
				}
				break;

			case CommunicationTypeEnum.ServerReceiveRequestUsernamesInRoom://TODO: Not implemented on client
				SendUsersUsernameInRoom(index, Guid.Parse(messageChunks [1]));
			break;

			case CommunicationTypeEnum.ServerReceiveRequestRoomGuidList: //TODO: Removed on client deprecated? or implement on client
				SendListOfRoomGuids(index);
			break;

			case CommunicationTypeEnum.ServerReceiveRequestRoomListJson: //"[ServerReceiveRequestRoomListJson]"
				SendListOfRoomJson(index);	
			break;

			default:
			break;
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
				_userController.connectedClients.Add(tmpUser);
				Console.WriteLine("Added User to Client list:" + tmpUser.WebSocketID + "User:" + tmpUser.GetUserName());
			}
			else
			{
				Console.WriteLine("User Already Authenticated: " + tmpUser.WebSocketID + "User:" + tmpUser.GetUserName());
			}

			SendAuthenticationPassed(index);
		}
		else // not authenticated
		{
			SendAuthenticationFailed(index);
		}
	}
	
	private void SendUserLeftRoom(User user, Guid inRoom, string userJson)
	{
		SendMessage(user.WebSocketID, $"{(int)CommunicationTypeEnum.ClientReceiveUserLeftRoom}:{inRoom}:{userJson}");
	}

	private void SendRoomDestroyed(User user, Room room)
	{
		SendMessage(user.WebSocketID, $"{(int)CommunicationTypeEnum.ClientReceiveRoomDestroyed}:{Room.GetJsonFromRoom(room)}");
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
				SendRoomsGuidList(index, $"{(int)CommunicationTypeEnum.ClientReceiveRoomGuidListPaginated}:{sentNumber}:{count}-{count+c}", count, list);
				sentNumber--;
			}
		}
		else
		{
			SendRoomsGuidList(index, $"{(int)CommunicationTypeEnum.ClientReceiveRoomGuidList}:{count}", count, roomList);
		}
	}
	
	
	
	private void SendMessageToAll(int index, string message, string[] messageChunks)
	{
		SendCommsToAllButSender(index,
			$"{(int)CommunicationTypeEnum.ClientReceiveCommunicationToAll}:{_userController.GetUserProfileFromSocketId(index).GetUserName()}:{message.Substring(messageChunks[0].Length + messageChunks[1].Length + 2)}");
	}

	private void SendRoomCreated(int index, string createdRoomJason)
	{
		SendMessage(index, $"{(int)CommunicationTypeEnum.ClientReceiveRoomCreated}:{createdRoomJason}");
	}

	private void SendRoomJoined(int index, string createdRoomJason)
	{
		SendMessage(index, $"{(int)CommunicationTypeEnum.ClientReceiveJoinedRoom}:{createdRoomJason}");
	}

	private void SendRoomJoined(User userProfile, Room userAddedToRoom)
	{
		SendMessage(userProfile.WebSocketID, $"{(int)CommunicationTypeEnum.ClientReceiveJoinedRoom}:{Room.GetJsonFromRoom(userAddedToRoom)}");
	}

	private void SendUserJoinedRoom(int index, string roomGUID, string jsonUser)
	{
		SendMessage(index, $"{(int)CommunicationTypeEnum.ClientReceiveUserJoinedRoom}:{roomGUID}:{jsonUser}");
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

	private void SendListOfRoomJson(int index)
	{
		List<Room> roomList = _roomController.GetRoomsList();
		int count = roomList.Count;
		int sentNumber = (int)MathF.Ceiling((float)count / Room.NumberOfRoomsToSendInMessage)-1;
		if (count > Room.NumberOfRoomsToSendInMessage)
		{
			string command = ((int)CommunicationTypeEnum.ClientReceiveUserListJson).ToString();
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
			CommunicationTypeEnum command = CommunicationTypeEnum.ClientReceiveUserListJsonPaginated;
			while (count > 0)
			{
				var list = new List<User>();
				int c = 0;
				for(; c < User.NumberOfUsersToSendInMessage && count > 0 ; c++)
				{
					list.Add(clients[count-1]);
					count--;
				}
				SendUserListJsonPaginated(index, command, sentNumber, count, c, list);
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
		returnMessage.Append($"{(int)CommunicationTypeEnum.ClientReceiveUserListJson}:{users}");
		Console.WriteLine("SENDING USER LIST@@ " + returnMessage.ToString());
		_ = Task.Run(() => SendMessage(myIndex, returnMessage.ToString()));
	}

	private void SendCommsToUser(string jsonUserData, string message)
	{
		Console.WriteLine(jsonUserData);
		User com = User.GetUserFromJson(jsonUserData);
		foreach (var u in _userController.connectedClients) {
			if (u.GetUserName() == com.GetUserName()) {
				SendMessage(u.WebSocketID, $"{(int)CommunicationTypeEnum.ClientReceiveMessageFromUser}:{jsonUserData}:{message}");
			}
		}
	}
	
	private void SendAuthenticationPassed(int index)
	{
		SendMessage(index, $"{(int)CommunicationTypeEnum.ClientReceiveAuthenticated}:OK");
	}

	private void SendAuthenticationFailed(int index)
	{
		SendMessage(index, $"{(int)CommunicationTypeEnum.ClientReceiveAuthenticated}:FAILED");
	}
	
	private void SendRoomListJson(int index, List<Room> rooms)
	{
		SendMessage(index, $"{(int)CommunicationTypeEnum.ClientReceiveRoomListJson}:{Room.GetJsonFromRoomList(rooms)}");
	}
	
	private void SendClientGuid(int index, User? userGUID)
	{
		SendMessage(index, $"{(int)CommunicationTypeEnum.ClientReceiveYourGuid}:{userGUID.GetUserGuid()}");
	}

	private void SendClientWebSocketId(int index)
	{
		SendMessage(index, $"{(int)CommunicationTypeEnum.ClientReceiveYourWebsocketId}:{index}");
	}

	private void SendUser(int index, User? user)
	{
		SendMessage(index, $"{(int)CommunicationTypeEnum.ClientReceiveUserInfo}:{User.GetJsonFromUser(user)}");
	}
	private void SendUserListJsonPaginated(int index, CommunicationTypeEnum command, int sentNumber, int count, int c, List<User> list)
	{
		SendMessage(index, $"{(int)command}:{sentNumber}:{count}-{count + c}:{User.GetJsonFromUsersList(list)}");
	}
	
	private void SendUserReceivedMessage(int index, User user, string message)
	{
		User Reciever = _userController.GetUserProfileFromSocketId(index);
		int i = _userController.GetWebSocketIdFromUser(user);
		SendMessage(i, $"{(int)CommunicationTypeEnum.ClientReceiveMessageSentSuccessful}:{User.GetJsonFromUser(Reciever)}:{message}");
	}
	
	//TODO:validate message is not a server command / send with message i.e. "COMMSTOALLUSERS:USER:MESSAGE"
	private void SendCommsToAllButSender(int index, string message)
	{
		for (int i = 0; i < sockets.Length; i++) {
			if (sockets [i] != null && i != index) {
				SendMessage(index, $"{(int)CommunicationTypeEnum.ClientReceiveCommunicationToAllButSender}:{message}");
			}
		}
	}
	
	//TODO:Send user with message implement on the client
	private void SendUsersUsernameInRoom(int index, Guid roomID)
	{
		var msg = new StringBuilder();
		msg.Append((int)CommunicationTypeEnum.ClientReceiveUsernameOfUsersInRoom);
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
	
	private void SendMessageToRoom(User usr, User? userMessage, Room room, string messageToSend)
	{
		SendMessage(usr.WebSocketID, $"{(int)CommunicationTypeEnum.ClientReceiveRoomMessage}:{userMessage.GetUserGuid()}:{Room.GetJsonFromRoom(room)}:{messageToSend}");
	}
}