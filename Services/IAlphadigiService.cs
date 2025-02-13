using Alphadigi_migration.Models;

namespace Alphadigi_migration.Services
{
    public interface IAlphadigiService
    {
        Task<Alphadigi> GetOrCreate(string ip);
    }
}
