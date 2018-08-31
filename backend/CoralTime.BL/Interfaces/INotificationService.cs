using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoralTime.ViewModels.Notifications.ByProjectSettings.Request.MemberWithProjectsLightIds;
using CoralTime.ViewModels.Notifications.ByProjectSettings.Responce.MemberWithProjectsLight;

namespace CoralTime.BL.Interfaces
{
    public interface INotificationService
    {
        Task ByProjectSettingsAsync(string baseUrl);

        Task SendWeeklyTimeEntryUpdatesAsync(string baseUrl);

        List<MemberWithProjectsLightView> GetMembersWithProjectsNotification(List<MemberWithProjectsIdsView> memberWithProjectsIds = null);

//        Task SendToMemberNotificationsByProjectsSettingsAsync(DateTime todayDate, string baseUrl, List<MemberWithProjectsIdsView> memberWithProjectsIds = null);
//
//        Task SendWeeklyNotificationsForMembers(string baseUrl, int[] membersIds = null);
    }
}
