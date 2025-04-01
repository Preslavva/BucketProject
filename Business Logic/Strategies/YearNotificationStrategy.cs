using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.BLL.Business_Logic.InterfacesService;

namespace BucketProject.BLL.Business_Logic.Strategies
{
    public class YearNotificationStrategy : INotificationStrategy
    {
        public bool ShouldNotify(DateTime today, DateTime deadline)
        {
            int daysLeft = (int)(deadline.Date - today.Date).TotalDays;
            return daysLeft <= 30 && daysLeft > 0;
        }

        public string GetNotificationMessage(string goalTitle, DateTime deadline)
        {
            int daysLeft = (int)(deadline.Date - DateTime.Today.Date).TotalDays;

            if (daysLeft > 1)
            {
                return $"Only {daysLeft} days left to achieve your yearly goal \"{goalTitle}\"!";
            }
            else if (daysLeft == 1)
            {
                return $"Today is the last day for your yearly goal \"{goalTitle}\"!";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

