using System.ComponentModel.DataAnnotations;

namespace CoralTime.ViewModels.Tasks
{
    public class TaskTypeView
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ProjectId { get; set; }

        public bool IsActive { get; set; }

        public int Color { get; set; }

        public string Description { get; set; }
    }
}