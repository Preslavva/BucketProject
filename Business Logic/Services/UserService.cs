using AutoMapper;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLLBusiness_Logic.Domain;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.DAL.Models.Entities;
using Microsoft.AspNetCore.Http;


namespace BucketProject.BLL.Business_Logic.Services
{
    public class UserService: IUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _hasher;


        public UserService(IUserRepo userRepo, IHttpContextAccessor contextAccessor, IMapper mapper, IPasswordHasher hasher)
        {
            _userRepo = userRepo;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _hasher = hasher;
        }

        public User? LogIn(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Enter a password", nameof(password));

            UserEntity? entity = _userRepo.GetUserByUsername(username);
            if (entity is null)
                return null;   

            bool valid = _hasher.VerifyPassword(password, entity.Password, entity.Salt);
            if (!valid)
                return null;  

            return _mapper.Map<User>(entity);
        }




        public bool Register(User userDomain)
        {
            var (hash, salt) = _hasher.HashPassword(userDomain.Password);
            UserEntity entity = _mapper.Map<UserEntity>(userDomain);
            entity.SetPasswordAndSalt(hash, salt);
            return _userRepo.Register(entity);

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

            if (string.IsNullOrWhiteSpace(newUsername))
                throw new ArgumentException("newUsername cannot be empty or whitespace.", nameof(newUsername));

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
