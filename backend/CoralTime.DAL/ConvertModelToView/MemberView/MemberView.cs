using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Member;
using System.Linq;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static MemberView GetView(this Member member, IMapper _mapper)
        {
            var memberView = _mapper.Map<Member, MemberView>(member);

            memberView.ProjectsCount = member.MemberProjectRoles?.Select(x => x.Project).Count();

            return memberView;
        }

        public static MemberView GetViewWithGlobalProjectsCount(this Member member, int[] globalActiveProjIds, IMapper _mapper)
        {
            var memberViewWithGlobalProjectsCount = member.GetView(_mapper);

            var countProjects = globalActiveProjIds.Length;
            var allMemberProjectRole = member.MemberProjectRoles.Where(z => z.Project != null && z.Project.IsActive);

            foreach (var memberProjRole in allMemberProjectRole)
            {
                // If memberProjRole with MemberId and ProjectId not exist at result with global projs -> add this custom project.
                // Not add if result has same name global and custom project name.
                var isNotAddedBeforeCustomProjects = !globalActiveProjIds.Contains(memberProjRole.ProjectId);

                if (isNotAddedBeforeCustomProjects)
                {
                    ++countProjects;
                }
            }

            memberViewWithGlobalProjectsCount.ProjectsCount = countProjects;

            return memberViewWithGlobalProjectsCount;
        }
    }
}