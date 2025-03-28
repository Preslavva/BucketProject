using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLL.Business_Logic.Strategies;
using BucketProject.DAL.Models.Enums;

namespace BucketProject.BLL.Business_Logic.Strategies
{
    public static class DeadlineStrategyManager
    {
        public static IDeadlineStrategy? GetStrategy(Category category)
        {
            return category switch
            {
                Category.Week => new WeekDeadlineStrategy(),
                Category.Month => new MonthDeadlineStrategy(),
                Category.Year => new YearDeadlineStrategy(),
                _ => null
            };
        }
    }
}
