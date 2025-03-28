using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BucketProject.BLL.Business_Logic.InterfacesService
{
    public interface IDeadlineStrategy
    {
        DateTime? GetDeadline(DateTime createdAt);
    }
}
