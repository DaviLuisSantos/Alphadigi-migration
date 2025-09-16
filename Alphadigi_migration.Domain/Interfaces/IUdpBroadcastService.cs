using Alphadigi_migration.Domain.DTOs.SystemBroadcast;
using Alphadigi_migration.Domain.DTOs.VehicleBroadcast;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IUdpBroadcastService
{
    Task SendVehicleDataAsync(VehicleBroadcastDTO data, string ipAddress);
    Task SendSystemDataAsync(SystemBroadcastDTO data, string ipAddress);

    Task SendStringAsync(string data, string ipAddress, bool dadosVeiculo);
    Task SendAsync(object data, string ipAddress, bool dadosVeiculo);
}
