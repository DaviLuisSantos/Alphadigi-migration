using AutoMapper;
using Alphadigi_migration.DTO;
using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.DTO.PlacaLida;

namespace Alphadigi_migration.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateAlphadigiDTO, Alphadigi>();
        CreateMap<UpdateAlphadigiDTO, Alphadigi>();
    }
}
