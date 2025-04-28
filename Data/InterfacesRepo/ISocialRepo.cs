using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.DAL.Models.Entities;

namespace BucketProject.DAL.Data.InterfacesRepo;

    public interface ISocialRepo
    {
    List<UserEntity> LoadFriends(int userId);
    List<UserEntity> LoadNonFriends(int userId);
    bool SendFriendRequest(int userId, int friendId);
    bool TryRemoveFriend(int userId, int friendId);
    List<UserEntity> LoadIncomingRequests(int userId);
    bool RespondToFriendRequest(int userId, int requesterId, bool accept);
    List<UserEntity> LoadOutgoingRequests(int userId);

    }

