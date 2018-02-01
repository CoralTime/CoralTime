using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.MemberProjectRoles;

namespace CoralTime.DAL.ConvertersOfViewModels
{
    public static partial class ConvertersOfViewModels
    {
        public static MemberProjectRoleView GetView(this MemberProjectRole memberProjRole, IMapper _mapper)
        {
            var memberProjRoleView = _mapper.Map<MemberProjectRole, MemberProjectRoleView>(memberProjRole);

            return memberProjRoleView;
        }

        public static MemberProjectRoleView GetViewWithGlobalProjects(this MemberProjectRole memberProjRole, IMapper _mapper)
        {
            var memberProjRoleView = memberProjRole.GetView(_mapper);

            memberProjRoleView.Id = 0;

            return memberProjRoleView;
        }
    }
}