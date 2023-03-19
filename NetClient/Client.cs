using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Security.Authentication;

namespace NetClient;

public class Client
{
        ClientWebSocket  webSocket = new ClientWebSocket();
        Uri serverUri = new Uri("ws://localhost:8080/");
        
        private bool isClientValidated = false;
        private int ClientID;
        public ObservableCollection<string> networkUsers = new ObservableCollection<string>();
     

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

        public bool IsClientValidated()
        {
            return isClientValidated;
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
                        isClientValidated = true;
                    else
                        throw new AuthenticationException("User is not Validated");
                    break;
                
                case "IDIS":
                    ClientID = System.Int32.Parse(MessageChunks[1]);
                    break;
                
                case "USERLIST":
                    int numberOfUsers = Int32.Parse(MessageChunks[1]);
                    networkUsers.Clear();
                    for (int counter = 0; counter < numberOfUsers; counter++)
                    {
                        networkUsers.Add(MessageChunks[3 + counter]);
                    }
                    break;
                case "RECIEVEMESSAGE":
                   //Todo: Add an event to handle incoming datastreams.
                    
                    //  MessageBox.Show("Message Recieved!" + MessageChunks[1]);
                    
                    break;
                
                  default:
                      throw new NotSupportedException();
            }
            
        }
        
        private async Task SendMessage(string message)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
            await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

        }

        public async Task UpdateUserList()
        {
            await SendMessage("GETUSERLIST");
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
    

    

