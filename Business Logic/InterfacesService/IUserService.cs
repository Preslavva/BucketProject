using BucketProject.Data.Models;
using BucketProject.Data.ViewModels;

namespace BucketProject.Business_Logic.InterfacesService
{
    public interface IUserService
    {
        bool Register(RegisterViewModel newUser);
       // User ValidateUser(string username, string password);
        User? LogIn(string username, string password);
    }
}
