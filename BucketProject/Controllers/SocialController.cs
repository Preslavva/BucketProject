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
        public IActionResult Social(string searchTerm)
        {
            int uid = CurrentUserId;

            var incoming = _socialService.GetIncomingFriendRequests(uid);
            var friends = _socialService.GetFriends(uid);
            var nonFriends = _socialService.GetNonFriends(uid);
            var outgoingUsers = _socialService.GetOutgoingFriendRequests(uid);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                incoming = incoming.Where(u => u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                friends = friends.Where(u => u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                nonFriends = nonFriends.Where(u => u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                outgoingUsers = outgoingUsers
                                .Where(u => u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                                .ToList();
            }

            var potential = nonFriends
                .Concat(outgoingUsers)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();

            var vm = new SocialViewModel
            {
                IncomingRequests = incoming,
                Friends = friends,
                PotentialFriends = potential,
                OutgoingRequestIds = outgoingUsers.Select(u => u.Id).ToList()
            };
            ViewBag.SearchTerm = searchTerm ?? "";

            return View(vm);
        }



        [HttpPost]
        public IActionResult SendFriendRequest(int friendId)
        {
            _socialService.SendFriendRequest(CurrentUserId, friendId);
            return RedirectToAction("Social");
        }

        [HttpPost]
        public IActionResult AcceptFriendRequest(int requesterId)
        {
            _socialService.AcceptFriendRequest(CurrentUserId, requesterId);
            return RedirectToAction("Social");
        }

        [HttpPost]
        public IActionResult DeclineFriendRequest(int requesterId)
        {
            _socialService.DeclineFriendRequest(CurrentUserId, requesterId);
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

