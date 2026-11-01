using JobSequenceApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSequenceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalCustomers = await _context.CustomerProfiles.CountAsync();
            ViewBag.TotalWorkers = await _context.WorkerProfiles.CountAsync();
            ViewBag.VerifiedWorkers = await _context.WorkerProfiles.CountAsync(w => w.IsVerified);
            ViewBag.PendingWorkers = await _context.WorkerProfiles.CountAsync(w => !w.IsVerified);
            ViewBag.TotalBookings = await _context.Bookings.CountAsync();
            ViewBag.CompletedBookings = await _context.Bookings.CountAsync(b => b.Status == "COMPLETED");
            ViewBag.CancelledBookings = await _context.Bookings.CountAsync(b => b.Status == "CANCELLED");
            ViewBag.TotalJobs = await _context.JobSequences.CountAsync();
            ViewBag.CompletedJobs = await _context.JobSequences.CountAsync(j => j.Status == "Completed");

            return View();
        }
    }
}