namespace JobSequenceApp.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalWorkers { get; set; }
        public int ActiveWorkers { get; set; }
        public int PendingWorkerVerifications { get; set; }
        public int TotalBookings { get; set; }
        public int ActiveBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int ActiveJobs { get; set; }
    }
}
