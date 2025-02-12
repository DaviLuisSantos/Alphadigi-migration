
namespace Alphadigi_migration.DTO.Alphadigi;

public class HeartbeatDTO
{
    public Heartbeat heartbeat { get; set; }
}

public class Heartbeat
{
    public int countid { get; set; }
    public TimeStamp timeStamp { get; set; }
    public string outageTs { get; set; }
    public string startTs { get; set; }
    public string serialno { get; set; }
}
