using System.Net.WebSockets;
using System.Text;

namespace MessageServer;

public class WebSocketHandler
{
    private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
    private readonly WebSocket[] sockets = new WebSocket[10];
    private RoomController _roomController = new RoomController();
    private UserController _userController = new UserController();
    private bool logginEnabled = true;

    public void AddSocket(WebSocket socket)
    {
        // Find an available slot in the sockets array
        int index = Array.IndexOf(sockets, null);
        if (index >= 0)
        {
            sockets[index] = socket;
            StartHandling(socket, index);
        }
        else
        {
            // No available slots, close the socket
            socket.Abort();
        }
    }

    public async Task Stop()
    {
        // Stop handling WebSocket messages
        cancellation.Cancel();
        foreach (var socket in sockets)
        {
            if (socket != null)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server shutting down",
                    CancellationToken.None);
            }
        }
    }

    private async void StartHandling(WebSocket socket, int index)
    {
        // Handle WebSocket messages in a separate thread
        var buffer = new byte[1024];
        while (!cancellation.IsCancellationRequested)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                try
                {
                    // Close the socket
                    sockets[index] = null;
                    _userController.connectedClients.Remove(_userController.GetUserProfileFromSocketId(index));
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected",
                        CancellationToken.None);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("SERVER EXCEPTION!!!: "+ ex.Message);
                }

                break;
            }
            else if (result.MessageType == WebSocketMessageType.Binary ||
                     result.MessageType == WebSocketMessageType.Text)
            {
                // Handle the message
                var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received message from client {index}: {message}");

                ProcessMessage(index, message);

                // Send the message to all connected clients except the sender
              
            }

        }
    }

    private void CommsToUser(string username, string message)
    {
        foreach (var u in _userController.connectedClients)
        {
            if (u.GetUserName() == username)
            {
                SendMessage(u.WebSocketID, message);
            }
        }
    }

    private void CommsToAllButSender(int index, string message)
    {
        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i] != null && i != index)
            {
               SendMessage(index, message);
            }

        }
    }

    private void GetUserList(int myIndex)
    {
        var returnMessage = new StringBuilder();

        returnMessage.Append("USERLIST:");
        returnMessage.Append(_userController.connectedClients.Count + ":");
        if (_userController.connectedClients.Count > 0)
        {
            foreach (var user in _userController.connectedClients)
            {
                returnMessage.Append(":" + user.GetUserName());
            }
        }

        Console.WriteLine("SENDING USER LIST@@ " + returnMessage.ToString());
        SendMessage(myIndex, returnMessage.ToString());
    }
    
    private void SendMessage(int index, string  message)
    {
      // sockets[index].SendAsync();
      byte[] buffer = Encoding.UTF8.GetBytes(message);

      // Create a WebSocket message from the buffer
      var webSocketMessage = new ArraySegment<byte>(buffer);
      
      sockets[index].SendAsync(webSocketMessage, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private void ProcessMessage(int index, string message)
    {
        string[] messageChunks = message.Split(':');

        switch (messageChunks[0])
        {
            case "AUTHENTICATE":
                    User? tmpUser = ValidateUser(message);
                    if (tmpUser != null)
                    {
                        tmpUser.WebSocketID = index;
                        _userController.connectedClients.Add(tmpUser);
                        Console.WriteLine("Added User to Client list:" + tmpUser.WebSocketID +"User:" + tmpUser.GetUserName());
                        SendMessage(index, "AUTH:OK");
                    }
                    else // not authenticated
                    {
                        SendMessage(index, "AUTH:FAILED");   
                    
                    }
                    break;
            
            case "GETMYID":
                SendMessage(index, "IDIS:" + index);
                break;
            
            case "GETUSERLIST":
                GetUserList(index);
                break;
            
            case "SENDMESGTOUSER":
                CommsToUser(messageChunks[1], messageChunks[2]);
                break;
            
            case "SENDMESGTOALL":
                CommsToAllButSender(index, messageChunks[1]);
                break;
            
                case "CREATEPRIVATEROOM":
                     int roomNumber =  _roomController.CreateNewRoom(_userController.GetUserProfileFromSocketId(index));
                    
                break; 
            default:
                break;
        }
    }

    private User? ValidateUser(string message)
    {
        DBManager dbManager = new DBManager("rpi4", "MessageServer", "App", "app");
        var messageChunks = message.Split(":");
        if (messageChunks.Length < 3)
            throw new Exception();
        
        if (dbManager.ValidateAccount(messageChunks[1], messageChunks[2]))
        {
            User? tmpUser = new User(messageChunks[1], messageChunks[2], true);
            return tmpUser;
        }

        return null;
    }
}