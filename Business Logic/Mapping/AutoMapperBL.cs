using AutoMapper;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.DAL.Models.Entities;
using BucketProject.BLL.Business_Logic.DTOs;


namespace BucketProject.BLL.Business_Logic.Mapping
{
    public class AutoMapperBL: Profile
    {
        public AutoMapperBL()
        {
            CreateMap<User, UserEntity>().ReverseMap();
            CreateMap<GoalEntity, Goal>().ReverseMap();
            CreateMap<UserEntity, UserSummaryDTO>();
            //CreateMap<User, UserSummaryDTO>();
            CreateMap<GoalInvitation, GoalInviteDTO>();
            CreateMap<User, UserSummaryDTO>().ReverseMap();


        }
    }
}
