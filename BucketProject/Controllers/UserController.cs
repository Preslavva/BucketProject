using Microsoft.AspNetCore.Mvc;
using BucketProject.Models;
using BucketProject.Repositories;
using BucketProject.ViewModels;
using BucketProject.Services;
using BucketProject.Interfaces;



namespace BucketProject.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        
        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel user)
        {
            if (_userService.Register(user))
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
        public IActionResult LogIn(string username, string password)
        {
            User? loggedUser = _userService.LogIn(username, password);

            if (loggedUser!=null)
            {
                HttpContext.Session.SetString("Username", loggedUser.Username);
                return RedirectToAction("Index","Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Wrong username or password";
                return View(loggedUser);
            }
        }
    }
}
