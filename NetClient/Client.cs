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
		public event Action<List<User>> onUserListRecievedEvent;
		public event Action<string> onUserJoinedEvent;
		public event Action<string> onUserLeftEvent;
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

		public async void Disconnect()
		{
			await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Destroyed", CancellationToken.None);
		}
		private bool ProcessIncomingMessage(string message)
		{
			Console.WriteLine("INCOMING MESSAGE!: " + message);
			onIncomingWebSocketMessage?.Invoke(message);

			string [] messageChunks = message.Split(':');

			switch (messageChunks [0]) {
				case "AUTH": //"AUTH:OK" / "AUTH:FAILED"
					// authorisation accepted by the server.
					if (messageChunks [1] == "OK") {
						isClientValidated = true;
						onAuthenticateEvent?.Invoke(true);
						Task.FromResult(RequestMyClientID());
						break;
					}
					else {
						onAuthenticateEvent?.Invoke(false);
						if (DisconnectOnFailAuthentication) return false;
						throw new AuthenticationException("User is not Validated");
						break;
					}
					break;

				case "YOURGUID": //"IDIS:[USERID_GUID]"
					ClientID = Guid.Parse(messageChunks [1]);
					onIDRecievedEvent?.Invoke(ClientID);
					break;

				case "USERLIST": //"USERLIST:[USERS_JSON]"
					networkUsers = GetUsersFromMessageFormatStringJsonUserList(message, messageChunks);
					onUserListRecievedEvent?.Invoke(networkUsers);
					break;

				case "RECIEVEMESSAGE": //"RECIEVEMESSAGE:[USER_JSON]:[MESSAGE_STRING]"
					var messageString = GetUserMessageFromMessageFormatStringJsonRoomString(message, messageChunks, out var user);
					ReceiveMessage(user,messageString);
					onMessageRecievedEvent?.Invoke((user, messageString));
					break;

				case "ROOMLIST*JSON": //"ROOMLIST*JSON:[ROOMS_JSON]"
					var JsonDe = GetRoomsListFromMessageFormatStringJsonRooms(message, messageChunks);
					roomList = JsonDe;
					onRoomListRecievedEvent?.Invoke(roomList);
					break;
				
				case "ROOMLISTPAGIJSON": //"ROOMLISTPAGIJSON:[PAGE_NUMBER(DEC)]:[INDEX_START]-[INDEX_END]:[ROOMS_JSON]"
					ReceivedPaginatedRoomJsonDataMessage(message, messageChunks);
					break;
				
				case "USERLISTPAGIJSON": //"USERLISTPAGIJSON:[PAGE_NUMBER(DEC)]:[INDEX_START]-[INDEX_END]:[USERS_JSON]"
					ReceivedPaginatedUserJsonDataMessage(message, messageChunks);
					break;

				case "ROOMJOINED": //"ROOMJOINED:[ROOM_JSON] 
					Room roomFromJson = GetRoomFromMessageFormatStringRoom(message, messageChunks);
					onRoomJoinedEvent?.Invoke(roomFromJson);
					Console.WriteLine($"joined room {roomFromJson.GetGuid().ToString()}");
					break;

				case "ROOMCREATED": //"ROOMCREATED:[ROOM_JSON] 
					Room fromJson = GetRoomFromMessageFormatStringRoom(message, messageChunks);
					onRoomCreatedEvent?.Invoke(fromJson);
					Console.WriteLine($"room {fromJson.GetGuid().ToString()} has been created");
					break;

				case "ROOMMSG": //"ROOMMSG:[ROOMID_JSON]:[UserID_GUID]:[MESSAGE_STRING]"
					Guid userID = Guid.Parse(messageChunks[^2]);
					string roomMessageString = messageChunks[^1];
					string jsonStrRoom = message.Substring(messageChunks[0].Length + 1, message.Length - (messageChunks [^2].Length + messageChunks [^1].Length + messageChunks [0].Length + 3));
					Room room = Room.GetRoomFromJson(jsonStrRoom);
					User userFromRoom = room.GetUserByGuid(userID);
					onRoomMessageRecievedEvent?.Invoke((room, userFromRoom, roomMessageString));
					break;

				case "USERJOINED": //TODO: Needs work only used in call "ADDUSERTOROOM" Not implemented on client
					onUserJoinedEvent?.Invoke(messageChunks [1]);
					Console.WriteLine($"{messageChunks [1]} joined room");
					break;
				
				case "USERLEFT": //TODO: Not implemented on server
					onUserLeftEvent?.Invoke(messageChunks [1]);
					Console.WriteLine($"{messageChunks [1]} left room");
					break;
				
				case "USERGUID": //"USERGUID:[User_Json]"
					
					User guidUser = User.GetUserFromJson(message.Substring(messageChunks[0].Length + 1));
					Guid guid = guidUser.GetUserGuid();
					onRecievedUserWithGuid?.Invoke((guidUser, guid));
					break;

				default:
					throw new NotSupportedException();


			}

			return true;

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

		public async Task RequestMyClientID()
		{
			
			await SendMessage("GETMYGUID");
		}

		public async Task RequestUserFromGuid(Guid guid)
		{
			await SendMessage($"USERFROMGUID:{guid.ToString()}");
		}

		private List<Room> GetRoomsListFromMessageFormatStringJsonRooms(string message, string[] messageChunks)
		{
			string jsonData = message.Substring(messageChunks[0].Length + 1);
			roomList.Clear();
			List<Room> JsonDe = Room.GetRoomListFromJson(jsonData);
			return JsonDe;
		}

		private static string GetUserMessageFromMessageFormatStringJsonRoomString(string message,
			string[] messageChunks, out User user)
		{
			var messageString = messageChunks[^1];
			string jsonStrUser = message.Substring(messageChunks[0].Length + 1,
				message.Length - (messageString.Length + messageChunks[0].Length + 2));
			user = User.GetUserFromJson(jsonStrUser);
			return messageString;
		}

		public List<User> GetUsersFromMessageFormatStringJsonUserList(string message, string[] messageChunks)
		{
			string jsonStrUsers = message.Substring(messageChunks[0].Length + 1);
			return User.GetUsersListFromJson(jsonStrUsers);
		}

		private static Room GetRoomFromMessageFormatStringRoom(string message, string[] messageChunks)
		{
			string jsonStrRoomCreated = message.Substring(messageChunks[0].Length + 1);
			Room fromJson = Room.GetRoomFromJson(jsonStrRoomCreated);
			return fromJson;
		}

		private void ReceiveMessage(User user, string Message)
		{
			//already called this
			//onMessageRecievedEvent?.Invoke((user, Message));
		}
		
		public async void RequestToAddUserToRoom(User user, Guid roomID)
		{
			await SendMessage($"ADDUSERTOROOM:{roomID}:{user.GetUserName()}");
		}

		private async Task SendMessage(string message)
		{
			ArraySegment<byte> buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
			await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
			onMessageSentToSocket?.Invoke(message);

		}

		public async Task CreateRoom(string meta, int roomSize, bool isPublic)
		{
			var msg = new StringBuilder();
			msg.Append($"CREATEROOM:{roomSize}:{(isPublic?"PUBLIC":"PRIVATE")}:{meta}");
			await SendMessage(msg.ToString());
		}

		public async Task SendMessageToRoomAsync(Guid RoomID, String Message)
		{
			var msg = new StringBuilder();
			msg.Append($"SENDMSGTOROOM:{RoomID}:{Message}");
			await SendMessage(msg.ToString());
		}

		public bool UpdateUserList()
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
			var msg = new StringBuilder();
			msg.Append("GETROOMLIST*JSON");
			Task.FromResult(SendMessage(msg.ToString()));
			return false;
		}

		public async Task Authenticate(string userName, string passWord)
		{
			string Authmessage = $"AUTHENTICATE:{userName}:{passWord}";
			await SendMessage(Authmessage);
		}
		
		public async void SendMessageToUser(User user, string Message)
		{
			var userJson = User.GetJsonFromUser(user);
			string msg = $"SENDMESGTOUSER:{userJson}:{Message}";
			await SendMessage(msg);
		}


	}
}