using Microsoft.AspNetCore.Identity;

namespace JobSequenceApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Navigation properties
        public CustomerProfile? CustomerProfile { get; set; }
        public WorkerProfile? WorkerProfile { get; set; }
    }
}
