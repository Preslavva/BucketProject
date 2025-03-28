using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BucketProject.DAL.Models.Entities;
using BucketProject.UI.ViewModels.ViewModels;

namespace BucketProject.BLL.Business_Logic.Mapping
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterViewModel, User>();
            

        }
    }
}
