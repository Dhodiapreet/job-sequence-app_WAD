using System.ComponentModel.DataAnnotations;

namespace JobSequenceApp.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerProfileId { get; set; }
        public CustomerProfile? CustomerProfile { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Service { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public DateTime RequestedDate { get; set; }
        public DateTime ExpectedStartTime { get; set; }
        public DateTime ExpectedEndTime { get; set; }

        public string Status { get; set; } = "REQUESTED"; // REQUESTED, ACCEPTED, DECLINED, IN_PROGRESS, COMPLETED, CANCELLED

        public int WorkerProfileId { get; set; }
        public WorkerProfile? WorkerProfile { get; set; }
    }
}
