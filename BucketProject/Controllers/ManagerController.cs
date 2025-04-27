using Microsoft.AspNetCore.Mvc;

namespace BucketProject.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Manager()
        {
            return View();
        }
    }
}
