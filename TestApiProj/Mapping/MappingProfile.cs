


using AutoMapper;
using TestApiProj.DTOS;
using TestApiProj.MainEntity;

namespace TestApiProj.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RefreshToken, RefreshTokensDTO>();

             CreateMap<RefreshTokensDTO, RefreshToken>();
        }
    }
}
