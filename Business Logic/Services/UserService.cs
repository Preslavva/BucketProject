using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLL.Business_Logic.DTOs;
using BucketProject.BLL.Business_Logic.InterfacesRepo;
using BucketProject.DAL.Models.Entities;
using Microsoft.AspNetCore.Http;
using Exceptions.Exceptions;


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
        public int GetCurrentUserId()
        {
            string? username = _contextAccessor.HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                throw new UserNotLoggedInException();


            int userId = _userRepo.GetIdOfUser(username);
            return userId;
        }
        public User? LogIn(string username, string password)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(username))
                errors.Add("Enter your username");

            if (string.IsNullOrWhiteSpace(password))
                errors.Add("Enter your password");

            if (errors.Any())
                throw new ValidationExceptionCollection(errors);

            UserEntity entity = _userRepo.GetUserByUsername(username)
                ?? throw new InvalidLoginException();

            if (!_hasher.VerifyPassword(password, entity.Password, entity.Salt))
                throw new InvalidLoginException();

            return _mapper.Map<User>(entity);
        }



        public bool Register(User userDomain)
        {
            if (string.IsNullOrWhiteSpace(userDomain.Username))
                throw new ValidationException("Username is required.");
            if (userDomain.Username.Length<3)
                throw new ValidationException("Username must be at least 3 characters.");
            if (string.IsNullOrWhiteSpace(userDomain.Email))
                throw new ValidationException("Email is required.");
            if (string.IsNullOrWhiteSpace(userDomain.Nationality))
                throw new ValidationException("Nationality is required.");
            if (string.IsNullOrWhiteSpace(userDomain.Gender))
                throw new ValidationException("Gender is required.");
            if (string.IsNullOrWhiteSpace(userDomain.Password))
                throw new ValidationException("Password is required.");
            if (userDomain.Password.Length < 6)
                throw new ValidationException("Password must be at least 6 characters.");

            var emailAttr = new EmailAddressAttribute();
            if (!emailAttr.IsValid(userDomain.Email))
                throw new ValidationException("Email is not a valid address.");

            if (userDomain.Username.Length>20)
            {
                throw new ValidationException("Username must be under 20 characters");
            }

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
            if (string.IsNullOrWhiteSpace(newUsername))
                throw new ValidationException("The new Username cannot be empty or whitespace.");

            if (newUsername.Length > 20)
                throw new ValidationException("Username must be under 20 characters.");
            
            if (newUsername.Length < 3)
                throw new ValidationException("Username must be at least 3 characters.");

            string currentUsername = _contextAccessor.HttpContext!
                             .Session
                             .GetString("Username")
                         ?? throw new UserNotLoggedInException();

            UserEntity user = _userRepo.GetUserByUsername(currentUsername)
                       ?? throw new UserNotFoundException(currentUsername);


            _userRepo.UpdateName(user, newUsername);

            _contextAccessor.HttpContext!
                 .Session
                 .SetString("Username", newUsername);
        }


        public async Task UpdateProfilePicture(IFormFile? photoFile)
        {
            if (photoFile == null || photoFile.Length == 0)
                throw new ValidationException("Please select a photo to upload");

            string username = _contextAccessor.HttpContext!
                            .Session
                            .GetString("Username")
                        ?? throw new UserNotLoggedInException();



            UserEntity user = _userRepo.GetUserByUsername(username)
             ?? throw new UserNotFoundException(username);

            using (var memoryStream = new MemoryStream())
            {
                await photoFile.CopyToAsync(memoryStream);
                byte[] photoBytes = memoryStream.ToArray();
                _userRepo.AddPhoto(user, photoBytes);
            }
        }
    }
}
