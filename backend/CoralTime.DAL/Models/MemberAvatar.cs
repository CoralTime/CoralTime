using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTime.DAL.Models
{
    public class MemberAvatar : LogChanges
    {
        public byte[] AvatarFile { get; set; }

        public byte[] ThumbnailFile { get; set; }

        [MaxLength(200)]
        public string AvatarFileName { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int MemberId { get; set; }

        [ForeignKey("MemberId")]
        public Member Member { get; set; }
    }
}