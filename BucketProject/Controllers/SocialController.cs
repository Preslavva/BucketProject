using AutoMapper;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProjetc.BLL.Business_Logic.InterfacesService;
using Microsoft.AspNetCore.Mvc;
using BucketProject.UI.ViewModels.ViewModels;
using BucketProject.BLL.Business_Logic.Services;
using BucketProject.BLLBusiness_Logic.Domain;


namespace BucketProject.Controllers
{
    public class SocialController : Controller
    {
        private readonly ISocialService _socialService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;



        public SocialController(ISocialService socialService, IMapper mapper, IConfiguration configuration, IUserService userService)
        {
            _socialService= socialService;
            _mapper = mapper;
            _configuration = configuration;
            _userService = userService;
        }
        private int CurrentUserId
        {
            get
            {
                
                User currentUser = _userService.GetUserByUsername();
                return currentUser.Id;
            }
        }


        [HttpGet]
        // Controllers/SocialController.cs
        [HttpGet]
        public IActionResult Social(string searchTerm)
        {
            var uid = CurrentUserId;

            // Get the full lists first
            var friends = _socialService.GetFriends(uid);
            var nonFriends = _socialService.GetNonFriends(uid);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Filter both lists by username (case-insensitive)
                friends = friends
                    .Where(u => u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                nonFriends = nonFriends
                    .Where(u => u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var vm = new SocialViewModel
            {
                Friends = friends,
                NonFriends = nonFriends
            };

            ViewBag.SearchTerm = searchTerm ?? string.Empty;
            return View(vm);
        }


        public IActionResult AddFriend(int friendId)
        {
            _socialService.AddFriend(CurrentUserId, friendId);
            return RedirectToAction("Social");
        }

        [HttpPost]
      
        public IActionResult RemoveFriend(int friendId)
        {
            _socialService.RemoveFriend(CurrentUserId, friendId);
            return RedirectToAction("Social");
        }
    }
}
