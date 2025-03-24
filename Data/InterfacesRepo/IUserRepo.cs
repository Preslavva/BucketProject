using BucketProject.Data.Models;
using BucketProject.Data.ViewModels;

namespace BucketProject.Data.InterfacesRepo
{
    public interface IUserRepo
    {
        bool Register(RegisterViewModel user);
        User ValidateUser(string username, string password);

        User GetUserByUsername(string username);

        void UpdateName(User user, string username);
    }
}
