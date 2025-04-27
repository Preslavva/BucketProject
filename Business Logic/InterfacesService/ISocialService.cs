using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.BLLBusiness_Logic.Domain;

namespace BucketProjetc.BLL.Business_Logic.InterfacesService
{
    public interface ISocialService
    {
        List<UserSummaryDTO> GetFriends(int userId);
        List<UserSummaryDTO> GetNonFriends(int userId);
        bool AddFriend(int userId, int friendId);
        bool RemoveFriend(int userId, int friendId);


    }
}
