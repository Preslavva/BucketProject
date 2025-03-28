using BucketProject.DAL.Models.Entities;

namespace BucketProject.DAL.Data.InterfacesRepo
{
    public interface IUserRepo
    {
        bool Register(User user);
        User ValidateUser(string username, string password);

        User? GetUserByUsername(string username);

        void UpdateName(User user, string username);

        void AddPhoto(User user, byte[] pictute);
    }
}
