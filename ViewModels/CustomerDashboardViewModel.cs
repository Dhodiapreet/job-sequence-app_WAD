using JobSequenceApp.Models;

namespace JobSequenceApp.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public string WelcomeMessage { get; set; } = string.Empty;
        public List<Booking> UpcomingBookings { get; set; } = new();
        public List<Booking> ActiveBookings { get; set; } = new();
        public List<Booking> CompletedBookings { get; set; } = new();
    }
}
