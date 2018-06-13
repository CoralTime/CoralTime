using AutoMapper;
using CoralTime.DAL.Models.Member;
using CoralTime.ViewModels.Member;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static ProjectMembersView GetViewProjectMembers(this Member member, IMapper _mapper, string urlIcon)
        {
            var projectMembersView = _mapper.Map<Member, ProjectMembersView>(member);

            projectMembersView.UrlIcon = urlIcon;

            return projectMembersView;
        }
    }
}
