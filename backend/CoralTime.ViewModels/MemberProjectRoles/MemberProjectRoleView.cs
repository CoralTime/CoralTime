using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CoralTime.ViewModels.Projects;
using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Interfaces;

namespace CoralTime.ViewModels.MemberProjectRoles
{
    public class MemberProjectRoleView : IMemberImageIconView
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public bool IsProjectActive { get; set; }

        public bool IsProjectPrivate { get; set; }


        public int MemberId { get; set; }

        public string UrlIcon { get; set; }

        public string MemberName { get; set; }

        public string MemberEmail { get; set; }

        public string MemberUserName { get; set; }

        public bool IsMemberActive { get; set; }


        public int RoleId { get; set; }

        public string RoleName { get; set; }


        public IEnumerable<MemberView> Members { get; set; }

        public IEnumerable<ProjectView> Projects { get; }
    }
}