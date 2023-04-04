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
					User userProfileFromSocketId = _userController.GetUserProfileFromSocketId(index);
					var findAllRoomsWhereUserInRoom = _roomController.FindAllServerRoomsWhereUserInServerRoom(userProfileFromSocketId);
					if (findAllRoomsWhereUserInRoom.Count > 0)
					{
						string userJson = User.GetJsonFromUser(userProfileFromSocketId);
						foreach (var inRoom in findAllRoomsWhereUserInRoom)
						{
							Room room = _roomController.GetServerRoomFromGUID(inRoom);
							var usersInRoom = _roomController.GetUsersInRoom(room);
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
								_roomController.RemoveUserFromServerRoom(userProfileFromSocketId, room);
								_roomController.DestroyServerRoom(inRoom);
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
		string [] messageChunks = ProcessMessageData.UnpackMessageSafe(message);

		var s = (CommunicationTypeEnum)(int.Parse(messageChunks [0]));
		switch (s) {
			case CommunicationTypeEnum.ServerReceiveAuthenticate: //"[ServerReceiveAuthenticate]:{userName}:{passWord}"
				Authenticate(index, messageChunks);
				break;
			
			case CommunicationTypeEnum.ServerReceiveMessageReceivedSuccessfully: //"[ServerReceiveMessageReceivedSuccessfully]:{User.GetJsonFromUser(user)}:{Message}"
				string messageFromUser = ProcessMessageData.GetUserMessageFromMessageFormatStringJsonRoomString( messageChunks,
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
				var m = ProcessMessageData.GetUserMessageFromMessageFormatStringJsonRoomString(messageChunks, out user);
				Console.WriteLine("Sending a Direct Message to:" + m);
				SendCommunicationsToUser(user, m);
				break;

				
				case CommunicationTypeEnum.ServerReceiveRequestSendMessageToAll: //"[ServerReceiveRequestSendMessageToAll]:[MESSAGE]"
				SendMessageToAll(index, message, messageChunks);
				break;

			case CommunicationTypeEnum.ServerReceiveRequestCreateRoom: //"[ServerReceiveRequestCreateRoom]:{roomSize_int}:{(isPublic?"PUBLIC":"PRIVATE")_string}:{meta_string}"
				Room createdRoom = _roomController.CreateNewServerRoom(_userController.GetUserProfileFromSocketId(index), messageChunks);
				String createdRoomJason = Room.GetJsonFromRoom(createdRoom);
				SendRoomCreated(index, createdRoomJason);
				SendRoomJoined(index, createdRoomJason);
				break;

			//TODO: NEEDS VALIDATION AND ERROR HANDLING
			case CommunicationTypeEnum.ServerReceiveRequestAddUserRoom://"[ServerReceiveRequestAddUserRoom]:[ROOM_GUID]:[UserName]"
				User userProfile = _userController.GetUserProfileFromUserName(messageChunks [2]);
				var roomGUID = messageChunks[1];
				Room userAddedToRoom = _roomController.GetServerRoomFromGUID(Guid.Parse(roomGUID));
				string jsonUser = User.GetJsonFromUser(userProfile);
				Console.WriteLine(userProfile.WebSocketID);
				Console.WriteLine(Guid.Parse(messageChunks[1]).ToString());
				Console.WriteLine(User.GetJsonFromUser(userProfile));
				_roomController.AddUserToServerRoom(userProfile, Guid.Parse(roomGUID));
				SendUserJoinedRoom(index, roomGUID, jsonUser);
				SendRoomJoined(userProfile, userAddedToRoom);
				break;
			
			case CommunicationTypeEnum.ServerReceiveSendMessageToRoom: //"[ServerReceiveSendMessageToRoom]:[ROOMID]:[MESSAGE]"
				ReceivedSendMessageToRoom(index, messageChunks);
				break;

			case CommunicationTypeEnum.ServerReceiveRequestUsersListJsonInRoom:
				SendUsersUserListJsonInRoom(index, Guid.Parse(messageChunks [1]));
			break;

			case CommunicationTypeEnum.ServerReceiveRequestRoomListJson: //"[ServerReceiveRequestRoomListJson]"
				SendListOfRoomJson(index);	
			break;

			default:
				SendErrorMessage(index, s, "Command not implemented");
			break;
		}
	}
	

	

	private void ReceivedSendMessageToRoom(int index, string[] messageChunks)
	{
		Guid guid = Guid.Parse(messageChunks[1]);
		Room room = _roomController.GetServerRoomFromGUID(guid);
		User userMessage = _userController.GetUserProfileFromSocketId(index);
		string messageToSend = messageChunks[2];
		foreach (var usr in _roomController.GetUsersInServerRoom(room.GetGuid()))
		{
			SendMessageToRoom(usr, userMessage, room, messageToSend);
		}
	}


	private User? ValidateUser(string[] messageChunks, int index)
	{
		if (messageChunks.Length < 3)
			throw new Exception();

		if (dbManager.ValidateAccount(messageChunks [1], messageChunks [2])) {
			User? tmpUser = new User(messageChunks [1], true, Guid.NewGuid(), index);
			return tmpUser;
		}

		return null;
	}

	private void Authenticate(int index, string[] message)
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
	
	private void SendErrorMessage(int index, CommunicationTypeEnum s, string errorMessage)
	{
		var send = new []
		{
			$"{(int) CommunicationTypeEnum.ClientReceiveErrorResponseFromServer}",
			$"{s}",
			$"{errorMessage}"
		};
		SendMessage(index, send);
	}
	
	private void SendUserLeftRoom(User user, Guid inRoom, string userJson)
	{
		var send = new []
		{
			$"{(int) CommunicationTypeEnum.ClientReceiveUserLeftRoom}",
			$"{inRoom}",
			$"{userJson}"
		};
		SendMessage(user.WebSocketID, send);
	}

	private void SendRoomDestroyed(User user, Room room)
	{
		var send = new []
		{
			$"{(int) CommunicationTypeEnum.ClientReceiveRoomDestroyed}",
			$"{Room.GetJsonFromRoom(room)}"
		};
		SendMessage(user.WebSocketID, send);
	}



	private void SendMessageToAll(int index, string message, string[] messageChunks)
	{
		SendCommsToAll(index,
			$"{messageChunks[1]}");
	}

	private void SendRoomCreated(int index, string createdRoomJason)
	{
		var send = new []
		{
			$"{(int) CommunicationTypeEnum.ClientReceiveRoomCreated}",
			$"{createdRoomJason}"
		};
		SendMessage(index, send);
	}

	private void SendRoomJoined(int index, string createdRoomJason)
	{
		var send = new []
		{
			$"{(int) CommunicationTypeEnum.ClientReceiveJoinedRoom}",
			$"{createdRoomJason}"
		};
		SendMessage(index, send);
	}

	private void SendRoomJoined(User userProfile, Room userAddedToRoom)
	{
		var send = new []
		{
			$"{(int) CommunicationTypeEnum.ClientReceiveJoinedRoom}",
			$"{Room.GetJsonFromRoom(userAddedToRoom)}"
		};
		SendMessage(userProfile.WebSocketID, send);
	}

	private void SendUserJoinedRoom(int index, string roomGUID, string jsonUser)
	{
		var send = new []
		{
			$"{(int) CommunicationTypeEnum.ClientReceiveUserJoinedRoom}",
			$"{roomGUID}",
			$"{jsonUser}"
		};
		SendMessage(index, send);
	}

	private void SendListOfRoomJson(int index)
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
				var send = new []
				{
					$"{(int) CommunicationTypeEnum.ClientReceiveRoomListJsonPaginated}",
					$"{sentNumber}",
					$"{count}-{count+c}",
					$"{Room.GetJsonFromRoomList(list)}"
				};
				SendMessage(index, send);
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

	private void SendUserListJson(int index)
	{
		string users = User.GetJsonFromUsersList(_userController.connectedClients);
		var send = new []
		{
			$"{(int) CommunicationTypeEnum.ClientReceiveUserListJson}",
			$"{users}"
		};
		SendMessage(index, send);
	}

	private void SendCommunicationsToUser(User com, string message)
	{
		
		foreach (var u in _userController.connectedClients) {
			if (u.GetUserName() == com.GetUserName()) {
				var send = new []
				{
					$"{(int) CommunicationTypeEnum.ClientReceiveMessageFromUser}",
					$"{User.GetJsonFromUser(com)}",
					$"{message}"
				};
				SendMessage(u.WebSocketID, send);
			}
		}
	}
	
	private void SendAuthenticationPassed(int index)
	{
		var send = new []
		{
			$"{(int) CommunicationTypeEnum.ClientReceiveAuthenticated}",
			$"OK"
		};
		SendMessage(index, send);
	}

	private void SendAuthenticationFailed(int index)
	{
		var send = new []
		{
			$"{(int) CommunicationTypeEnum.ClientReceiveAuthenticated}",
			$"FAILED"
		};
		SendMessage(index, send);
	}
	
	private void SendRoomListJson(int index, List<Room> rooms)
	{
		var send = new []
		{
			$"{(int) CommunicationTypeEnum.ClientReceiveRoomListJson}",
			$"{Room.GetJsonFromRoomList(rooms)}"
		};
		SendMessage(index, send);
	}
	
	private void SendClientGuid(int index, User? user)
	{
		var send = new []
		{
			$"{(int) CommunicationTypeEnum.ClientReceiveYourGuid}",
			$"{user.GetUserGuid()}"
		};
		SendMessage(index, send);
	}

	private void SendClientWebSocketId(int index)
	{
		var send = new []
		{
			$"{(int) CommunicationTypeEnum.ClientReceiveYourWebsocketId}",
			$"{index}"
		};
		SendMessage(index, send);
	}

	private void SendUser(int index, User? user)
	{
		var send = new []
		{
			$"{(int) CommunicationTypeEnum.ClientReceiveUserInfo}",
			$"{User.GetJsonFromUser(user)}"
		};
		SendMessage(index, send);
	}
	private void SendUserListJsonPaginated(int index, CommunicationTypeEnum command, int sentNumber, int count, int c, List<User> list)
	{
		var send = new []
		{
			$"{(int) command}",
			$"{sentNumber}",
			$"{count}-{count + c}",
			$"{User.GetJsonFromUsersList(list)}"
		};
		SendMessage(index, send);
	}
	
	private void SendUserReceivedMessage(int index, User user, string message)
	{
		User Reciever = _userController.GetUserProfileFromSocketId(index);
		int i = _userController.GetWebSocketIdFromUser(user);
		var send = new []
		{
			$"{(int)CommunicationTypeEnum.ClientReceiveMessageSentSuccessful}",
			$"{User.GetJsonFromUser(Reciever)}",
			$"{message}"
		};
		SendMessage(i, send);
	}
	
	private void SendCommsToAll(int index, string message)
	{
		User user = _userController.GetUserProfileFromSocketId(index);
		for (int i = 0; i < sockets.Length; i++) {
			if (sockets [i] != null && i != index) {
				var send = new []
				{
					$"{(int)CommunicationTypeEnum.ClientReceiveCommunicationToAll}",
					$"{User.GetJsonFromUser(user)}",
					$"{message}"
				};
				SendMessage(index, send);
			}
		}
	}
	
	private void SendUsersUserListJsonInRoom(int index, Guid roomID)
	{
		Room r = _roomController.GetServerRoomFromGUID(roomID);
		var send = new []
		{
			$"{(int)CommunicationTypeEnum.ClientReceiveUsersListJsonInRoom}",
			$"{roomID.ToString()}",
			$"{User.GetJsonFromUsersList(_roomController.GetUsersInRoom(r))}"
		};
		SendMessage(index, send);
	}
	
	private void SendMessageToRoom(User toUser, User? fromUser, Room room, string messageToSend)
	{
		var send = new []
		{
			$"{(int)CommunicationTypeEnum.ClientReceiveRoomMessage}",
			$"{User.GetJsonFromUser(fromUser)}",
			$"{Room.GetJsonFromRoom(room)}",
			$"{messageToSend}"
		};
		SendMessage(toUser.WebSocketID, send);
	}

	private void SendMessage(int index, string[] send)
	{
		string message = ProcessMessageData.BuildMessageSafe(send);

		Console.WriteLine("Index:" + index +  "Socket State: " + sockets[index].State);
		// sockets[index].SendAsync();
		if (sockets[index] == null)
			return;
	
		byte [] buffer = Encoding.UTF8.GetBytes(message);
		// Create a WebSocket message from the buffer
		var webSocketMessage = new ArraySegment<byte>(buffer);
		sockets[index].SendAsync(webSocketMessage, WebSocketMessageType.Text, true, CancellationToken.None);
		
		if (logginEnabled) {
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine($"Sent To Client {index} {_userController.GetUserProfileFromSocketId(index)} Message: {message}");
			Console.ResetColor();
		}
	}
}