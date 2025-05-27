using BucketProject.DAL.Models.Enums;

namespace BucketProject.BLL.Business_Logic.InterfacesService
{
    public interface IAIClient
    {
        Task<List<string>> BreakDownTextIntoGoalsAsync(string description, Category category);
    }
}
