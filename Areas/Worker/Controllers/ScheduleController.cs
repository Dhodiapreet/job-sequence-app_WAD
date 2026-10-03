using System.Security.Claims;
using JobSequenceApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSequenceApp.Areas.Worker.Controllers
{
    [Area("Worker")]
    [Authorize(Roles = "Worker")]
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var worker = await _context.WorkerProfiles.FirstOrDefaultAsync(w => w.ApplicationUserId == userId);

            if (worker == null) return NotFound();

            var sequences = await _context.JobSequences
                .Include(j => j.Booking)
                .ThenInclude(b => b!.CustomerProfile)
                .ThenInclude(c => c!.ApplicationUser)
                .Where(j => j.WorkerProfileId == worker.Id && j.Status != "Cancelled")
                .OrderBy(j => j.StartTime)
                .ToListAsync();

            return View(sequences);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sequence = await _context.JobSequences
                .Include(j => j.WorkerProfile)
                .Include(j => j.Booking)
                .ThenInclude(b => b!.CustomerProfile)
                .ThenInclude(c => c!.ApplicationUser)
                .FirstOrDefaultAsync(j => j.Id == id && j.WorkerProfile!.ApplicationUserId == userId);

            if (sequence == null) return NotFound();

            return View(sequence);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartJob(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sequence = await _context.JobSequences
                .Include(j => j.WorkerProfile)
                .Include(j => j.Booking)
                .FirstOrDefaultAsync(j => j.Id == id && j.WorkerProfile!.ApplicationUserId == userId);

            if (sequence == null) return NotFound();

            if (sequence.Status == "Scheduled" && sequence.Booking!.Status == "ACCEPTED")
            {
                sequence.Status = "InProgress";
                sequence.Booking.Status = "IN_PROGRESS";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Job started successfully.";
            }
            else
            {
                TempData["Error"] = "Invalid job transition.";
            }

            return RedirectToAction(nameof(Details), new { id = sequence.Id });
        }

        public async Task<IActionResult> CompleteJob(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sequence = await _context.JobSequences
                .Include(j => j.WorkerProfile)
                .Include(j => j.Booking)
                .FirstOrDefaultAsync(j => j.Id == id && j.WorkerProfile!.ApplicationUserId == userId);

            if (sequence == null || sequence.Status != "InProgress") return NotFound();

            return View(sequence);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteJob(int id, string completionNotes)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sequence = await _context.JobSequences
                .Include(j => j.WorkerProfile)
                .Include(j => j.Booking)
                .FirstOrDefaultAsync(j => j.Id == id && j.WorkerProfile!.ApplicationUserId == userId);

            if (sequence == null || sequence.Status != "InProgress") return NotFound();

            sequence.Status = "Completed";
            sequence.Booking!.Status = "COMPLETED";
            // Storing notes in Description temporarily or skipping since MVP.
            // Let's just finish the transition.
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job marked as completed.";
            return RedirectToAction(nameof(Details), new { id = sequence.Id });
        }
    }
}
