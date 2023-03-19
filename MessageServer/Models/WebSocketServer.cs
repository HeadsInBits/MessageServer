using System.Net;
using System.Net.WebSockets;
using Org.BouncyCastle.Security;


namespace MessageServer;

public class WebSocketServer
{
    private static readonly WebSocketServer instance = new WebSocketServer();
    private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
    private readonly HttpListener listener = new HttpListener();
    private readonly WebSocketHandler handler = new WebSocketHandler();

    private List<User> connectedClients = new List<User>();

    public List<User> ConnectedClients
    {
        get => connectedClients;
        set => connectedClients = value ?? throw new ArgumentNullException(nameof(value));
    }

    private DBManager _dbManager = new DBManager("rpi4", "MessageServer", "App", "app");
    
    private WebSocketServer()
    {
        // Set up HttpListener
        listener.Prefixes.Add("http://localhost:8080/");

    }

    public static WebSocketServer Instance { get { return instance; } }

    public async Task Start()
    {
        // Start HttpListener
        listener.Start();

        Console.WriteLine("WebSocket server started.");

        // Wait for incoming connections
        while (!cancellation.IsCancellationRequested)
        {
            var context = await listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                try
                {
                    // Accept WebSocket connection
                    var socketContext = await context.AcceptWebSocketAsync(subProtocol: null);
                    handler.AddSocket(socketContext.WebSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WebSocket connection error: {ex.Message}");
                }
            }
            else
            {
                // Handle non-WebSocket requests
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    public async Task Stop()
    {
        // Stop HttpListener and WebSocketHandler
        cancellation.Cancel();
        listener.Stop();
        await handler.Stop();
    }
}

