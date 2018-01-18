using CoralTime.DAL.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTime.DAL.Models
{
    public class ReportsSettings : LogChanges, IReportsSettingsSaveToDb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int MemberId { get; set; }

        [ForeignKey("MemberId")]
        public Member Member { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public int[] ProjectIds { get; set; }

        public int[] MemberIds { get; set; }

        public int?[] ClientIds { get; set; }

        public int? GroupById { get; set; }

        public int[] ShowColumnIds { get; set; }
    }
}