using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Text;

namespace NetClient;

public class Client
{
       ClientWebSocket  webSocket = new ClientWebSocket();
        Uri serverUri = new Uri("ws://localhost:8080/");
        
        private bool isClientValidated = false;
        private int ClientID;
        private String ClientName;
        public ObservableCollection<string> networkUsers = new ObservableCollection<string>();
        public List<Guid> roomList = new List<Guid>();
        public List<Guid> subscribedRooms = new List<Guid>();
        public event Action<string> onMessageRecievedEvent;
        public event Action<bool> onAuthenticateEvent;
        public event Action<bool> onRoomCreatedEvent;
        public event Action<bool> onRoomJoinedEvent;
        public event Action<bool> onUserJoinedEvent;
        public event Action<bool> onUserLeftEvent;
	    public event Action<List<Guid>> onRoomListRecievedEvent;
        public event Action<ObservableCollection<string>> onUserListRecievedEvent;
        public event Action<int> onIDRecievedEvent;
        

        ~Client(){
           this.Disconnect();
        }
        
        public async Task Connect()
        {
            await webSocket.ConnectAsync(serverUri, CancellationToken.None);
            
            byte[] receiveBuffer = new byte[1024];
            ArraySegment<byte> receiveSegment = new ArraySegment<byte>(receiveBuffer);
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(receiveSegment, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string receivedMessage = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);

                    ProcessIncomingMessage(receivedMessage);
                }
            }
        }

        public List<Guid> GetRoomList()
        { return roomList;
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
        private void ProcessIncomingMessage(string receivedMessage)
        {
            Console.WriteLine("INCOMING MESSAGE!: "+ receivedMessage);
            
            string[] MessageChunks = receivedMessage.Split(':');

            switch (MessageChunks[0])
            {
                case "AUTH": // authorisation accepted by the server.
                    if (MessageChunks[1] == "OK")
                    {
                        isClientValidated = true;
                        onAuthenticateEvent?.Invoke(true);
                    }
                    else
                        onAuthenticateEvent?.Invoke(false);
                        throw new AuthenticationException("User is not Validated");
                        
                    break;
                
                case "IDIS":
                    ClientID = System.Int32.Parse(MessageChunks[1]);
                    onIDRecievedEvent?.Invoke(ClientID);
                    break;
                
                case "USERLIST":
                    int numberOfUsers = Int32.Parse(MessageChunks[1]);
                    networkUsers.Clear();
                    for (int counter = 0; counter < numberOfUsers; counter++)
                    {
                        networkUsers.Add(MessageChunks[3 + counter]);
                    }
                    onUserListRecievedEvent?.Invoke(networkUsers);
                    break;

                case "RECIEVEMESSAGE":
                    ReceiveMessage(MessageChunks [1], MessageChunks [2]);
                    onMessageRecievedEvent?.Invoke(MessageChunks [2]);
                    break;
                
                case "ROOMLIST":
                    roomList.Clear();
                    int numberOfRooms = Int32.Parse(MessageChunks[1]);

                    for (int counter = 0; counter < numberOfRooms; counter++)
                    {
                        roomList.Add(Guid.Parse( MessageChunks[2+counter]));
                    }
                    onRoomListRecievedEvent?.Invoke(roomList);
                    break;

                case "ROOMJOINED":
                    onRoomJoinedEvent?.Invoke(true);
                    Console.WriteLine($"joined room {MessageChunks[1]}");
                    break;
                case "ROOMCREATED":
                    onRoomCreatedEvent?.Invoke(true);
                            Console.WriteLine($"room {MessageChunks[1]} has been created");
                    break;
                case "USERJOINED":
                    onUserJoinedEvent?.Invoke(true);
                    Console.WriteLine($"{MessageChunks[1]} joined room");
                    break;
                
                  default:
                      throw new NotSupportedException();
       
            
            }
            
        }

	private void ReceiveMessage(string userName, string Message)
	{
        onMessageRecievedEvent?.Invoke(Message);
	}

	private async Task SendMessage(string message)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
            await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

        }

        public async Task CreateRoom(string userName)
        {
            var msg = new StringBuilder();
            msg.Append("CREATEROOM:2:PRIVATE");
            await SendMessage(msg.ToString());
        }

        public async Task UpdateUserList()
        {
            await SendMessage("GETUSERLIST");
        }
        
        public async Task RefreshRoomList()
        {
            var msg = new StringBuilder();
            msg.Append("GETROOMLIST");
            await SendMessage(msg.ToString());
        }
        
        public async Task Authenticate(string userName, string passWord)
        {
            string Authmessage = $"AUTHENTICATE:{userName}:{passWord}";
            await SendMessage(Authmessage);
        }

        public async void SendMessageToUser(string userName, string Message)
        {
            string msg = $"SENDMESGTOUSER:{userName}:{Message}";
            await SendMessage(msg);
        }

}
    

    

