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
		for (var index = 0; index < sockets.Length; index++)
		{
			var socket = sockets[index];
			if (socket != null)
			{
				await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server shutting down",
					CancellationToken.None);
			}
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

	private async Task StartHandling(WebSocket socket, int index)
	{
		// Handle WebSocket messages in a separate thread
		var buffer = new byte [32768];
		while (!cancellation.IsCancellationRequested) {

			try {

				var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

				if (result.MessageType == WebSocketMessageType.Close) {
					// Close the socket
					User userDisconnected = _userController.GetUserProfileFromSocketId(index);
					var findAllRoomsWhereUserInRoom = _roomController.FindAllServerRoomsWhereUserInServerRoom(userDisconnected);
					if (findAllRoomsWhereUserInRoom.Count > 0)
					{
						string userJson = User.GetJsonFromUser(userDisconnected);
						for (var i = 0; i < findAllRoomsWhereUserInRoom.Count; i++)
						{
							var inRoom = findAllRoomsWhereUserInRoom[i];
							Room room = _roomController.GetServerRoomFromGUID(inRoom);
							var usersInRoom = _roomController.GetUsersInRoom(room);
							bool isOwner = room.GetCreator() == userDisconnected.GetUserName();
							if (_roomController.IsInRoom(room, userDisconnected))
							{
								_roomController.RemoveUserFromServerRoom(userDisconnected, room);
								for (var index1 = 0; index1 < usersInRoom.Count; index1++)
								{
									var user = usersInRoom[index1];
									if (inRoom != userDisconnected.GetUserGuid())
									{
										SendUserLeftRoom(user, inRoom, userJson);
									}
								}

								if (isOwner)
								{
									RoomDestroyed(room);
								}
							}
						}
					}
					_userController.RemoveUser(userDisconnected);
					sockets [index] = null;
					for (var i = 0; i < _userController.connectedClients.Count; i++)
					{
						var user = _userController.connectedClients[i];
						SendUserDisconnected(userDisconnected, user);
					}

					Console.WriteLine("Client Disconnected:" + index);
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

			} catch (Exception ex) {
				Console.WriteLine($"Error receiving message: {ex.Message}");
				// Handle the error here
			}


		}
	}

	

	private void ProcessMessage(int index, string message)
	{
		string [] messageChunks = ProcessMessageData.UnpackMessageSafe(message);
		Enum.TryParse(messageChunks[0], out CommunicationTypeEnum s);
    
		switch (s) {
			
			//"[ServerReceiveAuthenticate]:[USER_NAME_STRING]:[PASSWORD_STRING]"
			case CommunicationTypeEnum.ServerReceiveAuthenticate: 
				ReceivedAuthenticate(index, messageChunks);
				break;
			
			//"[ServerReceiveMessageReceivedSuccessfully]:[USER_JSON]:[MESSAGE_STRING]"
			case CommunicationTypeEnum.ServerReceiveMessageReceivedSuccessfully: 
				ReceivedMessageSentSuccessfully(index, messageChunks);
				break;

			//"[ServerReceiveRequestUserFromGuid]:[USER_GUID]"
			case CommunicationTypeEnum.ServerReceiveRequestUserFromGuid: 
				ReceivedRequestUserFromGuid(index, messageChunks);
				break;

			//"[ServerReceiveRequestClientGuid]"
			case CommunicationTypeEnum.ServerReceiveRequestClientGuid: 
				ReceivedRequestClientGuid(index);
				break;

			//"[ServerReceiveRequestUserListJson]"
			case CommunicationTypeEnum.ServerReceiveRequestUserListJson: 
				ReceivedRequestUserListJson(index);
				break;
			
			//"[ServerReceiveRequestSendMessageToUser]:[USER_JSON]:[MESSAGE_STRING]"
			case CommunicationTypeEnum.ServerReceiveRequestSendMessageToUser: 
				ReceivedRequestSendMessageToUser(messageChunks, index);
				break;

			//"[ServerReceiveRequestSendMessageToAll]:[MESSAGE_STRING]"
			case CommunicationTypeEnum.ServerReceiveRequestSendMessageToAll: 
				ReceivedRequestSendMessageToAll(index, messageChunks);
				break;

			//"[ServerReceiveRequestCreateRoom]:[ROOM_SIZE_INT]:[(Public?PUBLIC:PRIVATE)_STRING]:[META_STRING]:[ROOM_NAME_STRING]"
			case CommunicationTypeEnum.ServerReceiveRequestCreateRoom: 
				ReceivedRequestCreateRoom(index, messageChunks);
				break;
			
			//"[ServerReceiveRequestAddUserToRoom]:[ROOM_GUID]:[USER_NAME_STRING]"
			case CommunicationTypeEnum.ServerReceiveRequestAddUserToRoom:
				ReceivedRequestAddUserToRoom(index, messageChunks, s);
				break;
			
			//"[ServerReceiveSendMessageToRoom]:[ROOM_GUID]:[MESSAGE_STRING]"
			case CommunicationTypeEnum.ServerReceiveSendMessageToRoom: 
				ReceivedSendMessageToRoom(index, messageChunks, s);
				break;

			//"[ServerReceiveRequestUsersListJsonInRoom]:[ROOM_GUID]"
			case CommunicationTypeEnum.ServerReceiveRequestUsersListJsonInRoom:
				ReceivedRequestUsersListJsonInRoom(index, Guid.Parse(messageChunks [1]));
			break;

			//"[ServerReceiveRequestRoomListJson]"
			case CommunicationTypeEnum.ServerReceiveRequestRoomListJson: 
				ReceivedRequestRoomListJson(index);	
			break;
			
			//"[ServerReceiveRequestLockRoom]:[ROOM_JSON]"
			case CommunicationTypeEnum.ServerReceiveRequestLockRoom: 
				ReceivedRequestLockRoom(index, messageChunks, s, true);	
				break;
			
			//"[ServerReceiveRequestUnlockRoom]:[ROOM_JSON]"
			case CommunicationTypeEnum.ServerReceiveRequestUnlockRoom: 
				ReceivedRequestLockRoom(index, messageChunks, s, false);	
				break;
			
			//"[ServerReceiveRequestRemoveUserFromRoom]:[USER_JSON]:[ROOM_JSON]"
			case CommunicationTypeEnum.ServerReceiveRequestRemoveUserFromRoom: 
				ReceivedRequestRemoveUserFromRoom(index, messageChunks, s);	
				break;
			
			//"[ServerReceiveRequestBanUserFromRoom]:[USER_JSON]:[ROOM_JSON]"
			case CommunicationTypeEnum.ServerReceiveRequestBanUserFromRoom: 
				ReceivedRequestBanUserFromRoom(index, messageChunks, s);	
				break;
			
			//"[ServerReceiveRequestRemoveBanFromUserInRoom]:[USER_JSON]:[ROOM_JSON]"
			case CommunicationTypeEnum.ServerReceiveRequestRemoveBanFromUserInRoom: 
				ReceivedRequestRemoveBanFromUserInRoom(index, messageChunks, s);	
				break;
			
			//"[ServerReceiveRequestApproveUserFromRoom]:[USER_JSON]:[ROOM_JSON]"
			case CommunicationTypeEnum.ServerReceiveRequestApproveUserFromRoom: 
				ReceivedRequestApproveUserFromRoom(index, messageChunks, s);	
				break;
			
			//"[ServerReceiveRequestRemoveApproveFromUserInRoom]:[USER_JSON]:[ROOM_JSON]"
			case CommunicationTypeEnum.ServerReceiveRequestRemoveApproveFromUserInRoom: 
				ReceivedRequestRemoveApproveFromUserInRoom(index, messageChunks, s);	
				break;
			
			//"[ServerReceiveRequestRoomBannedUserList]:[ROOM_JSON]"
			case CommunicationTypeEnum.ServerReceiveRequestRoomBannedUserList: 
				ReceivedRequestBannedUsersListJsonInRoom(index, Guid.Parse(messageChunks [1]));
				break;
			
			//"[ServerReceiveRequestRoomApprovedUserList]:[ROOM_JSON]"
			case CommunicationTypeEnum.ServerReceiveRequestRoomApprovedUserList: 
				ReceivedRequestApprovedUsersListJsonInRoom(index, Guid.Parse(messageChunks [1]));	
				break;

			default:
				SendErrorMessage(index, s, "Command not implemented");
			break;
		}
	}
	
	void ReceivedRequestRemoveUserFromRoom(int index, string[] messageChunks, CommunicationTypeEnum com)
	{
		User requester = _userController.GetUserProfileFromSocketId(index);
		Room room = ProcessMessageData.GetUserRoomFromMessageFormatStringJsonUserJsonRoom(messageChunks, out User user);
		if (!_roomController.IsCreatorOfRoom(room, requester) && 
		    !(_roomController.IsInRoom(room, user) && user.GetUserName() == requester.GetUserName()))
		{
			SendErrorMessage(index, com, $"You do not own Room {room.GetRoomName()}");
			return;
		}
		
		_roomController.RemoveUserFromServerRoom(user, room);

		var us = _roomController.GetUsersInRoom(room);
		for (var i = 0; i < us.Count; i++)
		{
			var u = us[i];
			SendUserLeftRoom(u, room.GetGuid(), User.GetJsonFromUser(user));
		}

		if (requester.GetUserName() != user.GetUserName())
		{
			SendYouWhereRemovedFromTheRoom(user, room);
		}
		
		SendRoomLeft(user,room);

		if (_roomController.IsCreatorOfRoom(room, user))
		{
			RoomDestroyed(room);
		}
	}

	

	private void RoomDestroyed(Room room)
	{
		var us = _roomController.GetUsersInRoom(room);
		for (var index = 0; index < us.Count; index++)
		{
			var u = us[index];
			SendRoomDestroyed(u, room);
			SendRoomLeft(u, room);
		}

		_roomController.DestroyServerRoom(room.GetGuid());
	}

	void ReceivedRequestBanUserFromRoom(int index, string[] messageChunks, CommunicationTypeEnum com)
	{
		User requester = _userController.GetUserProfileFromSocketId(index);
		Room room = ProcessMessageData.GetUserRoomFromMessageFormatStringJsonUserJsonRoom(messageChunks, out User user);
		if (!_roomController.RoomExists(room.GetGuid()))
		{
			SendErrorMessage(index, com, $"Room {room.GetGuid()} does not exist");
			return;
		}
		if (!_roomController.IsCreatorOfRoom(room, requester))
		{
			SendErrorMessage(index, com, $"You do not own Room {room.GetRoomName()}");
			return;
		}
		SendYouHaveBeenBannedFromRoom(user, room);
		_roomController.AddUserToBanListInServerRoom(user, room);
	}
	
	void ReceivedRequestRemoveBanFromUserInRoom(int index, string[] messageChunks, CommunicationTypeEnum com)
	{
		User requester = _userController.GetUserProfileFromSocketId(index);
		Room room = ProcessMessageData.GetUserRoomFromMessageFormatStringJsonUserJsonRoom(messageChunks, out User user);
		if (!_roomController.RoomExists(room.GetGuid()))
		{
			SendErrorMessage(index, com, $"Room {room.GetGuid()} does not exist");
			return;
		}
		if (!_roomController.IsCreatorOfRoom(room, requester))
		{
			SendErrorMessage(index, com, $"You do not own Room {room.GetRoomName()}");
			return;
		}
		SendYouAreNoLongerBannedFromRoom(user, room);
		_roomController.RemoveUserFromBanListInServerRoom(user, room);
	}
	
	void ReceivedRequestApproveUserFromRoom(int index, string[] messageChunks, CommunicationTypeEnum com)
	{
		User requester = _userController.GetUserProfileFromSocketId(index);
		Room room = ProcessMessageData.GetUserRoomFromMessageFormatStringJsonUserJsonRoom(messageChunks, out User user);
		if (!_roomController.RoomExists(room.GetGuid()))
		{
			SendErrorMessage(index, com, $"Room {room.GetGuid()} does not exist");
			return;
		}
		if (!_roomController.IsCreatorOfRoom(room, requester))
		{
			SendErrorMessage(index, com, $"You do not own Room {room.GetRoomName()}");
			return;
		}
		SendYouHaveBeenApprovedForRoom(user, room);
		_roomController.ApproveUserFromRoom(user, room);
	}
	
	void ReceivedRequestRemoveApproveFromUserInRoom(int index, string[] messageChunks, CommunicationTypeEnum com)
	{
		User requester = _userController.GetUserProfileFromSocketId(index);
		Room room = ProcessMessageData.GetUserRoomFromMessageFormatStringJsonUserJsonRoom(messageChunks, out User user);
		if (!_roomController.RoomExists(room.GetGuid()))
		{
			SendErrorMessage(index, com, $"Room {room.GetGuid()} does not exist");
			return;
		}
		if (!_roomController.IsCreatorOfRoom(room, requester))
		{
			SendErrorMessage(index, com, $"You do not own Room {room.GetRoomName()}");
			return;
		}
		SendYouAreNoLongerApprovedForRoom(user, room);
		_roomController.RemoveUserFromApproveListInServerRoom(user, room);
	}

	private void ReceivedRequestLockRoom(int index, string[] messageChunks, CommunicationTypeEnum com, bool lockOn)
	{
		Room room = ProcessMessageData.GetRoomFromMessageFormatStringRoomJson(messageChunks);
		User user = _userController.GetUserProfileFromSocketId(index);
		if (!_roomController.RoomExists(room.GetGuid()))
		{
			SendErrorMessage(index, com, $"Room {room.GetGuid()} does not exist");
			return;
		}
		if (!_roomController.TryLockRoom(room, user, lockOn))
		{
			SendErrorMessage(index, com, $"You do not own Room {room.GetRoomName()}");
		}
	}

	private void ReceivedRequestAddUserToRoom(int index, string[] messageChunks, CommunicationTypeEnum com)
	{
		var roomGuid = Guid.Parse(messageChunks[1]);
		if (!_roomController.RoomExists(roomGuid))
		{
			SendErrorMessage(index, com, $"Room {roomGuid} does not exist");
			return;
		}
		User userProfile = _userController.GetUserProfileFromUserName(messageChunks[2]);
		User requestedByUser = _userController.GetUserProfileFromSocketId(index);
		
		Room userAddedToRoom = _roomController.GetServerRoomFromGUID(roomGuid);
		string jsonUser = User.GetJsonFromUser(userProfile);
		var addUserToServerRoomStatus = _roomController.AddUserToServerRoom(userProfile, roomGuid, requestedByUser);
		switch (addUserToServerRoomStatus)
		{
			case Room.RoomStatusCodes.Ok:
				SendRoomJoined(userProfile, userAddedToRoom);
				var room = _roomController.GetUsersInRoom(userAddedToRoom);
				for (var i = 0; i < room.Count; i++)
				{
					var user = room[i];
					if (user.GetUserName() == userProfile.GetUserName()) continue;
					int socket = _userController.GetWebSocketIdFromUser(user);
					SendUserJoinedRoom(socket, roomGuid, jsonUser);
				}

				break;
			case Room.RoomStatusCodes.Banned:
				SendErrorMessage(index, com, $"You are banned from room {userAddedToRoom.GetRoomName()}");
				break;
			case Room.RoomStatusCodes.RoomLocked:
				SendErrorMessage(index, com, $"Room {userAddedToRoom.GetRoomName()} is locked and you can't join right now");
				break;
			case Room.RoomStatusCodes.AlreadyJoined:
				SendErrorMessage(index, com, $"You have already joined room {userAddedToRoom.GetRoomName()}");
				break;
			case Room.RoomStatusCodes.Full:
				SendErrorMessage(index, com, $"Room {userAddedToRoom.GetRoomName()} is full");
				break;
			case Room.RoomStatusCodes.Private:
				SendErrorMessage(index, com, $"You don't have permission to join room {userAddedToRoom.GetRoomName()}");
				break;
			default:
				SendErrorMessage(index, com, $"Something went wrong joining room {userAddedToRoom.GetRoomName()}");
				break;
		}
	}

	private void ReceivedRequestCreateRoom(int index, string[] messageChunks)
	{
		Room createdRoom =
			_roomController.CreateNewServerRoom(_userController.GetUserProfileFromSocketId(index), messageChunks);
		String createdRoomJason = Room.GetJsonFromRoom(createdRoom);
		SendRoomCreated(index, createdRoomJason);
		SendRoomJoined(index, createdRoomJason);
	}

	private void ReceivedRequestSendMessageToUser(string[] messageChunks, int index)
	{
		var m = ProcessMessageData.GetUserMessageFromMessageFormatStringJsonUserString(messageChunks, out User user);
		Console.WriteLine("Sending a Direct Message to:" + m);
		SendCommunicationsToUser(user, m, index);
	}

	private void ReceivedRequestClientGuid(int index)
	{
		User userGUID = _userController.GetUserProfileFromSocketId(index);
		SendClientGuid(index, userGUID);
	}

	private void ReceivedRequestUserFromGuid(int index, string[] messageChunks)
	{
		User user = _userController.GetUserProfileFromSocketGuid(Guid.Parse(messageChunks[1]));
		SendUser(index, user);
	}

	private void ReceivedMessageSentSuccessfully(int index, string[] messageChunks)
	{
		string messageFromUser = ProcessMessageData.GetUserMessageFromMessageFormatStringJsonUserString(messageChunks,
			out User sender);
		SendUserReceivedMessage(index, sender, messageFromUser);
	}


	private void ReceivedSendMessageToRoom(int index, string[] messageChunks, CommunicationTypeEnum com)
	{
		Guid guid = Guid.Parse(messageChunks[1]);
		if (!_roomController.RoomExists(guid))
		{
			SendErrorMessage(index, com, $"Room {guid} does not exist");
			return;
		}
		Room room = _roomController.GetServerRoomFromGUID(guid);
		User userMessage = _userController.GetUserProfileFromSocketId(index);
		string messageToSend = messageChunks[2];
		var usrs = _roomController.GetUsersInServerRoom(room.GetGuid());
		for (var i = 0; i < usrs.Count; i++)
		{
			var usr = usrs[i];
			SendMessageToRoom(usr, userMessage, room, messageToSend);
		}
	}

	private void ReceivedRequestRoomListJson(int index)
	{
		List<Room> roomList = _roomController.GetRoomsList();
		int count = roomList.Count;
		int sentNumber = (int) MathF.Ceiling((float)count / Room.NumberOfRoomsToSendInMessage)-1;
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
					$"{ CommunicationTypeEnum.ClientReceiveRoomListJsonPaginated}",
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
	
	private void ReceivedRequestUserListJson(int index)
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
	
	private void ReceivedRequestUsersListJsonInRoom(int index, Guid roomID)
	{
		Room r = _roomController.GetServerRoomFromGUID(roomID);
		List<User> clients = _roomController.GetUsersInRoom(r);
		int count = clients.Count;
		int sentNumber = (int)MathF.Ceiling((float)count / User.NumberOfUsersToSendInMessage)-1;
		if (count > User.NumberOfUsersToSendInMessage)
		{
			CommunicationTypeEnum command = CommunicationTypeEnum.ClientReceiveUsersListJsonInRoomPaginated;
			while (count > 0)
			{
				var list = new List<User>();
				int c = 0;
				for(; c < User.NumberOfUsersToSendInMessage && count > 0 ; c++)
				{
					list.Add(clients[count-1]);
					count--;
				}
				SendRoomUserListJsonPaginated(r, index, command, sentNumber, count, c, list);
				sentNumber--;
			}
		}
		else
		{
			SendUserListJsonInRoom(index, r, clients, CommunicationTypeEnum.ClientReceiveUsersListJsonInRoom);
		}
	}
	
	private void ReceivedRequestBannedUsersListJsonInRoom(int index, Guid roomID)
	{
		Room r = _roomController.GetServerRoomFromGUID(roomID);
		List<User> clients = _roomController.GetBannedUserListInRoom(r);
		int count = clients.Count;
		int sentNumber = (int)MathF.Ceiling((float)count / User.NumberOfUsersToSendInMessage)-1;
		if (count > User.NumberOfUsersToSendInMessage)
		{
			CommunicationTypeEnum command = CommunicationTypeEnum.ClientReceiveRoomBannedUserListPaginated;
			while (count > 0)
			{
				var list = new List<User>();
				int c = 0;
				for(; c < User.NumberOfUsersToSendInMessage && count > 0 ; c++)
				{
					list.Add(clients[count-1]);
					count--;
				}
				SendRoomUserListJsonPaginated(r, index, command, sentNumber, count, c, list);
				sentNumber--;
			}
		}
		else
		{
			SendUserListJsonInRoom(index, r, clients, CommunicationTypeEnum.ClientReceiveRoomBannedUserList);
		}
	}
	
	private void ReceivedRequestApprovedUsersListJsonInRoom(int index, Guid roomID)
	{
		Room r = _roomController.GetServerRoomFromGUID(roomID);
		List<User> clients = _roomController.GetApprovedUserListInRoom(r);
		int count = clients.Count;
		int sentNumber = (int)MathF.Ceiling((float)count / User.NumberOfUsersToSendInMessage)-1;
		if (count > User.NumberOfUsersToSendInMessage)
		{
			CommunicationTypeEnum command = CommunicationTypeEnum.ClientReceiveRoomApprovedUserListPaginated;
			while (count > 0)
			{
				var list = new List<User>();
				int c = 0;
				for(; c < User.NumberOfUsersToSendInMessage && count > 0 ; c++)
				{
					list.Add(clients[count-1]);
					count--;
				}
				SendRoomUserListJsonPaginated(r, index, command, sentNumber, count, c, list);
				sentNumber--;
			}
		}
		else
		{
			SendUserListJsonInRoom(index, r, clients, CommunicationTypeEnum.ClientReceiveRoomApprovedUserList);
		}
	}

	private void ReceivedRequestSendMessageToAll(int index, string[] messageChunks)
	{
		SendCommsToAll(index,
			$"{messageChunks[1]}");
	}

	private void ReceivedAuthenticate(int index, string[] message)
	{
		User? tmpUser = ValidateUser(message, index);
		if (tmpUser != null)
		{
			tmpUser.WebSocketID = index;

			bool Unique = true;

			for (var i = 0; i < _userController.connectedClients.Count; i++)
			{
				var usr = _userController.connectedClients[i];
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
			$"{ CommunicationTypeEnum.ClientReceiveErrorResponseFromServer}",
			$"{s}",
			$"{errorMessage}"
		};
		SendMessage(index, send);
	}
	
	private void SendYouWhereRemovedFromTheRoom(User user, Room room)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveRemovedFromTheRoom}",
			$"{Room.GetJsonFromRoom(room)}"
		};
		SendMessage(_userController.GetWebSocketIdFromUser(user), send);
	}
	
	private void SendUserLeftRoom(User user, Guid inRoom, string userJson)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveUserLeftRoom}",
			$"{inRoom}",
			$"{userJson}"
		};
		SendMessage(_userController.GetWebSocketIdFromUser(user), send);
	}

	private void SendRoomDestroyed(User user, Room room)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveRoomDestroyed}",
			$"{Room.GetJsonFromRoom(room)}"
		};
		SendMessage(_userController.GetWebSocketIdFromUser(user), send);
	}
	
	private void SendUserDisconnected(User userDisconnected, User userToSentTo)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveUserDisconnected}",
			$"{User.GetJsonFromUser(userDisconnected)}"
		};
		SendMessage(_userController.GetWebSocketIdFromUser(userToSentTo), send);
	}

	private void SendRoomCreated(int index, string createdRoomJason)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveRoomCreated}",
			$"{createdRoomJason}"
		};
		SendMessage(index, send);
	}

	private void SendRoomJoined(int index, string createdRoomJason)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveJoinedRoom}",
			$"{createdRoomJason}"
		};
		SendMessage(index, send);
	}

	private void SendRoomJoined(User user, Room room)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveJoinedRoom}",
			$"{Room.GetJsonFromRoom(room)}"
		};
		SendMessage(_userController.GetWebSocketIdFromUser(user), send);
	}
	
	private void SendYouHaveBeenBannedFromRoom(User user, Room room)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveBannedFromRoom}",
			$"{Room.GetJsonFromRoom(room)}"
		};
		SendMessage(_userController.GetWebSocketIdFromUser(user), send);
	}
	
	private void SendYouAreNoLongerBannedFromRoom(User user, Room room)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveNoLongerBannedFromRoom}",
			$"{Room.GetJsonFromRoom(room)}"
		};
		SendMessage(_userController.GetWebSocketIdFromUser(user), send);
	}
	
	private void SendYouHaveBeenApprovedForRoom(User user, Room room)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveApprovedForRoom}",
			$"{Room.GetJsonFromRoom(room)}"
		};
		SendMessage(_userController.GetWebSocketIdFromUser(user), send);
	}
	
	private void SendYouAreNoLongerApprovedForRoom(User user, Room room)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveNoLongerApprovedForRoom}",
			$"{Room.GetJsonFromRoom(room)}"
		};
		SendMessage(_userController.GetWebSocketIdFromUser(user), send);
	}
	
	private void SendRoomLeft(User userProfile, Room userRemovedFromRoom)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveLeftRoom}",
			$"{Room.GetJsonFromRoom(userRemovedFromRoom)}"
		};
		SendMessage(_userController.GetWebSocketIdFromUser(userProfile), send);
	}

	private void SendUserJoinedRoom(int index, Guid roomGUID, string jsonUser)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveUserJoinedRoom}",
			$"{roomGUID.ToString()}",
			$"{jsonUser}"
		};
		SendMessage(index, send);
	}

	private void SendUserListJson(int index)
	{
		string users = User.GetJsonFromUsersList(_userController.connectedClients);
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveUserListJson}",
			$"{users}"
		};
		SendMessage(index, send);
	}

	private void SendCommunicationsToUser(User com, string message, int index)
	{
		for (var i = 0; i < _userController.connectedClients.Count; i++)
		{
			var u = _userController.connectedClients[i];
			if (u.GetUserName() == com.GetUserName())
			{
				var send = new[]
				{
					$"{ CommunicationTypeEnum.ClientReceiveMessageFromUser}",
					$"{User.GetJsonFromUser(_userController.GetUserProfileFromSocketId(index))}",
					$"{message}"
				};
				SendMessage(_userController.GetWebSocketIdFromUser(u), send);
			}
		}
	}
	
	private void SendAuthenticationPassed(int index)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveAuthenticated}",
			$"OK"
		};
		SendMessage(index, send);
	}

	private void SendAuthenticationFailed(int index)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveAuthenticated}",
			$"FAILED"
		};
		SendMessage(index, send);
	}
	
	private void SendRoomListJson(int index, List<Room> rooms)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveRoomListJson}",
			$"{Room.GetJsonFromRoomList(rooms)}"
		};
		SendMessage(index, send);
	}
	
	private void SendClientGuid(int index, User? user)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveClientGuid}",
			$"{user.GetUserGuid()}"
		};
		SendMessage(index, send);
	}

	private void SendUser(int index, User? user)
	{
		var send = new []
		{
			$"{ CommunicationTypeEnum.ClientReceiveUserInfo}",
			$"{User.GetJsonFromUser(user)}"
		};
		SendMessage(index, send);
	}
	private void SendUserListJsonPaginated(int index, CommunicationTypeEnum command, int sentNumber, int count, int c, List<User> list)
	{
		var send = new []
		{
			$"{ command}",
			$"{sentNumber}",
			$"{count}-{count + c}",
			$"{User.GetJsonFromUsersList(list)}"
		};
		SendMessage(index, send);
	}
	
	private void SendRoomUserListJsonPaginated(Room room, int index, CommunicationTypeEnum command, int sentNumber, int count, int c, List<User> list)
	{
		var send = new []
		{
			$"{ command}",
			$"{sentNumber}",
			$"{count}-{count + c}",
			$"{User.GetJsonFromUsersList(list)}",
			$"{Room.GetJsonFromRoom(room)}"
		};
		SendMessage(index, send);
	}
	
	
	
	private void SendUserReceivedMessage(int index, User user, string message)
	{
		User Reciever = _userController.GetUserProfileFromSocketId(index);
		int i = _userController.GetWebSocketIdFromUser(user);
		var send = new []
		{
			$"{CommunicationTypeEnum.ClientReceiveMessageSentSuccessful}",
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
					$"{CommunicationTypeEnum.ClientReceiveCommunicationToAll}",
					$"{User.GetJsonFromUser(user)}",
					$"{message}"
				};
				SendMessage(index, send);
			}
		}
	}

	private void SendUserListJsonInRoom(int index, Room r, List<User> users, CommunicationTypeEnum com)
	{
		
		var send = new[]
		{
			$"{ CommunicationTypeEnum.ClientReceiveUsersListJsonInRoom}",
			$"{Room.GetJsonFromRoom(r)}",
			$"{User.GetJsonFromUsersList(users)}"
		};
		SendMessage(index, send);
	}

	private void SendMessageToRoom(User toUser, User? fromUser, Room room, string messageToSend)
	{
		int sendIndex = _userController.GetWebSocketIdFromUser(toUser);
		var send = new []
		{
			$"{CommunicationTypeEnum.ClientReceiveRoomMessage}",
			$"{User.GetJsonFromUser(fromUser)}",
			$"{Room.GetJsonFromRoom(room)}",
			$"{messageToSend}"
		};
		SendMessage(sendIndex, send);
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