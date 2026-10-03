using JobSequenceApp.Models;

namespace JobSequenceApp.ViewModels
{
    public class WorkerDashboardViewModel
    {
        public string WelcomeMessage { get; set; } = string.Empty;
        public List<JobSequence> TodaysJobs { get; set; } = new();
        public List<JobSequence> UpcomingJobs { get; set; } = new();
        public List<JobSequence> CompletedJobs { get; set; } = new();
        public JobSequence? ActiveJob { get; set; }
        public bool IsVerified { get; set; }
    }
}
