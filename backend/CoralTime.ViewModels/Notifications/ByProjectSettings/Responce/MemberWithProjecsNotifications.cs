using System.Collections.Generic;
using CoralTime.ViewModels.Notifications.ByProjectSettings.Responce.MemberWithProjectsLight;

namespace CoralTime.ViewModels.Notifications.ByProjectSettings.Responce
{
    public class MemberWithProjecsNotificationsView
    {
        public MemberLightView MemberLight { get; set; }

        public List<ProjectsWithDatesEditing> ProjectsWithDatesEditing { get; set; }

        public MemberWithProjecsNotificationsView()
        {
            ProjectsWithDatesEditing = new List<ProjectsWithDatesEditing>();
        }
    }
}
