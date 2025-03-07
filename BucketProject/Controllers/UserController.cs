using Microsoft.AspNetCore.Mvc;
using BucketProject.Models;
using BucketProject.Repositories;
using BucketProject.ViewModels;

namespace BucketProject.Controllers
{
    public class UserController : Controller
    {
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel user)
        {
            if (UserHandlingDB.Register(user))
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
            User? loggedUser = UserHandlingDB.ValidateUser(username, password);

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
