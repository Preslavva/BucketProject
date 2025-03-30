using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.BLL.Business_Logic.InterfacesService;

namespace BucketProject.BLL.Business_Logic.Strategies
{
    public class YearDeadlineStrategy: IDeadlineStrategy
    {
        public DateTime? GetDeadline(DateTime createdAt, bool isPostponed)
        {
            if (!isPostponed)
            {
                return new DateTime(createdAt.Year + 1, 1, 1);

            }
            else
            {
                return new DateTime(createdAt.Year + 2, 1, 1);

            }
        }
    }
}
