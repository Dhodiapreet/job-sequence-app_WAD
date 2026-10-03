using System.ComponentModel.DataAnnotations;

namespace JobSequenceApp.Models
{
    public class CustomerProfile
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }

        public string Address { get; set; } = string.Empty;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
