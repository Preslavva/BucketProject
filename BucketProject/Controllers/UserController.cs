using Microsoft.AspNetCore.Mvc;
using BucketProject.DAL.Models.Entities;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLLBusiness_Logic.Domain;
using BucketProject.UI.ViewModels.ViewModels;
using BucketProject.DAL.Data.InterfacesRepo;
using System.Security.Claims;
using AutoMapper;


namespace BucketProject.UI.BucketProject.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        
        public UserController(IUserService userService, IPasswordHasher passwordHasher, IMapper mapper)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel user)
        {
            UserDomain newUser = _mapper.Map<UserDomain>(user);
            if (_userService.Register(newUser))
            {
                return RedirectToAction("LogIn","User");
            }
            else
            {
                ViewBag.ErrorMessage = "Username or email is already taken";
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(LogInViewModel user)
        {
            try
            {
                UserDomain loggedUser = _mapper.Map<UserDomain>(user);
                loggedUser = _userService.LogIn(user.Username, user.Password);

                if (loggedUser != null)
                {
                    HttpContext.Session.SetString("Username", loggedUser.Username);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Wrong username or password";
                    return View();
                }
            }
            catch (ArgumentException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }



        [HttpGet]
        public IActionResult Account()
        {

            UserViewModel userViewModel = _userService.GetUserByUsername();

            return View(userViewModel);
        }

        [HttpPost]
        public IActionResult UpdateUsername(string newUsername)
        {
            _userService.UpdateUsername(newUsername);

            return RedirectToAction("Account", "User");
        }

        [HttpPost]
        public async Task<IActionResult> AddPhoto(IFormFile photoFile)
        {
            await _userService.UpdateProfilePicture(photoFile);
            return RedirectToAction("Account", "User");
        }

    }
}
