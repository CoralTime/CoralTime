using System.ComponentModel.DataAnnotations;

namespace CoralTime.ViewModels.Member
{
    public class MemberChangePasswordView
    {

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        [Key]
        public int Id { get; set; }
    }
}
