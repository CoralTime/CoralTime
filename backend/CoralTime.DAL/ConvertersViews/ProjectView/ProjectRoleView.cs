using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.ProjectRole;

namespace CoralTime.DAL.ConvertersViews.ExstensionsMethods
{
    public static partial class ExstensionsMethods
    {
        public static ProjectRoleView GetView(this ProjectRole projectRole, IMapper _mapper)
        {
            var projRoleView = _mapper.Map<ProjectRole, ProjectRoleView>(projectRole);

            return projRoleView;
        }
    }
}
