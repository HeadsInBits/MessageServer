using System;
using System.IO;
using System.Net.Security;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace MessageServer.Models
{
	public class SslWebSocket : WebSocket
	{
		private readonly SslStream _sslStream;
		private readonly WebSocketHttpContext _webSocketHttpContext;

		public SslWebSocket(SslStream sslStream, WebSocketHttpContext webSocketHttpContext)
		{
			_sslStream = sslStream;
			_webSocketHttpContext = webSocketHttpContext;
		}

		public override WebSocketCloseStatus? CloseStatus => throw new NotImplementedException();

		public override string CloseStatusDescription => throw new NotImplementedException();

		public override WebSocketState State => throw new NotImplementedException();

		public override string SubProtocol => _webSocketHttpContext.Headers.ContainsKey("Sec-WebSocket-Protocol")
			? _webSocketHttpContext.Headers ["Sec-WebSocket-Protocol"]
			: null;

		public override void Abort()
		{
			_sslStream.Close();
		}

		public override async Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			// Implement close handshake logic here, if necessary
			_sslStream.Close();
			await Task.CompletedTask;
		}

		public override async Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			// Implement close handshake logic here, if necessary
			await _sslStream.FlushAsync(cancellationToken);
		}

		public override async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
		{
			WebSocketReceiveResult r = await ReceiveAsync(buffer, cancellationToken).ConfigureAwait(true);
			return r;
			
		}

		public override async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
		{
			// Implement WebSocket frame writing logic here
			throw new NotImplementedException();
		}

		public override void Dispose()
		{
			_sslStream.Dispose();
		}
	}
}
