using AutoMapper;
using Alphadigi_migration.Domain.DTOs.Alphadigi;

namespace Alphadigi_migration.Application.Mapping;


public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateAlphadigiDTO, Alphadigi_migration.Domain.Entities.Alphadigi>();
        CreateMap<UpdateAlphadigiDTO, Alphadigi_migration.Domain.Entities.Alphadigi>();
    }
}
