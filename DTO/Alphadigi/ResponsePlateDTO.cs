namespace Alphadigi_migration.DTO.Alphadigi;

public class ResponsePlateDTO
{
    public ResponseAlarmInfoPlate Response_AlarmInfoPlate  { get; set; }
}

public class ResponseAlarmInfoPlate
{
    public string info { get; set; }
    public List<SerialData>? serialData { get; set; }
}

public class SerialData
{
    public int serialChannel { get; set; }
    public string data { get; set; }
    public int dataLen { get; set; }
}