using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace MessageServer.Models
{
	public class WebSocketHttpContext
	{
		private const string WebSocketUpgradeToken = "websocket";
		private const string WebSocketKeyHeader = "Sec-WebSocket-Key";
		private const string WebSocketAcceptGuid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

		public IDictionary<string, string> Headers { get; private set; }
		public bool IsWebSocketRequest { get; private set; }
		public string WebSocketKey { get; private set; }

		private WebSocketHttpContext(IDictionary<string, string> headers)
		{
			Headers = headers;
			IsWebSocketRequest = Headers.TryGetValue("Upgrade", out var upgradeHeader) && upgradeHeader.Equals(WebSocketUpgradeToken, StringComparison.OrdinalIgnoreCase);
			WebSocketKey = Headers.ContainsKey(WebSocketKeyHeader) ? Headers [WebSocketKeyHeader] : null;
		}

		public static WebSocketHttpContext Parse(string request)
		{
			if (string.IsNullOrEmpty(request)) {
				return null;
			}

			var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			var lines = request.Split(new [] { "\r\n" }, StringSplitOptions.None);

			foreach (var line in lines.Skip(1)) {
				int colonIndex = line.IndexOf(':');
				if (colonIndex > 0) {
					string key = line.Substring(0, colonIndex).Trim();
					string value = line.Substring(colonIndex + 1).Trim();
					headers [key] = value;
				}
			}

			return new WebSocketHttpContext(headers);
		}

		public string CreateWebSocketResponse()
		{
			if (!IsWebSocketRequest || string.IsNullOrEmpty(WebSocketKey)) {
				throw new InvalidOperationException("Cannot create a WebSocket response for an invalid request.");
			}

			string acceptKey = ComputeAcceptKey(WebSocketKey);
			var responseBuilder = new StringBuilder();
			responseBuilder.AppendLine("HTTP/1.1 101 Switching Protocols");
			responseBuilder.AppendLine("Upgrade: websocket");
			responseBuilder.AppendLine("Connection: Upgrade");
			responseBuilder.AppendLine($"Sec-WebSocket-Accept: {acceptKey}");
			responseBuilder.AppendLine();
			return responseBuilder.ToString();
		}

		private static string ComputeAcceptKey(string webSocketKey)
		{
			string combined = webSocketKey + WebSocketAcceptGuid;
			byte [] combinedBytes = Encoding.UTF8.GetBytes(combined);
			byte [] hashBytes;
			using (var sha1 = SHA1.Create()) {
				hashBytes = sha1.ComputeHash(combinedBytes);
			}
			return Convert.ToBase64String(hashBytes);
		}
	}
}
