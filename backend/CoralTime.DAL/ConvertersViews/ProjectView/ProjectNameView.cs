using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Projects;

namespace CoralTime.DAL.ConvertersViews.ExstensionsMethods
{
    public static partial class ExstensionsMethods
    {
        public static ProjectNameView GetViewProjectName(this Project project, IMapper _mapper)
        {
            var projectNameView = _mapper.Map<Project, ProjectNameView>(project);
            return projectNameView;
        }
    }
}
