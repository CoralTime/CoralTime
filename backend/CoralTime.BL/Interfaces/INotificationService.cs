using System.Threading.Tasks;

namespace CoralTime.BL.Interfaces
{
    public interface INotificationService
    {
        Task ByProjectSettingsAsync(string baseUrl);

        Task SendWeeklyTimeEntryUpdatesAsync(string baseUrl);
    }
}
