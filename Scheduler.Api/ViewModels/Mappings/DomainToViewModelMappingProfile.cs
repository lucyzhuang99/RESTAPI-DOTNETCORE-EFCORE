using AutoMapper;
using Scheduler.API.ViewModels;
using Scheduler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Api.ViewModels.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(vm => vm.SchedulesCreated,
                map => map.MapFrom(u => u.SchedulesCreated.Count()));

            CreateMap<Schedule, ScheduleViewModel>()
                .ForMember(vm => vm.Creator,
                    map => map.MapFrom(s => s.Creator.Name))
                .ForMember(vm => vm.Attendees, map =>
                    map.MapFrom(s => s.Attendees.Select(a => a.UserId)));
            //CreateMap<Schedule, ScheduleDetailsViewModel>()
            //            .ForMember(vm => vm.Creator,
            //                map => map.MapFrom(s => s.Creator.Name))
            //            .ForMember(vm => vm.Status, map =>
            //                map.MapFrom(s => ((ScheduleStatus)s.Status).ToString()))
            //            .ForMember(vm => vm.Type, map =>
            //                map.MapFrom(s => ((ScheduleType)s.Type).ToString()));
        }
    }
}
