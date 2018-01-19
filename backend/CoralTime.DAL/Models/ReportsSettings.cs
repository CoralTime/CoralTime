using CoralTime.Common.Models.Reports;
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

        public int MemberId { get; set; }

        [ForeignKey("MemberId")]
        public Member Member { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public int? GroupById { get; set; }

        public string ClientIds { get; set; }

        public string ProjectIds { get; set; }

        public string MemberIds { get; set; }

        public string ShowColumnIds { get; set; }
    }
}