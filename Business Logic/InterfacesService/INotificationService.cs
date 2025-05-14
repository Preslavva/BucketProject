using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.BLL.Business_Logic.Domain;

namespace BucketProject.BLL.Business_Logic.InterfacesService
{
    public interface INotificationService
    {
        List<Goal> CheckAndNotify(DateTime today);
        List<Goal> GetSharedCompletionGoals();
        List<Goal> GetSharedDeletedGoals();
        List<Goal> GetSharedPostponedGoals();
        void DismissNotification(int userId, int goalId, string type, int triggeredByUserId);

    }
}
