using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace Zyh.Web.Api.Controllers
{
    /// <summary>
    /// Websocket相关接口
    /// </summary>
    /// <returns></returns>
    [Route("api/[controller]")]
    [ApiController]
    public class WsController : ControllerBase
    {
        const int BufferLength = 1024 * 4;
        private readonly ILogger<AuthController> _logger;

        public WsController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpGet, Route("entry"), AllowAnonymous]
        public async Task Entry()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var clientIP = HttpContext.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString();

                await HandleWebSocketConnection(webSocket, clientIP);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        private async Task HandleWebSocketConnection(WebSocket webSocket, string? clientIp)
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var buffer = await ReceiveMessage(webSocket);
                var message = Encoding.UTF8.GetString(buffer);
                Console.WriteLine($"Server recv {message}");
                await SendResponse(webSocket, "Server send");
            }
        }

        private static async Task<Byte[]> ReceiveMessage(WebSocket webSocket)
        {
            var buffer = new byte[BufferLength];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
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

        private async Task SendResponse(WebSocket webSocket, string msg)
        {
            var dataBytes = Encoding.UTF8.GetBytes(msg).ToArray();
            await webSocket.SendAsync(new ArraySegment<byte>(dataBytes), WebSocketMessageType.Binary, true, CancellationToken.None);
        }
    }
}
