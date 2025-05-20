using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProjetc.BLL.Business_Logic.InterfacesService;

using Microsoft.AspNetCore.Http;
using BucketProject.BLL.Business_Logic.DTOs;
using BucketProject.DAL.Models.Entities;


namespace BucketProject.BLL.Business_Logic.Services
{
    public class SocialService: ISocialService
    {
        private readonly ISocialRepo _socialRepo;
        private readonly IMapper _mapper;


        public SocialService(ISocialRepo socialRepo, IMapper mapper)
        {
            _socialRepo = socialRepo;
            _mapper = mapper;
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
        public string GetUsername(int userId)
        {
            UserEntity user = _socialRepo.GetUserById(userId);
            return user?.Username ?? "(Unknown User)";
        }




    }
}
