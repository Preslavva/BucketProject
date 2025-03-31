using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.BLL.Business_Logic.InterfacesService;

namespace BucketProject.BLL.Business_Logic.Strategies
{
    public class MonthNotificationStrategy:INotificationStrategy
    {
        private int _daysLeft;

        public bool ShouldNotify(DateTime today, DateTime deadline)
        {
            _daysLeft = (int)(deadline.Date - today.Date).TotalDays;
            return _daysLeft <= 7 && _daysLeft > 0;
        }

        public string GetNotificationMessage(string goalTitle)
        {
            return _daysLeft switch
            {
                > 1 => $"You have {_daysLeft} days left to complete your monthly goal \"{goalTitle}\"",
                1 => $"Today is the last day for your monthly goal \"{goalTitle}\"",
                _ => string.Empty
            };
        }
    }
}

