using AutoMapper;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProjetc.BLL.Business_Logic.InterfacesService;
using Microsoft.AspNetCore.Mvc;
using BucketProject.UI.ViewModels.ViewModels;
using BucketProject.BLL.Business_Logic.Services;
using BucketProject.BLL.Business_Logic.DTOs;


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
        
        [HttpGet]
        public IActionResult Social()
        {
            int uid = _userService.GetCurrentUserId();

            List<UserSummaryDTO> friends = _socialService.GetFriends(uid);
            List<UserSummaryDTO> incoming = _socialService.GetIncomingFriendRequests(uid);
            List<UserSummaryDTO> outgoing = _socialService.GetOutgoingFriendRequests(uid);

            SocialViewModel vm = new SocialViewModel
            {
                Friends = friends,
                IncomingRequests = incoming,
                OutgoingRequests = outgoing,
                OutgoingRequestIds = outgoing.Select(x => x.Id).ToList()
            };

            return View(vm);
        }



        [HttpGet]
        public IActionResult Search(string searchTerm, int page = 1, int pageSize = 2 )
        {
            int uid = _userService.GetCurrentUserId();
            searchTerm ??= "";

            var friends = _socialService.GetFriends(uid)
                .Where(u => u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var incoming = _socialService.GetIncomingFriendRequests(uid)
                .Where(u => u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var outgoing = _socialService.GetOutgoingFriendRequests(uid)
                .Where(u => u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var potential = _socialService.GetNonFriends(uid, searchTerm, page, pageSize);

            int total = _socialService.CountNonFriends(uid, searchTerm);
            int totalPages = (int)Math.Ceiling((double)total / pageSize);

            var vm = new SocialViewModel
            {
                Friends = friends,
                IncomingRequests = incoming,
                PotentialFriends = potential,
                OutgoingRequestIds = outgoing.Select(x => x.Id).ToList()
            };

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchTerm = searchTerm;

            return PartialView("_SearchResults", vm);
        }



        [HttpPost]
        public IActionResult SendFriendRequest(int friendId)
        {
            _socialService.SendFriendRequest(_userService.GetCurrentUserId(), friendId);
            return RedirectToAction("Social");
        }

        [HttpPost]
        public IActionResult AcceptFriendRequest(int requesterId)
        {
            _socialService.AcceptFriendRequest(_userService.GetCurrentUserId(), requesterId);
            return RedirectToAction("Social");
        }

        [HttpPost]
        public IActionResult DeclineFriendRequest(int requesterId)
        {
            _socialService.DeclineFriendRequest(_userService.GetCurrentUserId(), requesterId);
            return RedirectToAction("Social");
        }

        [HttpPost]
        public IActionResult RemoveFriend(int friendId)
        {
            _socialService.RemoveFriend(_userService.GetCurrentUserId(), friendId);
            return RedirectToAction("Social");
        }
    }
}

