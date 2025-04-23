using BucketProject.DAL.Models.Enums;

namespace BucketProjetc.BLL.Business_Logic.InterfacesService
{
    public interface IAIClient
    {
        Task<List<string>> BreakDownTextIntoGoalsAsync(string description, Category category);
    }
}
