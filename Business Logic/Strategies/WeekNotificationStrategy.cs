using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.BLL.Business_Logic.InterfacesService;

namespace BucketProject.BLL.Business_Logic.Strategies
{
    public class WeekNotificationStrategy: INotificationStrategy
    {
        public bool ShouldNotify(DateTime today, DateTime deadline)
        {
            return (deadline - today).TotalDays == 0;
        }

        public string GetNotificationMessage(string goalTitle)
        {
            return $"This is your last day to achieve your weekly goal \"{goalTitle}\"";
        }
    }
}
