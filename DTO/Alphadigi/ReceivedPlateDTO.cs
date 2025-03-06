using System;

namespace Alphadigi_migration.DTO.Alphadigi;

public class ProcessPlateDTO
{
    public string ip { get; set; }
    public string plate { get; set; }
    public bool isRealPlate { get; set; }
    public bool isCad { get; set; }
    public string carImage { get; set; }
    public string plateImage { get; set; }
}


    public class AlarmInfoPlateDTO
{
    public AlarmInfoPlate AlarmInfoPlate { get; set; }
}

public class AlarmInfoPlate
{
    public int channel { get; set; }
    public string deviceName { get; set; }
    public string ipaddr { get; set; }
    public int openflag { get; set; }
    public string entityName { get; set; }
    public string TaxID { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public Result result { get; set; }
    public string serialno { get; set; }
}

public class Result
{
    public PlateResult PlateResult { get; set; }
}

public class PlateResult
{
    public int bright { get; set; }
    public int carBright { get; set; }
    public int carColor { get; set; }
    public int colorType { get; set; }
    public int colorValue { get; set; }
    public int confidence { get; set; }
    public int direction { get; set; }
    public string license { get; set; }
    public int Whitelist { get; set; }
    public Location location { get; set; }
    public TimeStamp timeStamp { get; set; }
    public int timeUsed { get; set; }
    public int triggerType { get; set; }
    public int type { get; set; }
    public int speed { get; set; }
    public RadarSpeed radarSpeed { get; set; }
    public int vehicleId { get; set; }
    public bool realplate { get; set; }
    public int retryflag { get; set; }
    public string imageFile { get; set; }
    public int imageFileLen { get; set; }
    public string imageFragmentFile { get; set; }
    public int imageFragmentFileLen { get; set; }
}

public class Location
{
    public Rect RECT { get; set; }
}

public class Rect
{
    public int left { get; set; }
    public int top { get; set; }
    public int right { get; set; }
    public int bottom { get; set; }
}

public class TimeStamp
{
    public Timeval Timeval { get; set; }
}

public class Timeval
{
    public long sec { get; set; }
    public long usec { get; set; }
}

public class RadarSpeed
{
    public Speed Speed { get; set; }
}

public class Speed
{
    public int PerHour { get; set; }
    public int Direction { get; set; }
}