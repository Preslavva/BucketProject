
using BucketProject.DAL.Models.Entities;

using BucketProject.UI.ViewModels.ViewModels;
using Microsoft.AspNetCore.Http;
using BucketProject.BLLBusiness_Logic.Domain;

namespace BucketProject.BLL.Business_Logic.InterfacesService
{
    public interface IUserService
    {
        bool Register(UserDomain user);
        UserDomain? LogIn(string username, string password);

        UserDomain GetUserByUsername();

        void UpdateUsername(string newUsername);

        Task UpdateProfilePicture(IFormFile? photoFile);

        

    }
}
