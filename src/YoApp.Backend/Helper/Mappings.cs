﻿using AutoMapper;
using YoApp.Backend.Models;
using YoApp.DataObjects.Users;

namespace YoApp.Backend.Helper
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<UserDto, ApplicationUser>()
                .ForMember(a => a.UserName, o => o.MapFrom(u => u.PhoneNumber));

            CreateMap<ApplicationUser, UserDto>()
                .ForMember(u => u.PhoneNumber, o => o.MapFrom(a => a.UserName));
        }
    }
}
