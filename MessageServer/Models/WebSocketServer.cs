using System.Net;
using System.Net.WebSockets;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace MessageServer.Models
{
	public class WebSocketServer
	{
		private static readonly WebSocketServer instance = new WebSocketServer();
		private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
		private readonly HttpListener listener = new HttpListener();
		private readonly WebSocketHandler handler = new WebSocketHandler();

		private readonly DBManager _dbManager = new DBManager("rpi4", "MessageServer", "App", "app");

        private readonly bool _logginEnabled = true;
		private WebSocketServer()
		{
			// Set up HttpListener to listen on any IP address
			listener.Prefixes.Add("http://*:8080/");
		}

		public static WebSocketServer Instance { get { return instance; } }

		public async Task Start()
		{
			// Start HttpListener
			listener.Start();

			Console.WriteLine("WebSocket server started.");

			// Wait for incoming connections
			while (!cancellation.IsCancellationRequested) {
				HttpListenerContext context = null;

				try {
					// Get incoming HttpListenerContext
					context = await listener.GetContextAsync();
				} catch (HttpListenerException ex) {
					// HttpListenerException will be thrown when the HttpListener is stopped
					Console.WriteLine(ex.Message);
					continue;
				}

				// Handle the incoming request
				if (context.Request.IsWebSocketRequest) {
					// Accept WebSocket connection
					WebSocketContext socketContext = null;

					try {
						socketContext = await context.AcceptWebSocketAsync(subProtocol: null);
                        if (_logginEnabled)
                            Console.WriteLine($"WebSocket Accepted..");
                    } catch (Exception ex) {
						if(_logginEnabled)
						    Console.WriteLine($"WebSocket connection error: {ex.Message}");
						continue;
					}

					// Add WebSocket to handler
					handler.AddSocket(socketContext.WebSocket);
				}
				else {
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
}
