using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoralTime.DAL.Models
{
    public class UserForgotPassRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime DateFrom { get; set; }

        /// <summary>Gets or sets the DateTo field. </summary>

        [Required]
        public DateTime DateTo { get; set; }

        /// <summary>Gets or sets the Email field. </summary>

        [MaxLength(255)]
        [Required]
        public string Email { get; set; }

        /// <summary>Gets or sets the UserForgotPassRequestUid field. </summary>

        [Required]
        public Guid UserForgotPassRequestUid { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}