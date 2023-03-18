using System.Net.WebSockets;
using System.Text;

namespace MessageServer;

public class WebSocketHandler
{
     private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
    private readonly WebSocket[] sockets = new WebSocket[10];

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
                // Close the socket
                sockets[index] = null;
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected",
                    CancellationToken.None);
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
                for (int i = 0; i < sockets.Length; i++)
                {
                    if (sockets[i] != null && i != index)
                    {
                        await sockets[i].SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType,
                            result.EndOfMessage, CancellationToken.None);
                    }

                }
            }

        }
    }

    private void CommsToUser(string username, string message)
    {
        foreach (var u in WebSocketServer.Instance.ConnectedClients)
        {
            if (u.GetUserName() == username)
            {
                SendMessage(u.WebSocketID, message);
            }
        }
    }

    private void GetUserList(int myIndex)
    {
        var returnMessage = new StringBuilder();

        returnMessage.Append("USERLIST:");
        returnMessage.Append(WebSocketServer.Instance.ConnectedClients.Count + ":");
        foreach (var user in WebSocketServer.Instance.ConnectedClients)
        {
            returnMessage.Append(":" + user.GetUserName());
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
        if (message.StartsWith("AUTHENTICATE"))
        {
            User? tmpUser = ValidateUser(message);
            if (tmpUser != null)
            {
                tmpUser.WebSocketID = index;
                WebSocketServer.Instance.ConnectedClients.Add(tmpUser);
                Console.WriteLine("Added User to Client list:" + tmpUser.WebSocketID +"User:" + tmpUser.GetUserName());
                SendMessage(index, "AUTH:OK");
            }
            else // not authenticated
            {
                SendMessage(index, "AUTH:FAILED");   
                
            }
        }

        if (message.StartsWith("GETMYID"))
        {
            SendMessage(index, "IDIS:" + index);
        }
        if (message.StartsWith("GETUSERLIST"))
        {
            GetUserList(index);
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