
using MediatR;

namespace Alphadigi_migration.Application.Commands.SendUdpBroadcast;

public class SendUdpBroadcastCommand : IRequest
{
    public object Data { get; set; }
    public string IpAddress { get; set; }
    public bool IsVehicleData { get; set; }

    public SendUdpBroadcastCommand(object data, string ipAddress, bool isVehicleData)
    {
        Data = data;
        IpAddress = ipAddress;
        IsVehicleData = isVehicleData;
    }
}