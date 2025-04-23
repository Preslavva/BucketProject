using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.DAL.Models.Enums;

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
