using AutoMapper;
using CoralTime.DAL.Models.Member;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static ViewModels.MemberProjectRoles.MemberProjectRoleView GetView(this MemberProjectRole memberProjRole, IMapper mapper, string urlIcon)
        {
            var memberProjRoleView = mapper.Map<MemberProjectRole, ViewModels.MemberProjectRoles.MemberProjectRoleView>(memberProjRole);

            memberProjRoleView.UrlIcon = urlIcon;

            return memberProjRoleView;
        }

        public static ViewModels.MemberProjectRoles.MemberProjectRoleView GetViewWithGlobalProjects(this MemberProjectRole memberProjRole, IMapper mapper, string urlIcon)
        {
            var memberProjRoleView = memberProjRole.GetView(mapper, urlIcon);

            memberProjRoleView.Id = 0;

            return memberProjRoleView;
        }
    }
}