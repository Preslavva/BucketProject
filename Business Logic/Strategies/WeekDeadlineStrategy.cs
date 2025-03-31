using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.BLL.Business_Logic.InterfacesService;

namespace BucketProject.BLL.Business_Logic.Strategies
{
    public class WeekDeadlineStrategy : IDeadlineStrategy
    {
        public DateTime? GetDeadline(DateTime createdAt, bool isPostponed)
        {
            int daysUntilNextMonday = ((int)DayOfWeek.Monday - (int)createdAt.DayOfWeek + 7) % 7;
            DateTime nextMonday = createdAt.Date.AddDays(daysUntilNextMonday).Date;

            if (!isPostponed)
            {
                return nextMonday;
            }
            else
            {
                return nextMonday.AddDays(7);
            }
        }
    }
}
