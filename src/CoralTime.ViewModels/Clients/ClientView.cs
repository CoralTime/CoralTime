using System.ComponentModel.DataAnnotations;

namespace CoralTime.ViewModels.Clients
{
    public class ClientView 
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string Email { get; set; }

        public int ProjectsCount { get; set; }
    }
}