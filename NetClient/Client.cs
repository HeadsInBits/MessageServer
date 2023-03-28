using MessageServer.Data;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Text;


namespace NetClient
{
	public class Client
	{
		ClientWebSocket webSocket;
		Uri serverUri = new Uri("ws://localhost:8080/");

		private bool isClientValidated = false;
		private int ClientID;
		private String ClientName;
		public List<User> networkUsers = new List<User>();
		public List<Room> roomList = new List<Room>();
		public List<Room> subscribedRooms = new List<Room>();
		

		//Events
		public event Action<(User user, string message)> onMessageRecievedEvent;
		public event Action<bool> onAuthenticateEvent;
		public event Action<List<User>> onUserListRecievedEvent;
		public event Action<string> onUserJoinedEvent;
		public event Action<string> onUserLeftEvent;
		public event Action<List<Room>> onRoomListRecievedEvent;
		public event Action<int> onRoomCreatedEvent;
		public event Action<string> onRoomJoinedEvent;
		public event Action<(int RoomID, string Message)> onRoomMessageRecievedEvent;
		public event Action<int> onIDRecievedEvent;

		public event Action<string> onIncomingWebSocketMessage;

		private bool DisconnectOnFailAuthentication = false;

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
			byte[] receiveBuffer = new byte [16384];
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
				case "AUTH": // authorisation accepted by the server.
					if (messageChunks [1] == "OK") {
						isClientValidated = true;
						onAuthenticateEvent?.Invoke(true);
						break;
					}
					else {
						onAuthenticateEvent?.Invoke(false);
						if (DisconnectOnFailAuthentication) return false;
						throw new AuthenticationException("User is not Validated");
						break;
					}
					break;

				case "IDIS":
					ClientID = System.Int32.Parse(messageChunks [1]);
					onIDRecievedEvent?.Invoke(ClientID);
					break;

				case "USERLIST":
					int numberOfUsers = Int32.Parse(messageChunks [1]);
					networkUsers.Clear();
					for (int counter = 0; counter < numberOfUsers; counter++) {
						networkUsers.Add(new User(messageChunks [3 + counter], true)  );
					}
					onUserListRecievedEvent?.Invoke(networkUsers);
					break;

				case "RECIEVEMESSAGE":
					string jsonStrUser = message.Substring(messageChunks[0].Length + 1, message.Length - (messageChunks [^1].Length + messageChunks [0].Length + 2));
					User user = JsonConvert.DeserializeObject<User>(jsonStrUser);
					ReceiveMessage(user,messageChunks [^1]);
					onMessageRecievedEvent?.Invoke((user, messageChunks [^1]));
					break;

				//case "ROOMLIST":
				//roomList.Clear();
				//int numberOfRooms = Int32.Parse(MessageChunks [1]);
				//for (int counter = 0; counter < numberOfRooms; counter++) {
				//	roomList.Add(Guid.Parse(MessageChunks [2 + counter]));
				//}
				//onRoomListRecievedEvent?.Invoke(roomList);
				//break;

				case "ROOMLIST*JSON":
					string jsonData = message.Substring(messageChunks [0].Length + 1);
					roomList.Clear();
					List<Room> JsonDe = JsonConvert.DeserializeObject<List<Room>>(jsonData);
					roomList = JsonDe;
					onRoomListRecievedEvent?.Invoke(JsonDe);
					break;

				case "ROOMJOINED":
					onRoomJoinedEvent?.Invoke(messageChunks [1]);
					Console.WriteLine($"joined room {messageChunks [1]}");
					break;

				case "ROOMCREATED":
					onRoomCreatedEvent?.Invoke(Int32.Parse(messageChunks [1]));
					Console.WriteLine($"room {messageChunks [1]} has been created");
					break;

				case "ROOMMSG":
					onRoomMessageRecievedEvent?.Invoke((Int32.Parse(messageChunks [1]), messageChunks [2]));
					break;

				case "USERJOINED":
					onUserJoinedEvent?.Invoke(messageChunks [1]);
					Console.WriteLine($"{messageChunks [1]} joined room");
					break;
				
				case "USERLEFT":
					onUserLeftEvent?.Invoke(messageChunks [1]);
					Console.WriteLine($"{messageChunks [1]} left room");
					break;

				default:
					throw new NotSupportedException();


			}

			return true;

		}

		private void ReceiveMessage(User user, string Message)
		{
			//already called this
			//onMessageRecievedEvent?.Invoke((user, Message));
		}

		private async Task SendMessage(string message)
		{
			ArraySegment<byte> buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
			await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

		}

		public async Task CreateRoom(string meta, int roomSize, bool isPublic)
		{
			var msg = new StringBuilder();
			msg.Append($"CREATEROOM:{roomSize}:{(isPublic?"PUBLIC":"PRIVATE")}:{meta}");
			await SendMessage(msg.ToString());
		}

		public async Task SendMessageToRoomAsync(int RoomID, String Message)
		{
			var msg = new StringBuilder();
			msg.Append($"SENDMSGTOROOM:{RoomID}:{ClientID}:{Message}");
			await SendMessage(msg.ToString());
		}

		public async Task UpdateUserList()
		{
			await SendMessage("GETUSERLIST");
		}

		public async Task RequestRoomList()
		{
			var msg = new StringBuilder();
			msg.Append("GETROOMLIST*JSON");
			await SendMessage(msg.ToString());
		}

		public async Task Authenticate(string userName, string passWord)
		{
			string Authmessage = $"AUTHENTICATE:{userName}:{passWord}";
			await SendMessage(Authmessage);
		}
		
		public async void SendMessageToUser(User user, string Message)
		{
			var userJson = JsonConvert.SerializeObject(user, Newtonsoft.Json.Formatting.Indented);
			string msg = $"SENDMESGTOUSER:{userJson}:{Message}";
			await SendMessage(msg);
		}


	}
}