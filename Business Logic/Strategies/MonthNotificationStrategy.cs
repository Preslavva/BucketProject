using BucketProject.BLL.Business_Logic.InterfacesService;

namespace BucketProject.BLL.Business_Logic.Strategies
{
    public class MonthNotificationStrategy : INotificationStrategy
    {
        public bool ShouldNotify(DateTime today, DateTime deadline)
        {
            int daysLeft = (int)(deadline.Date - today.Date).TotalDays;
            return daysLeft <= 7 && daysLeft > 0;
        }

        public string GetNotificationMessage(string goalTitle, DateTime deadline)
        {
            int daysLeft = (int)(deadline.Date - DateTime.Today.Date).TotalDays;

            if (daysLeft > 1)
            {
                return $"You have {daysLeft} days left to complete your monthly goal \"{goalTitle}\".";
            }
            else if (daysLeft == 1)
            {
                return $"This is your last day to achieve your monthly goal \"{goalTitle}\"!";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

