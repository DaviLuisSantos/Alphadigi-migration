using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Models;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IAlphadigiHearthBeatService
{
    Task<object> ProcessHearthBeat(string ip);
    Task<object> HandleAlphadigiStage(Alphadigi_migration.Domain.Entities.Alphadigi alphadigi);
    DeleteWhiteListAllDTO HandleDelete(Alphadigi_migration.Domain.Entities.Alphadigi alphadigi);
    Task<bool> HandleDeleteReturn(string ip);
    Task<AddWhiteListDTO> HandleCreate(Alphadigi_migration.Domain.Entities.Alphadigi alphadigi);
    Task<bool> HandleCreateReturn(string ip);
}
