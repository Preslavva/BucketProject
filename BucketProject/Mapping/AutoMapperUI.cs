using AutoMapper;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.BLL.Business_Logic.DTOs;
using BucketProject.DAL.Models.Enums;
using BucketProject.UI.ViewModels.ViewModels;

namespace BucketProject.UI.BucketProject.Mapping
{
    public class AutoMapperUI : Profile
    {
        public AutoMapperUI()
        {

            //CreateMap<RegisterViewModel, User>();

            CreateMap<RegisterViewModel, User>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
    .ForMember(dest => dest.Salt, opt => opt.Ignore())
    .ForMember(dest => dest.Picture, opt => opt.Ignore())
    .ForMember(dest => dest.Role, opt => opt.Ignore());


            CreateMap<User, UserViewModel>();

            //CreateMap<LogInViewModel, User>();

            CreateMap<LogInViewModel, User>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.Email, opt => opt.Ignore())
    .ForMember(dest => dest.DateOfBirth, opt => opt.Ignore())
    .ForMember(dest => dest.Nationality, opt => opt.Ignore())
    .ForMember(dest => dest.Gender, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
    .ForMember(dest => dest.Salt, opt => opt.Ignore())
    .ForMember(dest => dest.Picture, opt => opt.Ignore())
    .ForMember(dest => dest.Role, opt => opt.Ignore());



            CreateMap<Goal, GoalViewModel>()
    .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
    .ForMember(dest => dest.Recipients, opt => opt.Ignore());


            //CreateMap<GoalViewModel, Goal>()
            //    .ForMember(dest => dest.Category, opt => opt.MapFrom(src => Enum.Parse<Category>(src.Category)))
            //    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<GoalType>(src.Type)));

            CreateMap<GoalViewModel, Goal>()
          .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
          .ForMember(dest => dest.Deadline, opt => opt.Ignore())
          .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
          .ForMember(dest => dest.IsPostponed, opt => opt.Ignore())
          .ForMember(dest => dest.Users, opt => opt.Ignore())
          .ForMember(dest => dest.Category, opt => opt.MapFrom(src => Enum.Parse<Category>(src.Category)))
          .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<GoalType>(src.Type)))
          .ForMember(dest => dest.Recipients, opt => opt.Ignore()); // ✅ Fix



            //        CreateMap<Goal, HistoryViewModel>()
            //        .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            //        .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()));

            //        CreateMap<Goal, NotificationViewModel>()
            //.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            //.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<Goal, HistoryViewModel>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
                .ForMember(dest => dest.ChildGoals, opt => opt.Ignore())
                .ForMember(dest => dest.Recipients, opt => opt.Ignore());


            CreateMap<Goal, NotificationViewModel>()
    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
    .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.Message, opt => opt.Ignore())
    .ForMember(dest => dest.TypeOfNotification, opt => opt.Ignore())
    .ForMember(dest => dest.TriggeredByUserId, opt => opt.Ignore())
    .ForMember(dest => dest.Recipients, opt => opt.Ignore());




        }
    }
}


