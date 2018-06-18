using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTime.DAL.Models.Member
{
    public class MemberImage : LogChanges.LogChanges
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int MemberId { get; set; }

        [ForeignKey("MemberId")]
        public Member Member { get; set; }

        [MaxLength(200)]
        public string FileNameImage { get; set; }

        public byte[] ByteArrayAvatar { get; set; }

        public byte[] ByteArrayIcon { get; set; }
    }
}