using BucketProject.DAL.Models.Entities;

namespace BucketProject.BLL.Business_Logic.InterfacesRepo;

public interface IUserRepo
    {
        bool Register(UserEntity user);

        UserEntity? GetUserByUsername(string username);

        void UpdateName(UserEntity user, string username);

        void AddPhoto(UserEntity user, byte[] pictute);
        int GetIdOfUser(string username);
         public UserEntity? GetUserById(int id); 
    }

