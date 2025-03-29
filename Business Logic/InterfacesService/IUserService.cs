using BucketProject.BLL.Business_Logic.Entity;
using BucketProject.DAL.Models.Entities;
using BucketProject.DAL.Models.Enums;
using BucketProject.UI.ViewModels.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BucketProject.BLL.Business_Logic.InterfacesService
{
    public interface IUserService
    {
        bool Register(RegisterViewModel newUser);
       // User ValidateUser(string username, string password);
        User? LogIn(string username, string password);

        User GetUserByUsername();

        void UpdateUsername(string newUsername);

        Task UpdateProfilePicture(IFormFile? photoFile);

        

    }
}
