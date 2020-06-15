using System.Collections.Generic;

namespace CoralTime.ViewModels.Vsts
{
    public class VstsTeam
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public string IdentityUrl { get; set; }

        public string ProjectName { get; set; }

        public string ProjectId { get; set; }
    }

    public class VstsTeamList
    {
        public List<VstsTeam> Value { get; set; }

        public int Count { get; set; }
    }
}