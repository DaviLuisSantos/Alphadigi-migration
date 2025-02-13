using Alphadigi_migration.Models;
using Alphadigi_migration.DTO.Alphadigi;
using System.Threading.Tasks;

namespace Alphadigi_migration.Services
{
    public interface IAlphadigiHearthBeatService
    {
        Task<object> ProcessHearthBeat(string ip);
        Task<object> HandleAlphadigiStage(Alphadigi alphadigi);
        DeleteWhiteListAllDTO HandleDelete(Alphadigi alphadigi);
        Task<AddWhiteListDTO> HandleCreate(Alphadigi alphadigi);
    }
}
