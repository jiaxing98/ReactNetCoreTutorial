using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Activity, Activity>();
        }
    }
}