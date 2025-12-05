
using System.Text.Json.Serialization;

namespace Alphadigi_migration.Domain.DTOs.Alphadigi;

public class ResponseHeathbeatDTO
{
    [JsonPropertyName("Response_Heartbeat")]
    public Response_AlarmInfoPlate Response_Heartbeat { get; set; }
    
}
