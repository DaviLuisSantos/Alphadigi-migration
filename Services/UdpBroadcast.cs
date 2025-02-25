using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Alphadigi_migration.DTO.MonitorAcessoLinear;

namespace Alphadigi_migration.Services;

public class UdpBroadcastService
{
    private readonly ILogger<UdpBroadcastService> _logger;

    public UdpBroadcastService(ILogger<UdpBroadcastService> logger)
    {
        _logger = logger;
    }

    public async Task SendAsync(Object data, string ipAddress,bool dadosVeiculo)
    {
        int portSum = dadosVeiculo ? 33253 : 25253;
        try
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.EnableBroadcast = true;
                int port = ExtractPortFromIp(ipAddress);
                port = portSum + port;
                IPAddress broadcastAddress = IPAddress.Broadcast;
                IPEndPoint endPoint = new IPEndPoint(broadcastAddress, port);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = new UpperCaseNamingPolicy()
                };

                string jsonData = JsonSerializer.Serialize(data, options);
                byte[] buffer = Encoding.ASCII.GetBytes(jsonData);

                await socket.SendToAsync(new ArraySegment<byte>(buffer), SocketFlags.None, endPoint);

                _logger.LogInformation($"Broadcast message sent to {broadcastAddress}:{port}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending UDP broadcast message.");
        }
    }

    private int ExtractPortFromIp(string ipAddress)
    {
        string portString = ipAddress.Substring(ipAddress.Length - 3);
        return int.Parse(portString);
    }
}

public class UpperCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.ToUpper();
    }
}
