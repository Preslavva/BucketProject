using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.DAL.Models.Entities;
using BucketProject.UI.ViewModels.ViewModels;
using Microsoft.AspNetCore.Http;
using AutoMapper;

namespace BucketProject.BLL.Business_Logic.Services
{
    public class UserService: IUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;


        public UserService(IUserRepo userRepo, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _userRepo = userRepo;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }

        public User? LogIn(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Enter a password");

            return _userRepo.ValidateUser(username, password);
        }


        public bool Register(RegisterViewModel newUser)
        {
            User user = _mapper.Map<User>(newUser);
          return _userRepo.Register(user);
        }

        public UserViewModel GetUserByUsername()
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");

            User user = _userRepo.GetUserByUsername(username);
            UserViewModel viewModel = _mapper.Map<UserViewModel>(user);


            return viewModel;

        }

        public void UpdateUsername(string newUsername)
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");

            User user = _userRepo.GetUserByUsername(username);

            _contextAccessor.HttpContext.Session.SetString("Username", newUsername);

            _userRepo.UpdateName(user, newUsername);
        }

        public async Task UpdateProfilePicture(IFormFile? photoFile)
        {
            if (photoFile == null || photoFile.Length == 0)
                throw new Exception("No file uploaded.");

            string? username = _contextAccessor.HttpContext?.Session.GetString("Username");


            User? user = _userRepo.GetUserByUsername(username);

            using (var memoryStream = new MemoryStream())
            {
                await photoFile.CopyToAsync(memoryStream);
                byte[] photoBytes = memoryStream.ToArray();
                _userRepo.AddPhoto(user, photoBytes);
            }
        }
    }
}
