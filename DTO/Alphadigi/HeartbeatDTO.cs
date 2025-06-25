
namespace Alphadigi_migration.DTO.Alphadigi;

public interface IHeartbeatRequest { }

public class HeartbeatDTO : IHeartbeatRequest
{
    public Heartbeat heartbeat { get; set; }
}

public class ReturnAddPlateDTO : IHeartbeatRequest
{
    public ResponseAddWhiteList Response_AddWhiteList { get; set; }
}

public class ReturnDelPlateDTO : IHeartbeatRequest
{
    public ResponseDelWhiteListAll Response_DelWhiteListAll { get; set; }
}

public class Heartbeat
{
    public int countid { get; set; }
    public TimeStamp timeStamp { get; set; }
    public string outageTs { get; set; }
    public string startTs { get; set; }
    public string serialno { get; set; }
}

public class ResponseAddWhiteList
{
    string response { get; set; }
    public string serialno { get; set; }
}

public class ResponseDelWhiteListAll
{
    public string response { get; set; }
    public string serialno { get; set; }
}
