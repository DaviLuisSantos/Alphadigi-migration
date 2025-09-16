using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Sockets;
using System.Text;

[ApiController]
[Route("api/debug")]
public class UdpSnifferController : ControllerBase
{
    private readonly ILogger<UdpSnifferController> _logger;

    public UdpSnifferController(ILogger<UdpSnifferController> logger)
    {
        _logger = logger;
    }

    [HttpGet("start-sniffer/{port}")]
    public async Task<IActionResult> StartSniffer(int port = 33253)
    {
        _ = Task.Run(() => StartUdpSniffer(port));
        return Ok(new { Message = $"Sniffer iniciado na porta {port}", Port = port });
    }

    private async Task StartUdpSniffer(int port)
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        var endPoint = new IPEndPoint(IPAddress.Any, port);
        socket.Bind(endPoint);

        Console.WriteLine($"🔍 SNIFFER INICIADO - Porta: {port}");
        Console.WriteLine($"📡 Aguardando dados UDP...");

        var buffer = new byte[4096];

        while (true)
        {
            try
            {
                var result = await socket.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, endPoint);
                string message = Encoding.ASCII.GetString(buffer, 0, result.ReceivedBytes);

                Console.WriteLine($"\n📨 RECEBIDO:");
                Console.WriteLine($"📍 De: {result.RemoteEndPoint}");
                Console.WriteLine($"📊 Bytes: {result.ReceivedBytes}");
                Console.WriteLine($"📝 Conteúdo: {message}");
                Console.WriteLine($"🔢 Hex: {BitConverter.ToString(buffer, 0, Math.Min(result.ReceivedBytes, 100))}");
                Console.WriteLine($"⏰ {DateTime.Now:HH:mm:ss}");
                Console.WriteLine(new string('-', 50));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro no sniffer: {ex.Message}");
            }
        }
    }
}