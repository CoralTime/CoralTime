using System.ComponentModel.DataAnnotations;

namespace CoralTime.DAL.Models
{
    public class Setting
    {
        [Key]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}