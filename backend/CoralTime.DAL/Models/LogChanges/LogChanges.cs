using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTime.DAL.Models
{
    public class LogChanges : ILogChanges
    {
        public DateTime CreationDate { get; set; }

        public string CreatorId { get; set; }

        [ForeignKey("CreatorId")]
        public ApplicationUser Creator { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public string LastEditorUserId { get; set; }

        [ForeignKey("LastEditorUserId")]
        public ApplicationUser LastEditor { get; set; }
    }
}