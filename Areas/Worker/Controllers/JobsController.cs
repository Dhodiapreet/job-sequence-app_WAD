using System.Security.Claims;
using JobSequenceApp.Data;
using JobSequenceApp.Models;
using JobSequenceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSequenceApp.Areas.Worker.Controllers
{
    [Area("Worker")]
    [Authorize(Roles = "Worker")]
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IJobSchedulingService _jobSchedulingService;

        public JobsController(ApplicationDbContext context, IJobSchedulingService jobSchedulingService)
        {
            _context = context;
            _jobSchedulingService = jobSchedulingService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var worker = await _context.WorkerProfiles.FirstOrDefaultAsync(w => w.ApplicationUserId == userId);

            if (worker == null) return NotFound();

            var requests = await _context.Bookings
                .Include(b => b.CustomerProfile)
                .ThenInclude(c => c!.ApplicationUser)
                .Where(b => b.WorkerProfileId == worker.Id && b.Status == "REQUESTED")
                .OrderBy(b => b.ExpectedStartTime)
                .ToListAsync();

            return View(requests);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var worker = await _context.WorkerProfiles.FirstOrDefaultAsync(w => w.ApplicationUserId == userId);

            var booking = await _context.Bookings
                .Include(b => b.CustomerProfile)
                .ThenInclude(c => c!.ApplicationUser)
                .FirstOrDefaultAsync(b => b.Id == id && b.WorkerProfileId == worker!.Id);

            if (booking == null)
            {
                return NotFound("You are not authorized to access this booking or it doesn't exist.");
            }

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var worker = await _context.WorkerProfiles.FirstOrDefaultAsync(w => w.ApplicationUserId == userId);

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.WorkerProfileId == worker!.Id);

            if (booking == null || booking.Status != "REQUESTED")
            {
                return BadRequest("This booking is no longer available for acceptance.");
            }

            // Using existing JobSchedulingService logic to atomically check conflict and insert JobSequence if passed
            var jobSequence = await _jobSchedulingService.ScheduleJobAsync(
                worker!.Id, 
                booking.Id, 
                booking.ExpectedStartTime, 
                booking.ExpectedEndTime
            );

            if (jobSequence == null)
            {
                return BadRequest("Worker is already booked during this time.");
            }

            // If sequence created successfully, update booking status
            booking.Status = "ACCEPTED";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var worker = await _context.WorkerProfiles.FirstOrDefaultAsync(w => w.ApplicationUserId == userId);

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.WorkerProfileId == worker!.Id);

            if (booking == null || booking.Status != "REQUESTED")
            {
                return BadRequest("This booking is no longer available for decline.");
            }

            booking.Status = "DECLINED";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
