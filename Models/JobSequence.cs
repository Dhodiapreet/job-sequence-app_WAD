using System.ComponentModel.DataAnnotations;

namespace JobSequenceApp.Models
{
    public class JobSequence
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WorkerProfileId { get; set; }
        public WorkerProfile? WorkerProfile { get; set; }

        [Required]
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed
    }
}
