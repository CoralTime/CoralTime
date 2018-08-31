using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoralTime.Common.Constants;

namespace CoralTime.DAL.Models.LogChanges
{
    public class MemberAction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public DateTime Date { get; set; } = DateTime.Now;
        
        public string ChangedObject { get; set; }
        
        public string Entity  { get; set; }
        
        public string ChangedFields { get; set; }
        
        public string Action { get; set; }
        
        public string EntityId { get; set; }
        
        public int MemberId { get; set; }
        
        [ForeignKey("MemberId")]
        public Member.Member Member { get; set; }
    }
}