using JobSequenceApp.Data;
using JobSequenceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSequenceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalCustomers = await _context.CustomerProfiles.CountAsync(),
                TotalWorkers = await _context.WorkerProfiles.CountAsync(),
                ActiveWorkers = await _context.WorkerProfiles.CountAsync(w => w.IsVerified),
                PendingWorkerVerifications = await _context.WorkerProfiles.CountAsync(w => !w.IsVerified),
                TotalBookings = await _context.Bookings.CountAsync(),
                ActiveBookings = await _context.Bookings.CountAsync(b => b.Status == "ACCEPTED" || b.Status == "IN_PROGRESS"),
                CompletedBookings = await _context.Bookings.CountAsync(b => b.Status == "COMPLETED"),
                CancelledBookings = await _context.Bookings.CountAsync(b => b.Status == "CANCELLED"),
                ActiveJobs = await _context.JobSequences.CountAsync(j => j.Status == "InProgress")
            };

            return View(vm);
        }
    }
}
