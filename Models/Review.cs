using System.ComponentModel.DataAnnotations;

namespace JobSequenceApp.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerProfileId { get; set; }
        public CustomerProfile? CustomerProfile { get; set; }

        [Required]
        public int WorkerProfileId { get; set; }
        public WorkerProfile? WorkerProfile { get; set; }

        [Required]
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
