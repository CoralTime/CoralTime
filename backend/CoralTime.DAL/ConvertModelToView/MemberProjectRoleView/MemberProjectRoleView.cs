using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.MemberProjectRoles;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static MemberProjectRoleView GetView(this MemberProjectRole memberProjRole, IMapper _mapper, string urlIcon)
        {
            var memberProjRoleView = _mapper.Map<MemberProjectRole, MemberProjectRoleView>(memberProjRole);

            memberProjRoleView.UrlIcon = urlIcon;

            return memberProjRoleView;
        }

        public static MemberProjectRoleView GetViewWithGlobalProjects(this MemberProjectRole memberProjRole, IMapper _mapper, string urlIcon)
        {
            var memberProjRoleView = memberProjRole.GetView(_mapper, urlIcon);

            memberProjRoleView.Id = 0;

            return memberProjRoleView;
        }
    }
}