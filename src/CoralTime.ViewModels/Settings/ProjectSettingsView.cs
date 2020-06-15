using System.ComponentModel.DataAnnotations;

namespace CoralTime.ViewModels.Settings
{ 
    public class SettingsView
    {
        [Key]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
