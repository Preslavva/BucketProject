using BucketProject.Interfaces;
using BucketProject.Models;
using BucketProject.Repositories;
using BucketProject.ViewModels;

namespace BucketProject.Services
{
    public class UserService: IUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo, IHttpContextAccessor contextAccessor)
        {
            _userRepo = userRepo;
            _contextAccessor = contextAccessor;
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
