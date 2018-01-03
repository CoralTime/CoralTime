using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTime.DAL.Models
{
    public interface ILogChanges
    {
        DateTime CreationDate { get; set; }

        string CreatorId { get; set; }

        [ForeignKey("CreatorId")]
        ApplicationUser Creator { get; set; }

        DateTime LastUpdateDate { get; set; }

        string LastEditorUserId { get; set; }

        [ForeignKey("LastEditorUserId")]
        ApplicationUser LastEditor { get; set; }
    }
}