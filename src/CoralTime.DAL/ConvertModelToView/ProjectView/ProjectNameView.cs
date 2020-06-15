using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Projects;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static ProjectNameView GetViewProjectName(this Project project, IMapper _mapper)
        {
            var projectNameView = _mapper.Map<Project, ProjectNameView>(project);
            return projectNameView;
        }
    }
}
