using BucketProject.Data.Models;
using BucketProject.Data.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BucketProject.Business_Logic.InterfacesService
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
