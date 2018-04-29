using System.Threading.Tasks;

namespace CoralTime.BL.Interfaces
{
    public interface INotificationService
    {
        //Task ByMemberSettings();

        Task ByProjectSettingsAsync(string baseUrl);

        Task SendWeeklyTimeEntryUpdates();
        //Task CheckTasksAsync(string userName);
        //Task SendNotificationAsync(Member member, Project project);
    }
}
