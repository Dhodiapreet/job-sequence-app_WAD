using JobSequenceApp.Data;
using JobSequenceApp.Models;
using JobSequenceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JobSequenceApp.Areas.Worker.Controllers
{
    [Area("Worker")]
    [Authorize(Roles = "Worker")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return NotFound();

            var worker = await _context.WorkerProfiles
                .Include(w => w.ApplicationUser)
                .FirstOrDefaultAsync(w => w.ApplicationUserId == userId);
            
            if (worker == null) return NotFound("Worker profile not found.");

            var today = DateTime.Today;
            var jobs = await _context.JobSequences
                .Include(j => j.Booking)
                .Where(j => j.WorkerProfileId == worker.Id)
                .OrderBy(j => j.StartTime)
                .ToListAsync();

            var vm = new WorkerDashboardViewModel
            {
                WelcomeMessage = $"Hello, {worker.ApplicationUser?.FirstName}!",
                IsVerified = worker.IsVerified,
                TodaysJobs = jobs.Where(j => j.StartTime.Date == today && j.Status != "Completed").ToList(),
                UpcomingJobs = jobs.Where(j => j.StartTime.Date > today).ToList(),
                CompletedJobs = jobs.Where(j => j.Status == "Completed").Take(5).ToList(),
                ActiveJob = jobs.FirstOrDefault(j => j.Status == "InProgress")
            };

            return View(vm);
        }
    }
}
