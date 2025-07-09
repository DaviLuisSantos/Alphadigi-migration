using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.Models;

namespace Alphadigi_migration.Interfaces;

public interface IAlphadigiHearthBeatService
{
    Task<object> ProcessHearthBeat(string ip);
    Task<object> HandleAlphadigiStage(Alphadigi alphadigi);
    DeleteWhiteListAllDTO HandleDelete(Alphadigi alphadigi);
    Task<bool> HandleDeleteReturn(string ip);
    Task<AddWhiteListDTO> HandleCreate(Alphadigi alphadigi);
    Task<bool> HandleCreateReturn(string ip);
}
