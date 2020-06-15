using Microsoft.AspNetCore.Identity;

namespace CoralTime.DAL.Models
{
    // Add profile data for application Members by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public bool IsAdmin { get; set; }

        public bool IsManager { get; set; }

        public bool IsActive { get; set; }
    }
}