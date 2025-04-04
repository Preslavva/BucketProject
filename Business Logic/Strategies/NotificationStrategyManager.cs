using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.DAL.Models.Enums;
using BucketProject.BLL.Business_Logic.Strategies;

namespace BucketProject.BLL.Business_Logic.Strategies
{
    public class NotificationStrategyManager
    {
        public static INotificationStrategy? GetStrategy(Category category)
        {
            return category switch
            {
                Category.Week => new WeekNotificationStrategy(),
                Category.Month => new MonthNotificationStrategy(),
                Category.Year => new YearNotificationStrategy(),
                _ => null
            };
        }

    }
}
