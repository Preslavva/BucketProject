using Microsoft.AspNetCore.Http;
using BucketProject.BLL.Business_Logic.DTOs;

namespace BucketProject.BLL.Business_Logic.InterfacesService
{
    public interface IUserService
    {
        bool Register(User user);
        User? LogIn(string username, string password);

        User GetUserByUsername();

        void UpdateUsername(string newUsername);

        Task UpdateProfilePicture(IFormFile? photoFile);

        

    }
}
