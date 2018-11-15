using System.Collections.Generic;

namespace CoralTime.ViewModels.Vsts
{
    public class Avatar
    {
        public string Href { get; set; }
    }

    public class Links
    {
        public Avatar Avatar { get; set; }
    }

    public class Identity
    {
        public string DisplayName { get; set; }

        public string Url { get; set; }

        public Links _links { get; set; }

        public string Id { get; set; }

        public string UniqueName { get; set; }

        public string ImageUrl { get; set; }

        public string Descriptor { get; set; }
    }

    public class VstsMember
    {
        public Identity Identity { get; set; }

        public bool? IsTeamAdmin { get; set; }
    }

    public class VstsMemberList
    {
        public List<VstsMember> Value { get; set; }

        public int Count { get; set; }
    }
}