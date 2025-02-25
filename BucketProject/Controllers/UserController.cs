using Microsoft.AspNetCore.Mvc;
using BucketProject.Models;
using BucketProject.Repositories;

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
        public IActionResult Register(User user)
        {
            if (DBHelper.Register(user))
            {
                return RedirectToAction("LogIn");
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
            User loggedUser = DBHelper.ValidateUser(username, password);

            if (loggedUser!=null)
            {
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
