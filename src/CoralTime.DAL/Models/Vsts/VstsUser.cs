using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTime.DAL.Models.Vsts
{
    public class VstsUser : LogChanges.LogChanges
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int MemberId { get; set; }

        [ForeignKey("MemberId")]
        public Member.Member Member { get; set; }

        public string VstsUserId { get; set; }

        public string VstsUserName { get; set; }

        public List<VstsProjectUser> VstsProjectUsers { get; set; }
    }
}