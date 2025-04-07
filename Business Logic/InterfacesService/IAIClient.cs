using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.DAL.Models.Enums;

namespace BucketProjetc.BLL.Business_Logic.InterfacesService
{
    public interface IAIClient
    {
        Task<List<string>> BreakDownTextIntoGoalsAsync(string description, Category category);
    }
}
