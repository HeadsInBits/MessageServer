using MessageServer.Data;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Text;
using LibObjects;


namespace NetClient
{
	public class Client
	{
		ClientWebSocket webSocket;
		Uri serverUri = new Uri("ws://localhost:8080/");

		private bool isClientValidated = false;
		private Guid ClientID;
		private String ClientName;
		public List<User> networkUsers = new List<User>();
		public List<Room> roomList = new List<Room>();
		private List<Room> tmRoomsList = new List<Room>();
		private List<User> tmUsersList = new List<User>();
		public List<Room> subscribedRooms = new List<Room>();
		

		//Events
		public event Action<(User user, string message)> onMessageRecievedEvent;
		public event Action<bool> onAuthenticateEvent;
		public event Action<Room> onRoomDestroyedEvent;
		public event Action<List<User>> onUserListRecievedEvent;
		public event Action<(User user, Guid roomGuid)> onUserJoinedRoomEvent;
		public event Action<(User user, Guid roomGuid)> onUserLeftRoomEvent;
		public event Action<List<Room>> onRoomListRecievedEvent;
		public event Action<Room> onRoomCreatedEvent;
		public event Action<Room> onRoomJoinedEvent;
		public event Action<(Room room, User user, string Message)> onRoomMessageRecievedEvent;
		public event Action<Guid> onIDRecievedEvent;
		public event Action<(User user, Guid guid)> onRecievedUserWithGuid;
		public event Action<string> onMessageSentToSocket;
		public event Action<string> onIncomingWebSocketMessage;
		private bool DisconnectOnFailAuthentication = false;
		private bool handlingUserPationation = false;
		private bool handlingRoomsPationation = false;


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
		
		private async Task SendMessage(string message)
        {
        	ArraySegment<byte> buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
        	await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        	onMessageSentToSocket?.Invoke(message);

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
			onIncomingWebSocketMessage?.Invoke(message);

			string [] messageChunks = message.Split(':');

			var s = (CommunicationTypeEnum)int.Parse(messageChunks [0]);
			switch (s) {
				case CommunicationTypeEnum.ClientReceiveAuthenticated: //"AUTH:OK" / "AUTH:FAILED"
					if (!ReceivedAuthenticate(messageChunks))
						return false;
					else
						break;

				case CommunicationTypeEnum.ClientReceiveYourGuid: //"IDIS:[USERID_GUID]"
					ReceivedGuid(messageChunks);
					break;

				case CommunicationTypeEnum.ClientReceiveUserListJson: //"USERLIST:[USERS_JSON]"
					ReceivedUserList(message, messageChunks);
					break;

				case CommunicationTypeEnum.ClientReceiveMessageFromUser: //"RECIEVEMESSAGE:[USER_JSON]:[MESSAGE_STRING]"
					ReceivedMessage(message, messageChunks);
					break;

				case CommunicationTypeEnum.ClientReceiveRoomListJson: //"ROOMLIST*JSON:[ROOMS_LIST_JSON]"
					ReceivedRoomListJson(message, messageChunks);
					break;
				
				case CommunicationTypeEnum.ClientReceiveRoomListJsonPaginated: //"ROOMLISTPAGIJSON:[PAGE_NUMBER(DEC)]:[INDEX_START]-[INDEX_END]:[ROOMS_JSON]"
					ReceivedPaginatedRoomJsonDataMessage(message, messageChunks);
					break;
				
				case CommunicationTypeEnum.ClientReceiveUserListJsonPaginated: //"USERLISTPAGIJSON:[PAGE_NUMBER(DEC)]:[INDEX_START]-[INDEX_END]:[USERS_JSON]"
					ReceivedPaginatedUserJsonDataMessage(message, messageChunks);
					break;

				case CommunicationTypeEnum.ClientReceiveJoinedRoom: //"ROOMJOINED:[ROOM_JSON] 
					ReceivedRoomJoined(message, messageChunks);
					break;
				
				case CommunicationTypeEnum.ClientReceiveRoomDestroyed: //"ROOMDESTROYED:[ROOM_JSON] 
					ReceivedRoomDestroyed(message, messageChunks);
					break;

				case CommunicationTypeEnum.ClientReceiveRoomCreated: //"ROOMCREATED:[ROOM_JSON] 
					ReceivedRoomCreated(message, messageChunks);
					break;
				
				case CommunicationTypeEnum.ClientReceiveRoomMessage: //"ROOMMSG:[UserID_GUID]:[ROOMID_JSON]:[MESSAGE_STRING]"
					ReceivedRoomMessage(message, messageChunks);
					break;

				case CommunicationTypeEnum.ClientReceiveUserJoinedRoom: //"USERJOINEDROOM:[ROOM_GUID]:[USER_JSON]"
					ReceivedUserJoinedRoom(message, messageChunks);
					break;
				
				case CommunicationTypeEnum.ClientReceiveUserLeftRoom: //"USERLEFTROOM:[ROOM_GUID]:[USER_JSON]"
					ReceivedUserLeftRoom(message, messageChunks);
					break;
				
				case CommunicationTypeEnum.ClientReceiveUserInfo: //"USERGUID:[User_Json]"
					ReceivedUserGuid(message, messageChunks);
					break;

				default:
					throw new NotSupportedException();


			}

			return true;

		}

		private void ReceivedUserGuid(string message, string[] messageChunks)
		{
			User guidUser = ProcessMessageData.GetUserFromMessageFormatStringJsonUser(message, messageChunks);
			Guid guid = guidUser.GetUserGuid();
			onRecievedUserWithGuid?.Invoke((guidUser, guid));
		}

		private void ReceivedUserLeftRoom(string message, string[] messageChunks)
		{
			User leftUser =
				ProcessMessageData.GetUserAndGuidFromFormatStringGuidUserJson(message, messageChunks, out var guidLeft);
			onUserLeftRoomEvent?.Invoke((leftUser, guidLeft));
			Console.WriteLine($"{leftUser.GetUserName()} left room");
		}

		private void ReceivedUserJoinedRoom(string message, string[] messageChunks)
		{
			var joinedUser =
				ProcessMessageData.GetUserAndGuidFromFormatStringGuidUserJson(message, messageChunks, out var guidJoined);
			onUserJoinedRoomEvent?.Invoke((joinedUser, guidJoined));
			Console.WriteLine($"{joinedUser.GetUserName()} joined room");
		}

		private void ReceivedRoomMessage(string message, string[] messageChunks)
		{
			var roomMessageString = ProcessMessageData.GetRoomUserAndMessageFromFormatStringGuidRoomJsonMessage(message, messageChunks, out var room,
					out var userFromRoom);
			onRoomMessageRecievedEvent?.Invoke((room, userFromRoom, roomMessageString));
		}

		private void ReceivedRoomCreated(string message, string[] messageChunks)
		{
			Room fromJson = ProcessMessageData.GetRoomFromMessageFormatStringRoom(message, messageChunks);
			onRoomCreatedEvent?.Invoke(fromJson);
			Console.WriteLine($"created room: {fromJson.GetGuid().ToString()} has been created");
		}

		private void ReceivedRoomDestroyed(string message, string[] messageChunks)
		{
			Room destroyedRoomFromJson = ProcessMessageData.GetRoomFromMessageFormatStringRoom(message, messageChunks);
			onRoomDestroyedEvent?.Invoke(destroyedRoomFromJson);
			Console.WriteLine($"room has been destroyed: {destroyedRoomFromJson.GetGuid().ToString()}");
		}

		private void ReceivedRoomJoined(string message, string[] messageChunks)
		{
			Room roomFromJson = ProcessMessageData.GetRoomFromMessageFormatStringRoom(message, messageChunks);
			onRoomJoinedEvent?.Invoke(roomFromJson);
			Console.WriteLine($"joined room: {roomFromJson.GetGuid().ToString()}");
		}

		private void ReceivedRoomListJson(string message, string[] messageChunks)
		{
			roomList.Clear();
			var JsonDe = ProcessMessageData.GetRoomsListFromMessageFormatStringJsonRooms(message, messageChunks);
			roomList = JsonDe;
			onRoomListRecievedEvent?.Invoke(roomList);
		}

		private void ReceivedMessage(string message, string[] messageChunks)
		{
			var messageString =
				ProcessMessageData.GetUserMessageFromMessageFormatStringJsonRoomString(message, messageChunks,
					out var user);
			SendReceivedMessage(user, messageString);
			onMessageRecievedEvent?.Invoke((user, messageString));
		}

		private void ReceivedUserList(string message, string[] messageChunks)
		{
			networkUsers = ProcessMessageData.GetUsersFromMessageFormatStringJsonUserList(message, messageChunks);
			onUserListRecievedEvent?.Invoke(networkUsers);
		}

		private void ReceivedGuid(string[] messageChunks)
		{
			ClientID = Guid.Parse(messageChunks[1]);
			onIDRecievedEvent?.Invoke(ClientID);
		}

		private bool ReceivedAuthenticate(string[] messageChunks)
		{
			// authorisation accepted by the server.
			if (messageChunks[1] == "OK")
			{
				isClientValidated = true;
				onAuthenticateEvent?.Invoke(true);
				Task.FromResult(RequestMyClientId());
				return true;
			}
			else
			{
				onAuthenticateEvent?.Invoke(false);
				if (DisconnectOnFailAuthentication) return false;
				throw new AuthenticationException("User is not Validated");
			}
			
		}

		private void ReceivedPaginatedUserJsonDataMessage(string message, string[] messageChunks)
		{
			handlingUserPationation = true;
			
			int userPage = Int32.Parse(messageChunks[1]);
			string urJson = message.Substring(messageChunks[0].Length + messageChunks[1].Length +
			                                  messageChunks[2].Length + 3);
			List<User> tmUsers = User.GetUsersListFromJson(urJson);
			tmUsersList.AddRange(tmUsers);
			if (userPage == 0)
			{
				networkUsers = tmUsersList;
				tmUsersList = new List<User>();
				onUserListRecievedEvent?.Invoke(networkUsers);
				handlingUserPationation = false;
			}
		}

		private void ReceivedPaginatedRoomJsonDataMessage(string message, string[] messageChunks)
		{
			handlingRoomsPationation = true;
			int page = Int32.Parse(messageChunks[1]);
			string rmJson = message.Substring(messageChunks[0].Length + messageChunks[1].Length +
			                                  messageChunks[2].Length + 3);
			List<Room> tmRooms = Room.GetRoomListFromJson(rmJson);
			tmRoomsList.AddRange(tmRooms);
			if (page == 0)
			{
				roomList = tmRoomsList;
				tmRoomsList = new List<Room>();
				onRoomListRecievedEvent?.Invoke(roomList);
				handlingRoomsPationation = false;
			}
		}

		public async Task RequestMyClientId()
		{
			await SendMessage("GETMYGUID");
		}

		public async Task RequestUserFromGuid(Guid guid)
		{
			await SendMessage($"USERFROMGUID:{guid.ToString()}");
		}

		private async Task SendReceivedMessage(User user, string Message)
		{
			await SendMessage($"RECIEVEDMESSAGE:{User.GetJsonFromUser(user)}:{Message}");
		}
		
		public async void RequestToAddUserToRoom(User user, Guid roomID)
		{
			await SendMessage($"ADDUSERTOROOM:{roomID.ToString()}:{user.GetUserName()}");
		}

		public async Task RequestCreateRoom(string meta, int roomSize, bool isPublic)
		{
			await SendMessage($"CREATEROOM:{roomSize.ToString()}:{(isPublic?"PUBLIC":"PRIVATE")}:{meta}");
		}

		public bool RequestUserList()
		{
			if (handlingUserPationation)
			{
				return false;
			}
			Task.FromResult(SendMessage("GETUSERLIST"));
			return true;
		}

		public bool RequestRoomList()
		{
			if (handlingRoomsPationation)
			{
				return false;
			}
			Task.FromResult(SendMessage("GETROOMLIST*JSON"));
			return false;
		}

		public async Task RequestAuthenticate(string userName, string passWord)
		{
			await SendMessage($"AUTHENTICATE:{userName}:{passWord}");
		}
		
		public async void SendMessageToUser(User user, string Message)
		{
			var userJson = User.GetJsonFromUser(user);
			await SendMessage($"SENDMESGTOUSER:{userJson}:{Message}");
		}
		public async Task SendMessageToRoomAsync(Guid RoomID, String Message)
		{
			await SendMessage($"SENDMSGTOROOM:{RoomID.ToString()}:{Message}");
		}
	}
}