
using AutoMapper;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.BLLBusiness_Logic.Domain;
using BucketProject.DAL.Models.Entities;


namespace BucketProject.BLL.Business_Logic.Mapping
{
    public class AutoMapperBL: Profile
    {
        public AutoMapperBL()
        {
            CreateMap<User, UserEntity>().ReverseMap();
            //CreateMap<UserEntity, User>();
            CreateMap<GoalEntity, Goal>().ReverseMap();


        }
    }
}
