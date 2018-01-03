using System.ComponentModel.DataAnnotations;

namespace CoralTime.ViewModels.Profiles
{
    public class ProfileProjectView
    {
        public string Name { get; set; }

        [Key]
        public int Id { get; set; }

        public int MemberCount { get; set; }

        public string[] ManagersNames { get; set; }

        public bool IsPrivate { get; set; }

        public int Color { get; set; }

        public bool IsPrimary { get; set; }
    }
}
