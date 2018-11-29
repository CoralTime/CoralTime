using System.ComponentModel.DataAnnotations;

namespace CoralTime.ViewModels.Vsts
{
    public class VstsMemberView
    {
        [Key]
        public int Id { get; set; }

        public string FullName { get; set; }

        public int MemberId { get; set; }

        public string UrlIcon { get; set; }
    }
}