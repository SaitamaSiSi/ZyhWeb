using System.Net.WebSockets;
using System.Text;

namespace Zyh.Client.Test
{
    public class WsClient : IDisposable
    {
        const int BufferLength = 1024 * 4;
        private ClientWebSocket _ws;
        private Uri _serverUri;
        private CancellationTokenSource _cts;

        public event Action<string>? Connected;
        public event Action<string>? Disconnected;
        public event Action<object>? MessageReceived;

        public WsClient(string url)
        {
            _serverUri = new Uri(url);
            _ws = new ClientWebSocket();
            _cts = new CancellationTokenSource();
        }

        public async Task ConnectAsync()
        {
            await _ws.ConnectAsync(_serverUri, _cts.Token);
            Connected?.Invoke("Connected to " + _serverUri);

            _ = ReceiveLoopAsync();
        }

        private async Task ReceiveLoopAsync()
        {
            var buffer = new byte[BufferLength];
            try
            {
                while (_ws.State == WebSocketState.Open)
                {
                    buffer = await ReceiveMessage(_ws);
                    var message = Encoding.UTF8.GetString(buffer);
                    Console.WriteLine($"Client recv {message}");
                }
            }
            catch (Exception ex)
            {
                Disconnected?.Invoke("Connection error: " + ex.Message);
                await TryReconnectAsync();
            }
        }

        public async Task SendMessageAsync(string message)
        {
            await SendBinaryAsync(message);
        }

        private async Task SendBinaryAsync(string msg)
        {
            var dataBytes = Encoding.UTF8.GetBytes(msg).ToArray();
            await _ws.SendAsync(new ArraySegment<byte>(dataBytes), WebSocketMessageType.Binary, true, _cts.Token);
        }

        private async Task<Byte[]> ReceiveMessage(WebSocket webSocket)
        {
            var buffer = new byte[BufferLength];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await CloseAsync();
            }
            else
            {
                if (result.Count < buffer.Length)
                {
                    buffer = buffer.Take(result.Count).ToArray();
                }
                while (!result.EndOfMessage)
                {
                    var tmpBuffer = new byte[BufferLength];
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(tmpBuffer), CancellationToken.None);
                    if (result.Count > 0)
                    {
                        buffer = buffer.Concat(tmpBuffer.Take(result.Count)).ToArray();
                    }
                    Thread.Yield();
                }
            }
            return buffer;
        }

        public async Task CloseAsync()
        {
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            Disconnected?.Invoke("Normal closure");
        }

        private async Task TryReconnectAsync()
        {
            Dispose();
            _ws = new ClientWebSocket();
            await ConnectAsync();
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _ws?.Dispose();
            _cts?.Dispose();
        }
    }
}
