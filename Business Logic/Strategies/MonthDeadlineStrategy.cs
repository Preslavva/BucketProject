using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.BLL.Business_Logic.InterfacesService;

namespace BucketProject.BLL.Business_Logic.Strategies
{
    internal class MonthDeadlineStrategy: IDeadlineStrategy
    {
        public DateTime? GetDeadline(DateTime createdAt,bool isPostponed)
        {
            if (!isPostponed)
            {
                return new DateTime(createdAt.Year, createdAt.Month, 1).AddMonths(1);
            }
            else
            {
                return new DateTime(createdAt.Year, createdAt.Month, 1).AddMonths(2);
            }
        }
    }
}
