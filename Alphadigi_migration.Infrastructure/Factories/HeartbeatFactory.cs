using Alphadigi_migration.Domain.DTOs.Alphadigi;
using System.Text.Json;

namespace Alphadigi_migration.Infrastructure.Factories;

public interface IHeartbeatFactory
{
    IHeartbeatRequest? Create(string json);
}

public class HeartbeatFactory : IHeartbeatFactory
{
    public IHeartbeatRequest? Create(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        // Verifica qual tipo de DTO deve ser usado com base na presença das chaves
        if (root.TryGetProperty("heartbeat", out _))
        {
            return JsonSerializer.Deserialize<HeartbeatDTO>(json);
        }
        else if (root.TryGetProperty("Response_AddWhiteList", out _))
        {
            return JsonSerializer.Deserialize<ReturnAddPlateDTO>(json);
        }
        else if (root.TryGetProperty("Response_DelWhiteListAll", out _))
        {
            return JsonSerializer.Deserialize<ReturnDelPlateDTO>(json);
        }

        return null; // Tipo não reconhecido
    }
}
