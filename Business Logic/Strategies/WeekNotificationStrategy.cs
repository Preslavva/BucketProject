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
                return today.Date == deadline.Date.AddDays(-2) || today.Date == deadline.Date.AddDays(-1);
            }

            public string GetNotificationMessage(string goalTitle, DateTime deadline)
            {
                DateTime today = DateTime.Today;

            if (today == deadline.Date.AddDays(-2))
            {
                return $"Only 2 days left to achieve your weekly goal \"{goalTitle}\". Keep going!";
            }
            else if (today == deadline.Date.AddDays(-1))
            {
                return $"This is your last day to achieve your weekly goal \"{goalTitle}\"";
            }
            else
            {
                return string.Empty;
            }
        }
        }

    }

