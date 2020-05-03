using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Api.ViewModels.Mappings
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            (new MapperConfiguration(x => {
                x.AddProfile<DomainToViewModelMappingProfile>();
            })).CreateMapper();
        }
    }
}
