using System;
using System.ComponentModel.DataAnnotations;


namespace CoralTime.ViewModels.MemberActions
{
    public class MemberActionView
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime Date { get; set; } 
        
        public string ChangedObject { get; set; }
        
        public string Entity  { get; set; }
        
        public string ChangedFields { get; set; }
        
        public string Action { get; set; }
        
        public string EntityId { get; set; }
        
        public int MemberId { get; set; }

        public string MemberFullName { get; set; }
    }
}