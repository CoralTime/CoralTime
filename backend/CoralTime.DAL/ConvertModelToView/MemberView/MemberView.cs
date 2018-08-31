using System.Linq;
using AutoMapper;
using CoralTime.DAL.Models.Member;
using MemberView = CoralTime.ViewModels.Member.MemberView;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static MemberView GetView(this Member member, IMapper mapper, string urlIcon)
        {
            var memberView = mapper.Map<Member, MemberView>(member);

            memberView.UrlIcon = urlIcon;
            
            return memberView;
        }

        public static MemberView GetViewWithProjectCount(this Member member, IMapper mapper, string urlIcon)
        {
            var memberView = member.GetView(mapper, urlIcon);

            memberView.ProjectsCount = member.MemberProjectRoles?.Select(x => x.Project).Count();

            return memberView;
        }

        public static MemberView GetViewWithGlobalProjectsCount(this Member member, int[] globalActiveProjIds, IMapper mapper, string urlIcon)
        {
            var memberViewWithGlobalProjectsCount = member.GetView(mapper, urlIcon);

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