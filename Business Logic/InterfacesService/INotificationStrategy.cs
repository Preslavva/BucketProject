namespace BucketProject.BLL.Business_Logic.InterfacesService
{
    public interface INotificationStrategy
    {
        bool ShouldNotify(DateTime today, DateTime deadline);
        string GetNotificationMessage(string goalTitle, DateTime deadline);
    }
}
