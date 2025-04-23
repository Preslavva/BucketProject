using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLLBusiness_Logic.Domain;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.DAL.Models.Entities;
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

            UserEntity user = _userRepo.ValidateUser(username, password);

            if (user == null)
                return null;

            return _mapper.Map<User>(user); 
        }



        public bool Register(User userDomain)
        {
           
          UserEntity user = _mapper.Map<UserEntity>(userDomain);
          return _userRepo.Register(user);
        }

        public User GetUserByUsername()
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");

            UserEntity user = _userRepo.GetUserByUsername(username);
            User userDomain = _mapper.Map<User>(user);


            return userDomain;

        }

        public void UpdateUsername(string newUsername)
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");

            UserEntity user = _userRepo.GetUserByUsername(username);

            _contextAccessor.HttpContext.Session.SetString("Username", newUsername);

            _userRepo.UpdateName(user, newUsername);
        }

        public async Task UpdateProfilePicture(IFormFile? photoFile)
        {
            if (photoFile == null || photoFile.Length == 0)
                throw new Exception("No file uploaded.");

            string? username = _contextAccessor.HttpContext?.Session.GetString("Username");


            UserEntity? user = _userRepo.GetUserByUsername(username);

            using (var memoryStream = new MemoryStream())
            {
                await photoFile.CopyToAsync(memoryStream);
                byte[] photoBytes = memoryStream.ToArray();
                _userRepo.AddPhoto(user, photoBytes);
            }
        }
    }
}
