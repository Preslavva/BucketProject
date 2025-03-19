using System.Diagnostics;
using BucketProject.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace BucketProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
    
            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View();
        }

        //public IActionResult WeekGoals()
        //{
        //    return View();
        //}
        //public IActionResult MonthGoals()
        //{
        //    return View();
        //}
        //public IActionResult YearGoals()
        //{
        //    return View();
        //}

        public IActionResult Privacy()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
