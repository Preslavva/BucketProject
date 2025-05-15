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
        public IActionResult Social()
        {
            int uid = CurrentUserId;

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
        public IActionResult Search(string searchTerm)
        {
            int uid = CurrentUserId;
            searchTerm ??= "";

            List<UserSummaryDTO> friends = _socialService.GetFriends(uid)
                .Where(u => u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            List<UserSummaryDTO> incoming = _socialService.GetIncomingFriendRequests(uid)
                .Where(u => u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            List<UserSummaryDTO> outgoing = _socialService.GetOutgoingFriendRequests(uid)
                .Where(u => u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            List<UserSummaryDTO> nonFriends = _socialService.GetNonFriends(uid)
                .Where(u => u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var potential = nonFriends
                .Concat(outgoing)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();

            SocialViewModel vm = new SocialViewModel
            {
                Friends = friends,
                IncomingRequests = incoming,
                PotentialFriends = potential,
                OutgoingRequestIds = outgoing.Select(x => x.Id).ToList()
            };

            return PartialView("_SearchResults", vm);
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

