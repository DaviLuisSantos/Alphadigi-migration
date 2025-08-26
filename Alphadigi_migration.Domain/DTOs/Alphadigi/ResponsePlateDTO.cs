namespace Alphadigi_migration.Domain.DTOs.Alphadigi;

public class ResponsePlateDTO
{
    public ResponseAlarmInfoPlate Response_AlarmInfoPlate  { get; set; }
}


public class SerialData
{
    public int serialChannel { get; set; }
    public string data { get; set; }
    public int dataLen { get; set; }
}