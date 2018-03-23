using CoralTime.DAL.Models.TimeValues;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTime.DAL.Models
{
    public class TimeEntry : LogChanges, ITimeValues
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        public int MemberId { get; set; }

        [ForeignKey("MemberId")]
        public Member Member { get; set; }

        public int TaskTypesId { get; set; }

        [Required]
        [ForeignKey("TaskTypesId")]
        public TaskType TaskType { set; get; }

        public DateTime Date { get; set; }

        #region Time values.

        public int TimeActual { get; set; }

        public int TimeEstimated { get; set; }

        [Required]

        public int TimeFrom { get; set; }

        public int TimeTo { get; set; }

        public int TimeTimerStart { get; set; } // It's time in seconds after 00.00, that display time when Timer is run.

        #endregion

        [MaxLength(1000)]
        public string Description { get; set; }

        public bool IsFromToShow { get; set; }
    }
}