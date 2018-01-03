using System.ComponentModel.DataAnnotations;

namespace CoralTime.ViewModels.Errors
{
    public class ErrorView
    {
        [Key]
        public string Source { get; set; }

        public string Title { get; set; }

        public string Details { get; set; }
    }
}