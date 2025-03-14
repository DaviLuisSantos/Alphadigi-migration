using AutoMapper;
using Alphadigi_migration.DTO;
using Alphadigi_migration.DTO.Alphadigi;

namespace Alphadigi_migration.Models;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<CreateAlphadigiDTO, Alphadigi>();
    }
}
