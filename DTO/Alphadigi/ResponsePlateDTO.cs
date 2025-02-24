namespace Alphadigi_migration.DTO.Alphadigi;

public class ResponsePlateDTO
{
    public ResponseAlarmInfoPlate Response_AlarmInfoPlate  { get; set; }
}

public class ResponseAlarmInfoPlate
{
    public string info { get; set; }
    public string content { get; set; }
    public List<SerialData>? SerialData { get; set; }
}

public class SerialData
{
    public int SerialChannel { get; set; }
    public string Data { get; set; }
    public int DataLen { get; set; }
}