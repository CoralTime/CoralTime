using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTime.DAL.Models.Vsts
{
    public class VstsProjectUser : LogChanges.LogChanges
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int VstsUserId { get; set; }

        [ForeignKey("VstsUserId")]
        public virtual VstsUser VstsUser { get; set; }

        public int VstsProjectId { get; set; }

        [ForeignKey("VstsProjectId")]
        public VstsProject VstsProject { get; set; }
    }
}