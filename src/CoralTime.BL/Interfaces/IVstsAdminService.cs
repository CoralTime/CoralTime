namespace CoralTime.BL.Interfaces
{
    public interface IVstsAdminService
    {
        void UpdateVstsProjects();

        void UpdateVstsUsers();

        bool UpdateVstsUsersByProject(int projectId);

        bool UpdateVstsProject(int projectId);
    }
}