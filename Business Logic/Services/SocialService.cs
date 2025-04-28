using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProjetc.BLL.Business_Logic.InterfacesService;

using Microsoft.AspNetCore.Http;
using BucketProject.BLLBusiness_Logic.Domain;
using BucketProject.DAL.Models.Entities;


namespace BucketProject.BLL.Business_Logic.Services
{
    public class SocialService: ISocialService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ISocialRepo _socialRepo;
        private readonly IGoalRepo _goalRepo;

        private readonly IMapper _mapper;


        public SocialService(ISocialRepo socialRepo, IHttpContextAccessor contextAccessor, IMapper mapper, IGoalRepo goalRepo)
        {
            _socialRepo = socialRepo;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _goalRepo = goalRepo;
        }

        public List<UserSummaryDTO> GetFriends(int userId)
        {
            var ents = _socialRepo.LoadFriends(userId);
            return _mapper.Map<List<UserSummaryDTO>>(ents);
        }

        public List<UserSummaryDTO> GetNonFriends(int userId)
        {
            var ents = _socialRepo.LoadNonFriends(userId);
            return _mapper.Map<List<UserSummaryDTO>>(ents);
        }

        public List<UserSummaryDTO> GetIncomingFriendRequests(int userId)
        {
            var ents = _socialRepo.LoadIncomingRequests(userId);
            return _mapper.Map<List<UserSummaryDTO>>(ents);
        }

        public bool SendFriendRequest(int userId, int friendId)
        {
            return _socialRepo.SendFriendRequest(userId, friendId);
        }

        public bool AcceptFriendRequest(int userId, int requesterId)
        {
            return _socialRepo.RespondToFriendRequest(userId, requesterId, accept: true);
        }

        public bool DeclineFriendRequest(int userId, int requesterId)
        {
            return _socialRepo.RespondToFriendRequest(userId, requesterId, accept: false);
        }

        public bool RemoveFriend(int userId, int friendId)
        {
            return _socialRepo.TryRemoveFriend(userId, friendId);
        }
        public List<UserSummaryDTO> GetOutgoingFriendRequests(int userId)
        {
            var ents = _socialRepo.LoadOutgoingRequests(userId);
            return _mapper.Map<List<UserSummaryDTO>>(ents);
        }




    }
}
