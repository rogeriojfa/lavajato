using System.Linq;
using GestaoEventos.Core.Aggregates.AuthAgg.Dtos;
using GestaoEventos.Core.Aggregates.AuthAgg.Entities;
using GestaoEventos.Core.Aggregates.EventAgg.Dtos;
using GestaoEventos.Core.Aggregates.EventAgg.Entities;

namespace GestaoEventos.Core.AutoMappers
{
    public class DomainToDtoMapping : AutoMapper.Profile
    {
        public DomainToDtoMapping()
        {
            CreateMap<User, UserForGetDto>();
            CreateMap<User, UserForRegisterDto>();
            CreateMap<Profile, ProfileForRegisterDto>();
            CreateMap<Profile, ProfileForGetDto>();
            CreateMap<Profile, ProfileForGetWithFunctionalitiesDto>()
                .ForMember(dest => dest.Functionalities,
                    opt => opt.MapFrom(x => x.ProfilesFunctionalities.Select(y => y.Functionality)));
            CreateMap<Functionality, FunctionalityForRegisterDto>();
            CreateMap<Functionality, FunctionalityForGetDto>();
            CreateMap<Event, EventForGetDto>();
            CreateMap<Event, EventForRegisterDto>();
            CreateMap<Event, EventForEditDto>();
            CreateMap<City, CityForGetDto>();
            CreateMap<EventType, EventTypeForGetDto>();
            CreateMap<Subscription, SubscriptionForRegisterDto>();
        }
    }
}
