using BucketProject.Business_Logic.InterfacesService;
using BucketProject.Data.InterfacesRepo;
using BucketProject.Data.Models;
using BucketProject.Data.Repositories;
using BucketProject.Data.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BucketProject.Business_Logic.Services
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

        public User GetUserByUsername()
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");

            User user = _userRepo.GetUserByUsername(username);

            return user;

        }

        public void UpdateUsername(string newUsername)
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");

            User user = _userRepo.GetUserByUsername(username);

            _contextAccessor.HttpContext.Session.SetString("Username", newUsername);

            _userRepo.UpdateName(user, newUsername);
        }
    }
}
