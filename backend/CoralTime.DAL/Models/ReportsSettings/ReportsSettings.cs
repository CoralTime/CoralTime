using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTime.DAL.Models
{
    public class ReportsSettings : LogChanges, IReportsSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string QueryName { get; set; }

        public int MemberId { get; set; }

        [ForeignKey("MemberId")]
        public Member Member { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public int? DateStaticId { get; set; }

        public int? GroupById { get; set; }

        public string FilterClientIds { get; set; }

        public string FilterProjectIds { get; set; }

        public string FilterMemberIds { get; set; }

        public string FilterShowColumnIds { get; set; }

        public bool IsCurrentQuery { get; set; }
    }
}