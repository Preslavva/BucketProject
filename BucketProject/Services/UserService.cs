using BucketProject.Models;
using BucketProject.Repositories;
using BucketProject.ViewModels;

namespace BucketProject.Services
{
    public class UserService
    {
        private readonly UserRepo _userRepo;

        public UserService(UserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public User? LogIn(string username, string password)
        {
            return _userRepo.ValidateUser(username, password);
        }

        public bool Register(RegisterViewModel newUser)
        {
          return _userRepo.Register(newUser);
        }
    }
}
