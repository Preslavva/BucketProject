using BucketProject.Models;
using BucketProject.ViewModels;

namespace BucketProject.Interfaces
{
    public interface IUserService
    {
        bool Register(RegisterViewModel newUser);
       // User ValidateUser(string username, string password);
        User? LogIn(string username, string password);
    }
}
