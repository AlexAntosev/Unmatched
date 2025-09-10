namespace Unmatched.PlayerService.Api.Mapping
{
    using AutoMapper;

    using Unmatched.PlayerService.Api.Dto;
    using Unmatched.PlayerService.Domain.Entities;

    public class ApiMapper : Profile
    {
        public ApiMapper()
        {
            CreateMap<Player, PlayerDto>();
        }
    }
}
