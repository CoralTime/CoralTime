using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoralTime.ViewModels.Vsts
{
    public class VstsProjectIntegrationView
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string VstsProjectId { get; set; }

        public string VstsProjectName { get; set; }

        public string VstsCompanyUrl { get; set; }

        public string VstsPat { get; set; }

        public IEnumerable<VstsMemberView> Members { get; set; }

        public int MembersCount { get; set; }
    }
}