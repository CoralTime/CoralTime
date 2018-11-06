using System.Collections.Generic;

namespace CoralTime.ViewModels.Vsts
{
    public class VstsSetup
    {
        public int? ProjectId { get; set; }

        public string VstsProjectId { get; set; }

        public int? MemberId { get; set; }

        public string VstsUserId { get; set; }

        public int? VstsUserName { get; set; }

        public List<string> Errors { get; set; }
    }
}