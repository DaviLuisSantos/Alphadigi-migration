using System.Text.Json.Serialization;

namespace Alphadigi_migration.Domain.DTOs.Alphadigi;

// Esta é a classe PRINCIPAL que a AlphaDigi espera
public class AlarmInfoPlateResponseDTO
{
    [JsonPropertyName("Response_AlarmInfoPlate")]
    public Response_AlarmInfoPlate ResponseAlarmInfoPlate { get; set; }
}

