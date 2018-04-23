using System.Threading.Tasks;

namespace CoralTime.BL.Interfaces
{
    public interface INotificationService
    {
        //Task ByMemberSettings();

        Task ByProjectSettings(string baseUrl);
        
        //Task CheckTasksAsync(string userName);
        //Task SendNotificationAsync(Member member, Project project);
    }
}
