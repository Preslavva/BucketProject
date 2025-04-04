using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.DAL.Models.Entities;
using BucketProject.DAL.Models.Enums;
using BucketProject.UI.ViewModels.ViewModels;

namespace BucketProject.BLL.Business_Logic.Mapping
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterViewModel, User>();

            CreateMap<User, UserViewModel>();

            CreateMap<GoalEntity, Goal>().ReverseMap();



            CreateMap<Goal, GoalViewModel>()
    .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            CreateMap<GoalViewModel, Goal>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => Enum.Parse<Category>(src.Category)))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<GoalType>(src.Type)));

            CreateMap<Goal, HistoryViewModel>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()));

            CreateMap<Goal, NotificationViewModel>()
    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
    .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

        }
    }
}
