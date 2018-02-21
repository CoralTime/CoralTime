using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTime.DAL.Models
{
    public class TaskType : LogChanges, IInitializeByName
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public int? ProjectId { get; set; }

        public bool IsActive { get; set; }

        public int Color { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
    }
}