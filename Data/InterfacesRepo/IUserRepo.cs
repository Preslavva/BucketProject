using BucketProject.DAL.Models.Entities;

namespace BucketProject.DAL.Data.InterfacesRepo
{
    public interface IUserRepo
    {
        bool Register(UserEntity user);
        UserEntity ValidateUser(string username, string password);

        UserEntity? GetUserByUsername(string username);

        void UpdateName(UserEntity user, string username);

        void AddPhoto(UserEntity user, byte[] pictute);
    }
}
