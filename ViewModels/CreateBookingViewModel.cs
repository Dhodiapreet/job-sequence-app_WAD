using System.ComponentModel.DataAnnotations;

namespace JobSequenceApp.ViewModels
{
    public class CreateBookingViewModel
    {
        [Required]
        public int WorkerProfileId { get; set; }

        public string WorkerName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Job Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Service { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
    }
}
