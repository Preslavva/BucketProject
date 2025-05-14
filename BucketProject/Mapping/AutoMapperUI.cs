using AutoMapper;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.BLLBusiness_Logic.Domain;
using BucketProject.DAL.Models.Enums;
using BucketProject.UI.ViewModels.ViewModels;

namespace BucketProject.UI.BucketProject.Mapping
{
    public class AutoMapperUI : Profile
    {
        public AutoMapperUI()
        {

            CreateMap<RegisterViewModel, User>();

            CreateMap<User, UserViewModel>();

            CreateMap<LogInViewModel, User>();


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


