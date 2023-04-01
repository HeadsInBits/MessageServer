using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageServer.Models
{
	public static class SslStreamExtensions
	{
		public static async Task<WebSocket> UpgradeToWebSocketAsync(this SslStream sslStream)
		{
			if (sslStream == null) {
				throw new ArgumentNullException(nameof(sslStream));
			}

			var buffer = new byte [8192];
			int bytesRead = await sslStream.ReadAsync(buffer, 0, buffer.Length);
			var requestString = Encoding.UTF8.GetString(buffer, 0, bytesRead);

			var webSocketContext = WebSocketHttpContext.Parse(requestString);
			if (webSocketContext == null || !webSocketContext.IsWebSocketRequest) {
				throw new InvalidOperationException("The request is not a valid WebSocket upgrade request.");
			}

			var response = webSocketContext.CreateWebSocketResponse();
			byte [] responseBuffer = Encoding.UTF8.GetBytes(response);
			await sslStream.WriteAsync(responseBuffer, 0, responseBuffer.Length);

			return new SslWebSocket(sslStream, webSocketContext);
		}
	}
}
