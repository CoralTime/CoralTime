using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTime.DAL.Models.Vsts
{
    public class VstsProject : LogChanges.LogChanges
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        public string VstsProjectId { get; set; }

        public string VstsProjectName { get; set; }

        public string VstsCompanyUrl { get; set; }

        public string VstsPat { get; set; }

        public List<VstsProjectUser> VstsProjectUsers { get; set; }
    }
}