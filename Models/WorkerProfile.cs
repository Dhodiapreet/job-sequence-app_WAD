using System.ComponentModel.DataAnnotations;

namespace JobSequenceApp.Models
{
    public class WorkerProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }

        public bool IsVerified { get; set; } = false;

        public string Profession { get; set; } = string.Empty;
        public string Skills { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public decimal Pricing { get; set; } = 0;
        public string Biography { get; set; } = string.Empty;
        public string ServiceArea { get; set; } = string.Empty;

        public ICollection<JobSequence> JobSequences { get; set; } = new List<JobSequence>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public double AverageRating { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;
    }
}
