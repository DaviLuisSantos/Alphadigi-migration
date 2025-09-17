using Alphadigi_migration.Domain.DTOs.SystemBroadcast;
using Alphadigi_migration.Domain.DTOs.VehicleBroadcast;
using Alphadigi_migration.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Alphadigi_migration.Application.Service;

public class UdpBroadcastService : IUdpBroadcastService
{
    private readonly ILogger<UdpBroadcastService> _logger;

    public UdpBroadcastService(ILogger<UdpBroadcastService> logger)
    {
        _logger = logger;
    }

    public async Task SendVehicleDataAsync(VehicleBroadcastDTO data, string ipAddress)
    {
        await SendAsync(data, ipAddress, true);
    }
    public async Task SendStringAsync(string data, string ipAddress, bool dadosVeiculo)
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

                // Apenas converte a string para bytes, sem serialização JSON
                byte[] buffer = Encoding.ASCII.GetBytes(data);

                await socket.SendToAsync(new ArraySegment<byte>(buffer), SocketFlags.None, endPoint);

                _logger.LogInformation($"//---------------------Broadcast string message sent to {broadcastAddress}:{port}\"---------------------//");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending UDP broadcast string message.");
        }
    }

    public async Task SendSystemDataAsync(SystemBroadcastDTO data, string ipAddress)
    {
        await SendAsync(data, ipAddress, false);
    }

    public async Task SendAsync(object data, string ipAddress, bool dadosVeiculo)
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

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CustomContractResolver()
                };

                // Remove acentos dos valores do objeto (igual na original)
                RemoveAcentosFromObject(data);

                string jsonData = JsonConvert.SerializeObject(data, settings); // Newtonsoft
                byte[] buffer = Encoding.ASCII.GetBytes(jsonData); // ASCII

                await socket.SendToAsync(new ArraySegment<byte>(buffer), SocketFlags.None, endPoint);
                _logger.LogInformation($"//---------------------Broadcast message sent to {broadcastAddress}:{port}\"---------------------//");
            
            _logger.LogInformation($"Broadcast message sent to {broadcastAddress}:{port}");
                Console.WriteLine($"Broadcast enviado para {broadcastAddress}:{port}");
                Console.WriteLine($" Dados: {jsonData}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending UDP broadcast message.");
            Console.WriteLine($" Erro no broadcast: {ex.Message}");
        }
    }

    private int ExtractPortFromIp(string ipAddress)
    {
        string[] parts = ipAddress.Split('.');
        string lastPart = parts[^1];
        string portString = lastPart.PadLeft(3, '0');
        return int.Parse(portString);
    }

    private void RemoveAcentosFromObject(object obj)
    {
        if (obj == null) return;

        var properties = obj.GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(string) && property.CanWrite)
            {
                var value = (string)property.GetValue(obj);
                if (!string.IsNullOrEmpty(value))
                {
                    var normalizedValue = RemoveAcentos(value);
                    property.SetValue(obj, normalizedValue);
                }
            }
            else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                RemoveAcentosFromObject(property.GetValue(obj));
            }
        }
    }

    private string RemoveAcentos(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}

public class CustomContractResolver : DefaultContractResolver
{
    protected override string ResolvePropertyName(string propertyName)
    {
        return propertyName.ToUpper(); 
    }
}