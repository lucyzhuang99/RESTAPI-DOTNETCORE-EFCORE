using AutoMapper;
using Scheduler.API.ViewModels;
using Scheduler.Model;

namespace Scheduler.Api.ViewModels.Mappings
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Schedule, ScheduleViewModel>();
            CreateMap<ScheduleViewModel, Schedule>();
            CreateMap<Schedule, ScheduleDetailsViewModel>();
            CreateMap<ScheduleDetailsViewModel, Schedule>();
            CreateMap<User, UserViewModel>();
            CreateMap<UserViewModel, User>();
        }
    }
}
