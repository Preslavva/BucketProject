using BucketProject.Models;
using BucketProject.ViewModels;

namespace BucketProject.Interfaces
{
    public interface IUserRepo
    {
        bool Register(RegisterViewModel user);
        User ValidateUser(string username, string password);
    }
}
