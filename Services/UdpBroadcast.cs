using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Alphadigi_migration.DTO.MonitorAcessoLinear;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;

namespace Alphadigi_migration.Services;

public class UdpBroadcastService
{
    private readonly ILogger<UdpBroadcastService> _logger;

    public UdpBroadcastService(ILogger<UdpBroadcastService> logger)
    {
        _logger = logger;
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

                // Remove acentos dos valores do objeto
                RemoveAcentosFromObject(data);

                string jsonData = JsonConvert.SerializeObject(data, settings);
                byte[] buffer = Encoding.ASCII.GetBytes(jsonData);

                await socket.SendToAsync(new ArraySegment<byte>(buffer), SocketFlags.None, endPoint);

                _logger.LogInformation($"//---------------------Broadcast message sent to {broadcastAddress}:{port}\"---------------------//");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending UDP broadcast message.");
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
            if (property.PropertyType == typeof(string))
            {
                string value = (string)property.GetValue(obj);
                if (value != null)
                {
                    string normalizedValue = RemoveAcentos(value);
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