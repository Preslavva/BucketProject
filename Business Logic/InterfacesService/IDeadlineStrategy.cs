namespace BucketProject.BLL.Business_Logic.InterfacesService
{
    public interface IDeadlineStrategy
    {
        DateTime? GetDeadline(DateTime createdAt, bool isPostponed);
    }
}
