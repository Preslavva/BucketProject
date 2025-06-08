using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.BLL.Business_Logic.DTOs;

namespace BucketProjetc.BLL.Business_Logic.InterfacesService
{
    public interface ISocialService
    {
        List<UserSummaryDTO> GetFriends(int userId);
        List<UserSummaryDTO> GetNonFriends(int userId, string searchTerm, int page, int pageSize);
        List<UserSummaryDTO> GetIncomingFriendRequests(int userId);

        bool SendFriendRequest(int userId, int friendId);
        bool AcceptFriendRequest(int userId, int requesterId);
        bool DeclineFriendRequest(int userId, int requesterId);
        bool RemoveFriend(int userId, int friendId);
        List<UserSummaryDTO> GetOutgoingFriendRequests(int userId);
        string GetUsername(int userId);
        int CountNonFriends(int userId, string searchTerm);


    }
}
