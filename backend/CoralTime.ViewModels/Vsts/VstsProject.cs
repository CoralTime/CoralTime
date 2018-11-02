using System.Collections.Generic;

namespace CoralTime.ViewModels.Vsts
{
    public class VstsProject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }

        public int Revision { get; set; }

        public string Visibility { get; set; }
    }

    public class VstsProjectList
    {
        public int Count { get; set; }

        public List<VstsProject> Value { get; set; }
    }
}