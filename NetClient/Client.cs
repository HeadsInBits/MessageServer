using MessageServer.Data;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Text;
using LibObjects;


namespace NetClient
{
	public class Client
	{
		private ClientWebSocket webSocket;
		private Uri serverUri = new Uri("ws://localhost:8080/");
		private bool isClientValidated = false;
		private Guid ClientID;
		private String ClientName;
		private User ClientUser;
		private List<User> networkUsers = new List<User>();
		private List<Room> roomList = new List<Room>();
		private List<Room> tmRoomsList = new List<Room>();
		private List<User> tmUsersList = new List<User>();
		private Dictionary<Guid, List<User>> tmRoomsUsersListDictionary = new Dictionary<Guid, List<User>>();
		private List<Room> subscribedRooms = new List<Room>();
		private bool DisconnectOnFailAuthentication = false;
		private bool handlingUserPationation = false;
		private bool handlingRoomsPationation = false;
		

		//Events
		public event Action<(User user, string message)> onRecievedMessageFromUserEvent;
		public event Action<bool> onReceivedAuthenticateEvent;
		public event Action<Room> onRecievedRoomDestroyedEvent;
		public event Action<List<User>> onRecievedUserListEvent;
		public event Action<(User user, Guid roomGuid)> onRecievedUserJoinedRoomEvent;
		public event Action<(User user, Guid roomGuid)> onRecievedUserLeftRoomEvent;
		public event Action<List<Room>> onRecievedRoomListEvent;
		public event Action<Room> onRecievedRoomCreatedEvent;
		public event Action<Room> onRecievedRoomJoinedEvent;
		public event Action<(Room room, User user, string Message)> onRecievedRoomMessageEvent;
		public event Action<(Room room, List<User> users)> onReceivedUsersListInRoomEvent;
		public event Action<(User user, string messageSent)> onReceivedCommunicationToAllButSenderEvent;
		public event Action<(User user, string messageSent)> onReceivedMessageWasReceivedByUserEvent;
		public event Action<(User user, string messageSent)> onReceivedCommunicationToAllEvent;

		public event Action<(CommunicationTypeEnum comEnum, string message)> onReceivedErrorResponseFromServer;
		public event Action<Guid> onRecievedGuidEvent;
		public event Action<(User user, Guid guid)> onRecievedUserWithGuidEvent;
		
		//FOR DEBUGGING
		public event Action<string> onMessageSentToSocketEvent;
		public event Action<string> onIncomingWebSocketMessageEvent;
		


		~Client()
		{
			 this.Disconnect();
		}

		public void SetDisconnectOnFailAuthentication(bool on)
		{
			DisconnectOnFailAuthentication = on;
		}
		
		public async Task Connect(string url, string port)
		{
			// Create a new WebSocket instance and connect to the server
			webSocket = new ClientWebSocket();
			Uri serverUri = new Uri($"ws://{url}:{port}/");
			await webSocket.ConnectAsync(serverUri, CancellationToken.None);
		}

		public async Task Connect()
		{
			// Create a new WebSocket instance and connect to the server
			webSocket = new ClientWebSocket();
			await webSocket.ConnectAsync(serverUri, CancellationToken.None);
		}
		
		public async void Disconnect()
		{
			await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Destroyed", CancellationToken.None);
		}

		public async Task Listen()
		{
			byte[] receiveBuffer = new byte [32768];
			ArraySegment<byte> receiveSegment = new ArraySegment<byte>(receiveBuffer);
			while (webSocket.State == WebSocketState.Open)
			{
				WebSocketReceiveResult result = await webSocket.ReceiveAsync(receiveSegment, CancellationToken.None);
				if (result.MessageType == WebSocketMessageType.Text)
				{
					string receivedMessage = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);

					if (!ProcessIncomingMessage(receivedMessage))
					{
						return;
					}
				}
			}
		}
		
		private async Task SendMessage(string[] messageArray)
		{
			string message = ProcessMessageData.BuildMessageSafe(messageArray);
        	ArraySegment<byte> buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
        	await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        	onMessageSentToSocketEvent?.Invoke(message);

        }
		
		public List<Room> GetLocalClientRoomList()
		{
			return roomList;
		}
		
		public bool IsClientValidated()
		{
			return isClientValidated;
		}

		public string GetClientName()
		{
			return ClientName;
		}

		
		private bool ProcessIncomingMessage(string message)
		{
			Console.WriteLine("INCOMING MESSAGE!: " + message);
			onIncomingWebSocketMessageEvent?.Invoke(message);

			string [] messageChunks = ProcessMessageData.UnpackMessageSafe(message);

			var s = (CommunicationTypeEnum)int.Parse(messageChunks [0]);
			switch (s) {
				
				//"[ClientReceiveAuthenticated]:OK" / "[ClientReceiveAuthenticated]:FAILED"
				case CommunicationTypeEnum.ClientReceiveAuthenticated: 
					if (!ReceivedAuthenticate(messageChunks))
						return false;
					else
						break;

				//"[ClientReceiveMessageSentSuccessful]:[USER_JSON]:[MESSAGE_STRING]"
				case CommunicationTypeEnum.ClientReceiveMessageSentSuccessful: 
					ReceivedMessageWasReceivedByUser(messageChunks);
					break;
				
				//"[ClientReceiveYourGuid]:[USERID_GUID]"
				case CommunicationTypeEnum.ClientReceiveYourGuid: 
					ReceivedClientGuid(messageChunks);
					break;

				//"[ClientReceiveUserListJson]:[USERS_JSON]"
				case CommunicationTypeEnum.ClientReceiveUserListJson: 
					ReceivedUserList(messageChunks);
					break;

				//"[ClientReceiveMessageFromUser]:[USER_JSON]:[MESSAGE_STRING]"
				case CommunicationTypeEnum.ClientReceiveMessageFromUser: 
					ReceivedMessage(messageChunks);
					break;

				//"[ClientReceiveRoomListJson]:[ROOMS_LIST_JSON]"
				case CommunicationTypeEnum.ClientReceiveRoomListJson: 
					ReceivedRoomListJson(messageChunks);
					break;
				
				//"[ClientReceiveRoomListJsonPaginated]:[PAGE_NUMBER(DEC)]:[INDEX_START]-[INDEX_END]:[ROOMS_JSON]"
				case CommunicationTypeEnum.ClientReceiveRoomListJsonPaginated: 
					ReceivedPaginatedRoomJsonDataMessage(messageChunks);
					break;
				
				//"[ClientReceiveUserListJsonPaginated]:[PAGE_NUMBER(DEC)]:[INDEX_START]-[INDEX_END]:[USERS_JSON]"
				case CommunicationTypeEnum.ClientReceiveUserListJsonPaginated: 
					ReceivedPaginatedUserJsonDataMessage(messageChunks);
					break;

				//"[ClientReceiveJoinedRoom]:[ROOM_JSON] 
				case CommunicationTypeEnum.ClientReceiveJoinedRoom: 
					ReceivedRoomJoined(messageChunks);
					break;
				
				//"[ClientReceiveRoomDestroyed]:[ROOM_JSON] 
				case CommunicationTypeEnum.ClientReceiveRoomDestroyed: 
					ReceivedRoomDestroyed(messageChunks);
					break;

				//"[ClientReceiveRoomCreated]:[ROOM_JSON] 
				case CommunicationTypeEnum.ClientReceiveRoomCreated: 
					ReceivedRoomCreated(messageChunks);
					break;
				
				//"[ClientReceiveRoomMessage]:[UserID_GUID]:[ROOM_JSON]:[MESSAGE_STRING]"
				case CommunicationTypeEnum.ClientReceiveRoomMessage: 
					ReceivedRoomMessage(messageChunks);
					break;

				//"[ClientReceiveUserJoinedRoom]:[ROOM_GUID]:[USER_JSON]"
				case CommunicationTypeEnum.ClientReceiveUserJoinedRoom: 
					ReceivedUserJoinedRoom(messageChunks);
					break;
				
				//"[ClientReceiveUserLeftRoom]:[ROOM_GUID]:[USER_JSON]"
				case CommunicationTypeEnum.ClientReceiveUserLeftRoom: 
					ReceivedUserLeftRoom(messageChunks);
					break;
				
				//"[ClientReceiveUserInfo]:[USER_JSON]"
				case CommunicationTypeEnum.ClientReceiveUserInfo: 
					ReceivedUserInfo(messageChunks);
					break;
				
				//"[ClientReceiveUsersListJsonInRoom]:[ROOM_GUID]:[USER_LIST_JSON]"
				case CommunicationTypeEnum.ClientReceiveUsersListJsonInRoom: 
					ReceivedUsersInRoom(messageChunks);
					break;
				
				//"[ClientReceiveUsersListJsonInRoomPaginated]:[PAGE_NUMBER(DEC)]:[INDEX_START]-[INDEX_END]:[USERS_JSON]:[ROOM_JSON]"
				case CommunicationTypeEnum.ClientReceiveUsersListJsonInRoomPaginated: 
					ReceivedUsersInRoomPaginated(messageChunks);
					break;
				
				//"[ClientReceiveCommunicationToAllButSender]:[USER_JSON]:[MESSAGE]"
				case CommunicationTypeEnum.ClientReceiveCommunicationToAllButSender: 
					ReceivedCommunicationToAllButSender(messageChunks);
					break;
				
				//"[ClientReceiveCommunicationToAllButSender]:[USER_JSON]:[MESSAGE]"
				case CommunicationTypeEnum.ClientReceiveCommunicationToAll: 
					ReceivedCommunicationToAll(messageChunks);
					break;

				//"[ClientReceiveErrorResponseFromServer]:[CommunicationTypeEnum]:[MESSAGE]"
				case CommunicationTypeEnum.ClientReceiveErrorResponseFromServer: 
					ReceivedErrorResponseFromServer(messageChunks);
					break;

				default:
					throw new NotSupportedException();

			}

			return true;

		}

		private void ReceivedUsersInRoomPaginated(string[] messageChunks)
		{
			handlingUserPationation = true;
			
			var page = ProcessMessageData.GetPageRoomAndUsersFromFormatStringIntStringUserListJsonRoomJson(messageChunks, 
				out var tmUsers, out Room room);
			if (tmRoomsUsersListDictionary.ContainsKey(room.GetGuid()))
			{
				tmRoomsUsersListDictionary[room.GetGuid()].AddRange(tmUsers);
			}
			else
			{
				tmRoomsUsersListDictionary.Add(room.GetGuid(),tmUsers);
			}
			if (page == 0)
			{
				onReceivedUsersListInRoomEvent?.Invoke((room, tmRoomsUsersListDictionary[room.GetGuid()]));
				tmRoomsUsersListDictionary.Remove(room.GetGuid());
				handlingUserPationation = false;
			}
		}

		private void ReceivedErrorResponseFromServer(string[] messageChunks)
		{
			onReceivedErrorResponseFromServer?.Invoke(((CommunicationTypeEnum)int.Parse(messageChunks [1]),messageChunks[2]));
		}

		private void ReceivedCommunicationToAll(string[] messageChunks)
		{
			string messageSent = ProcessMessageData.GetUserMessageFromMessageFormatStringJsonUserString(messageChunks,
				out User user);
			onReceivedCommunicationToAllEvent?.Invoke((user, messageSent));
		}

		private void ReceivedCommunicationToAllButSender(string[] messageChunks)
		{
			string messageSent = ProcessMessageData.GetUserMessageFromMessageFormatStringJsonUserString(messageChunks,
				out User user);
			onReceivedCommunicationToAllButSenderEvent?.Invoke((user, messageSent));
		}

		private void ReceivedUsersInRoom(string[] messageChunks)
		{
			List<User> users =
				ProcessMessageData.GetUserListAndRoomFromFormatStringRoomJsonUserListJson(messageChunks, out Room room);
			onReceivedUsersListInRoomEvent?.Invoke((room, users));
		}

		private void ReceivedMessageWasReceivedByUser(string[] messageChunks)
		{
			string messageSent = ProcessMessageData.GetUserMessageFromMessageFormatStringJsonUserString( messageChunks,
				out User user);
			onReceivedMessageWasReceivedByUserEvent?.Invoke((user, messageSent));
		}

		private void ReceivedUserInfo(string[] messageChunks)
		{
			User guidUser = ProcessMessageData.GetUserFromMessageFormatStringJsonUser(messageChunks);
			Guid guid = guidUser.GetUserGuid();
			if (guid == ClientID)
			{
				ClientUser = guidUser;
			}
			onRecievedUserWithGuidEvent?.Invoke((guidUser, guid));
		}

		private void ReceivedUserLeftRoom(string[] messageChunks)
		{
			User leftUser =
				ProcessMessageData.GetUserAndGuidFromFormatStringGuidUserJson(messageChunks, out var guidLeft);
			onRecievedUserLeftRoomEvent?.Invoke((leftUser, guidLeft));
			Console.WriteLine($"{leftUser.GetUserName()} left room");
		}

		private void ReceivedUserJoinedRoom(string[] messageChunks)
		{
			var joinedUser =
				ProcessMessageData.GetUserAndGuidFromFormatStringGuidUserJson(messageChunks, out var guidJoined);
			onRecievedUserJoinedRoomEvent?.Invoke((joinedUser, guidJoined));
			Console.WriteLine($"{joinedUser.GetUserName()} joined room");
		}

		private void ReceivedRoomMessage(string[] messageChunks)
		{
			var roomMessageString = ProcessMessageData.GetRoomUserAndMessageFromFormatStringUserJsonRoomJsonMessage(messageChunks, out var room,
					out var userFromRoom);
			onRecievedRoomMessageEvent?.Invoke((room, userFromRoom, roomMessageString));
		}

		private void ReceivedRoomCreated(string[] messageChunks)
		{
			Room fromJson = ProcessMessageData.GetRoomFromMessageFormatStringRoomJson( messageChunks);
			onRecievedRoomCreatedEvent?.Invoke(fromJson);
			Console.WriteLine($"created room: {fromJson.GetGuid().ToString()} has been created");
		}

		private void ReceivedRoomDestroyed(string[] messageChunks)
		{
			Room destroyedRoomFromJson = ProcessMessageData.GetRoomFromMessageFormatStringRoomJson(messageChunks);
			onRecievedRoomDestroyedEvent?.Invoke(destroyedRoomFromJson);
			Console.WriteLine($"room has been destroyed: {destroyedRoomFromJson.GetGuid().ToString()}");
		}

		private void ReceivedRoomJoined(string[] messageChunks)
		{
			Room roomFromJson = ProcessMessageData.GetRoomFromMessageFormatStringRoomJson(messageChunks);
			onRecievedRoomJoinedEvent?.Invoke(roomFromJson);
			Console.WriteLine($"joined room: {roomFromJson.GetGuid().ToString()}");
		}

		private void ReceivedRoomListJson(string[] messageChunks)
		{
			roomList.Clear();
			var JsonDe = ProcessMessageData.GetRoomsListFromMessageFormatStringJsonRooms(messageChunks);
			roomList = JsonDe;
			onRecievedRoomListEvent?.Invoke(roomList);
		}

		private void ReceivedMessage(string[] messageChunks)
		{
			var messageString =
				ProcessMessageData.GetUserMessageFromMessageFormatStringJsonUserString(messageChunks,
					out var user);
			SendReceivedMessage(user, messageString);
			onRecievedMessageFromUserEvent?.Invoke((user, messageString));
		}

		private void ReceivedUserList(string[] messageChunks)
		{
			networkUsers = ProcessMessageData.GetUsersFromMessageFormatStringJsonUserList( messageChunks);
			onRecievedUserListEvent?.Invoke(networkUsers);
		}

		private void ReceivedClientGuid(string[] messageChunks)
		{
			ClientID = Guid.Parse(messageChunks[1]);
			onRecievedGuidEvent?.Invoke(ClientID);
			Task.FromResult(RequestUserFromGuid(ClientID));
		}

		private bool ReceivedAuthenticate(string[] messageChunks)
		{
			// authorisation accepted by the server.
			if (messageChunks[1] == "OK")
			{
				isClientValidated = true;
				onReceivedAuthenticateEvent?.Invoke(true);
				Task.FromResult(RequestMyClientId());
				return true;
			}
			else
			{
				onReceivedAuthenticateEvent?.Invoke(false);
				if (DisconnectOnFailAuthentication) return false;
				throw new AuthenticationException("User is not Validated");
			}
			
		}

		private void ReceivedPaginatedUserJsonDataMessage(string[] messageChunks)
		{
			handlingUserPationation = true;
			
			var page = ProcessMessageData.GetPageAndUsersFromFormatStringIntStringUserListJson(messageChunks, out var tmUsers);
			tmUsersList.AddRange(tmUsers);
			if (page == 0)
			{
				networkUsers = tmUsersList;
				tmUsersList = new List<User>();
				onRecievedUserListEvent?.Invoke(networkUsers);
				handlingUserPationation = false;
			}
		}

		private void ReceivedPaginatedRoomJsonDataMessage(string[] messageChunks)
		{
			handlingRoomsPationation = true;

			var page = ProcessMessageData.GetPageAndUsersFromFormatStringIntStringRoomListJson(messageChunks,
					out List<Room> tmRooms);
			tmRoomsList.AddRange(tmRooms);
			if (page == 0)
			{
				roomList = tmRoomsList;
				tmRoomsList = new List<Room>();
				onRecievedRoomListEvent?.Invoke(roomList);
				handlingRoomsPationation = false;
			}
		}
		
		private async Task SendReceivedMessage(User user, string message)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveMessageReceivedSuccessfully}",
				$"{User.GetJsonFromUser(user)}",
				$"{message}"
			};
			await SendMessage(send);
		}

		public async Task RequestMyClientId()
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestClientGuid}"
			};
			await SendMessage(send);
		}

		public async Task RequestUserFromGuid(Guid guid)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestUserFromGuid}",
				$"{guid.ToString()}"
			};
			await SendMessage(send);
		}

		public async void RequestToAddUserToRoom(User user, Guid roomID)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestAddUserToRoom}",
				$"{roomID.ToString()}",
				$"{user.GetUserName()}"
			};
			await SendMessage(send);
		}

		public async Task RequestCreateRoom(string meta, int roomSize, bool isPublic, string nameOfRoom)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestCreateRoom}",
				$"{roomSize.ToString()}",
				$"{(isPublic?"PUBLIC":"PRIVATE")}",
				$"{meta}",
				$"{nameOfRoom}"
			};
			await SendMessage(send);
		}

		public bool RequestUserList()
		{
			if (handlingUserPationation)
			{
				return false;
			}
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestUserListJson}"
			};
			Task.FromResult(SendMessage(send));
			return true;
		}

		public bool RequestRoomList()
		{
			if (handlingRoomsPationation)
			{
				return false;
			}
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestRoomListJson}"
			};
			Task.FromResult(SendMessage(send));
			return false;
		}

		public async Task RequestAuthenticate(string userName, string passWord)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveAuthenticate}",
				$"{userName}",
				$"{passWord}"
			};
			await SendMessage(send);
		}
		
		public async Task RequestLockRoom(Room room)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestLockRoom}",
				$"{Room.GetJsonFromRoom(room)}"
			};
			await SendMessage(send);
		}
		
		public async Task RequestUnlockRoom(Room room)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestUnlockRoom}",
				$"{Room.GetJsonFromRoom(room)}"
			};
			await SendMessage(send);
		}
		
		public async Task RequestRemoveUserFromRoom(Room room, User user)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestRemoveUserFromRoom}",
				$"{User.GetJsonFromUser(user)}",
				$"{Room.GetJsonFromRoom(room)}"
			};
			await SendMessage(send);
		}
		
		public async Task RequestBanUserFromRoom(Room room, User user)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestBanUserFromRoom}",
				$"{User.GetJsonFromUser(user)}",
				$"{Room.GetJsonFromRoom(room)}"
			};
			await SendMessage(send);
		}
		
		public async Task RequestRemoveBanFromUserInRoom(Room room, User user)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestRemoveBanFromUserInRoom}",
				$"{User.GetJsonFromUser(user)}",
				$"{Room.GetJsonFromRoom(room)}"
			};
			await SendMessage(send);
		}
		
		public async Task RequestApproveUserFromRoom(Room room, User user)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestApproveUserFromRoom}",
				$"{User.GetJsonFromUser(user)}",
				$"{Room.GetJsonFromRoom(room)}"
			};
			await SendMessage(send);
		}
		
		public async Task RequestRemoveApproveFromUserInRoom(Room room, User user)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestRemoveApproveFromUserInRoom}",
				$"{User.GetJsonFromUser(user)}",
				$"{Room.GetJsonFromRoom(room)}"
			};
			await SendMessage(send);
		}
		
		public async void RequestSendMessageToUser(User user, string message)
		{
			var userJson = User.GetJsonFromUser(user);
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestSendMessageToUser}",
				$"{userJson}",
				$"{message}"
			};
			await SendMessage(send);
		}
		public async Task RequestSendMessageToRoomAsync(Guid roomGuid, String message)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveSendMessageToRoom}",
				$"{roomGuid.ToString()}",
				$"{message}"
			};
			await SendMessage(send);
		}
		
		public async Task RequestSendMessageToAllAsync(string message)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestSendMessageToAll}",
				$"{message}"
			};
			await SendMessage(send);
		}
		
		public async Task RequestGetUsersInRoomAsync(Guid roomID)
		{
			var send = new []
			{
				$"{(int)CommunicationTypeEnum.ServerReceiveRequestUsersListJsonInRoom}",
				$"{roomID.ToString()}"
			};
			await SendMessage(send);
		}
		
	}
}