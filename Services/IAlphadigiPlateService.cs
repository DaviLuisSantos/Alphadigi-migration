using Alphadigi_migration.DTO.Alphadigi;

namespace Alphadigi_migration.Services;

public interface IAlphadigiPlateService
{
    Task<Object> ProcessPlate(ProcessPlateDTO plateReaded);
}
