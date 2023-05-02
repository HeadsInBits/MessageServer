using System;
using System.Data;
using System.Net;
using System.Net.WebSockets;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace MessageServer.Models
{
    public class WebSocketServer : IDisposable
    {
        private static readonly WebSocketServer instance = new WebSocketServer();
        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
        private readonly HttpListener listener = new HttpListener();
        private readonly WebSocketHandler handler = new WebSocketHandler();

        private readonly DBManager _dbManager = new DBManager("rpi4", "MessageServer", "App", "app");

        private readonly bool _logginEnabled = true;
        private readonly string serverVersion = "1.0.0";

        private WebSocketServer()
        {
            // Set up HttpListener to listen on any IP address
            listener.Prefixes.Add("http://*:8080/");
        }

        public static WebSocketServer Instance { get { return instance; } }

        public async Task Start()
        {

            try
            {
                // Start HttpListener
                listener.Start();
            }
            catch (HttpListenerException e)
            {
                Console.WriteLine("Unable to Start server: " + e.Message);
                return;
            }

            Console.WriteLine("WebSocket server started..." + "V:" + serverVersion);
            Console.Title = "MessageServer - SocketServer V:" + serverVersion;
            

            // Wait for incoming connections
            while (!cancellation.IsCancellationRequested)
            {
                HttpListenerContext context = null;

                try
                {
                    // Get incoming HttpListenerContext
                    context = await listener.GetContextAsync().ConfigureAwait(false);
                }
                catch (HttpListenerException ex)
                {
                    // HttpListenerException will be thrown when the HttpListener is stopped
                    Console.WriteLine(ex.Message);
                    continue;
                }

                // Handle the incoming request
                if (context.Request.IsWebSocketRequest)
                {
                    // Accept WebSocket connection
                    WebSocketContext socketContext = null;

                    var webSocketCreationOptions = new WebSocketCreationOptions
                    {
                        SubProtocol = null,
                        KeepAliveInterval = TimeSpan.FromSeconds(30),
                        IsServer = true,
                        // Enable compression using WebSocketDeflateOptions
                        DangerousDeflateOptions = new WebSocketDeflateOptions
                        {
                            ClientContextTakeover = true,
                            ServerContextTakeover = true
                        }
                    };

                    try
                    {
                        socketContext = await context.AcceptWebSocketAsync(subProtocol: null).ConfigureAwait(false);
                        if (_logginEnabled)
                            Console.WriteLine($"WebSocket Accepted..");
                    }
                    catch (Exception ex)
                    {
                        if (_logginEnabled)
                            Console.WriteLine($"WebSocket connection error: {ex.Message}");
                        continue;
                    }

                    // Add WebSocket to handler
                    handler.AddSocket(socketContext.WebSocket);
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
            await handler.Stop().ConfigureAwait(false);
        }

        public void Dispose()
        {
            listener.Close();
            cancellation.Dispose();
        }
    }
}