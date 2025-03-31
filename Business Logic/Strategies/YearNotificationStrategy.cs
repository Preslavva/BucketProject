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
        private int _daysLeft;

        public bool ShouldNotify(DateTime today, DateTime deadline)
        {
            _daysLeft = (int)(deadline.Date - today.Date).TotalDays;
            return _daysLeft <= 30 && _daysLeft >= 0;
        }

        public string GetNotificationMessage(string goalTitle)
        {
            return _daysLeft switch
            {
                > 1 => $"Only {_daysLeft} days left to achieve your yearly goal \"{goalTitle}\"",
                1 => $"Today is the last day for your yearly goal \"{goalTitle}\"",
                _ => string.Empty
            };
        }
    }
}
