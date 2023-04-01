using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MessageServer.Models
{
	public class WebSocketServer
	{
		private static readonly WebSocketServer instance = new WebSocketServer();
		private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
		private readonly TcpListener listener;
		private readonly WebSocketHandler handler = new WebSocketHandler();
		private readonly DBManager _dbManager = new DBManager("rpi4", "MessageServer", "App", "app");

		private WebSocketServer()
		{
			// Set up TcpListener to listen on any IP address
			listener = new TcpListener(IPAddress.Any, 8080);
		}

		public static WebSocketServer Instance { get { return instance; } }

		public async Task Start()
		{
			// Start TcpListener
			listener.Start();

			Console.WriteLine("WebSocket server started.");

			// Load SSL certificate
			var certPath = "C:\\temp\\server_socket.pfx";
			var certPassword = "manic";
			var certificate = new X509Certificate2(certPath, certPassword);

			// Wait for incoming connections
			while (!cancellation.IsCancellationRequested) {
				TcpClient client = null;
				SslStream sslStream = null;

				try {
					// Accept TcpClient and create SslStream
					client = await listener.AcceptTcpClientAsync();
					sslStream = new SslStream(client.GetStream(), false);

					// Authenticate the server using the SSL certificate
					await sslStream.AuthenticateAsServerAsync(certificate, false, SslProtocols.Tls12, false);

					// Upgrade the connection to WebSocket
					var socket = await sslStream.UpgradeToWebSocketAsync();

					// Add WebSocket to handler
					handler.AddSocket(socket);
				} catch (Exception ex) {
					Console.WriteLine($"WebSocket connection error: {ex.Message}");
					sslStream?.Dispose();
					client?.Close();
				}
			}
		}

		public async Task Stop()
		{
			// Stop TcpListener and WebSocketHandler
			cancellation.Cancel();
			listener.Stop();
			await handler.Stop();
		}
	}
}
