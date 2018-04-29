namespace CoralTime.ViewModels.Notifications
{
    public class ProjectsWithDatesEditing
    {
        public ProjectWithNotifications ProjectWithDatesEditing { get; set; }

        public ProjectEditionDays EditionDays { get; set; }

        public ProjectsWithDatesEditing()
        {
            ProjectWithDatesEditing = new ProjectWithNotifications();
            EditionDays = new ProjectEditionDays();
        }
    }
}
