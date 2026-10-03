using System.Security.Claims;
using JobSequenceApp.Data;
using JobSequenceApp.Models;
using JobSequenceApp.Services;
using JobSequenceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSequenceApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IJobSchedulingService _jobSchedulingService;

        public BookingsController(ApplicationDbContext context, IJobSchedulingService jobSchedulingService)
        {
            _context = context;
            _jobSchedulingService = jobSchedulingService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.CustomerProfiles.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null) return NotFound();

            var bookings = await _context.Bookings
                .Include(b => b.WorkerProfile)
                .ThenInclude(w => w!.ApplicationUser)
                .Where(b => b.CustomerProfileId == customer.Id)
                .OrderByDescending(b => b.ExpectedStartTime)
                .ToListAsync();

            return View(bookings);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.CustomerProfiles.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            var booking = await _context.Bookings
                .Include(b => b.WorkerProfile)
                .ThenInclude(w => w!.ApplicationUser)
                .FirstOrDefaultAsync(b => b.Id == id && b.CustomerProfileId == customer!.Id);

            if (booking == null)
            {
                return NotFound("You are not authorized to access this booking or it doesn't exist.");
            }

            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int workerId)
        {
            var worker = await _context.WorkerProfiles
                .Include(w => w.ApplicationUser)
                .FirstOrDefaultAsync(w => w.Id == workerId);

            if (worker == null) return NotFound("No worker was found.");

            var model = new CreateBookingViewModel
            {
                WorkerProfileId = worker.Id,
                WorkerName = $"{worker.ApplicationUser?.FirstName} {worker.ApplicationUser?.LastName}",
                Service = worker.Profession
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookingViewModel model)
        {
            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time must be after start time.");
            }

            var worker = await _context.WorkerProfiles
                .Include(w => w.ApplicationUser)
                .FirstOrDefaultAsync(w => w.Id == model.WorkerProfileId);

            if (worker == null)
            {
                ModelState.AddModelError("", "Worker not found.");
            }
            else
            {
                model.WorkerName = $"{worker.ApplicationUser?.FirstName} {worker.ApplicationUser?.LastName}";
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var expectedStart = model.Date.Date + model.StartTime;
            var expectedEnd = model.Date.Date + model.EndTime;

            // Conflict check
            bool canSchedule = await _jobSchedulingService.CanScheduleJobAsync(model.WorkerProfileId, expectedStart, expectedEnd);
            
            if (!canSchedule)
            {
                ModelState.AddModelError("", "Worker is already booked during this time.");
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.CustomerProfiles.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null) return Forbid();

            var booking = new Booking
            {
                CustomerProfileId = customer.Id,
                WorkerProfileId = model.WorkerProfileId,
                Title = model.Title,
                Description = model.Description,
                Service = model.Service,
                Location = model.Location,
                RequestedDate = DateTime.UtcNow,
                ExpectedStartTime = expectedStart,
                ExpectedEndTime = expectedEnd,
                Status = "REQUESTED"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> WriteReview(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.CustomerProfiles.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
            
            var booking = await _context.Bookings
                .Include(b => b.WorkerProfile)
                .ThenInclude(w => w!.ApplicationUser)
                .FirstOrDefaultAsync(b => b.Id == id && b.CustomerProfileId == customer!.Id);

            if (booking == null || booking.Status != "COMPLETED")
                return NotFound("Booking not found or not completed.");

            // Check if review already exists
            var existingReview = await _context.Reviews.FirstOrDefaultAsync(r => r.BookingId == id);
            if (existingReview != null)
            {
                TempData["Error"] = "You have already reviewed this booking.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var model = new Review
            {
                BookingId = booking.Id,
                WorkerProfileId = booking.WorkerProfileId,
                CustomerProfileId = customer!.Id
            };

            ViewBag.WorkerName = $"{booking.WorkerProfile?.ApplicationUser?.FirstName} {booking.WorkerProfile?.ApplicationUser?.LastName}";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WriteReview(Review model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.CustomerProfiles.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (model.CustomerProfileId != customer?.Id)
                return Forbid();

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == model.BookingId && b.CustomerProfileId == customer.Id);

            if (booking == null || booking.Status != "COMPLETED")
                return NotFound("Booking not found or not completed.");

            // Check duplicate
            if (await _context.Reviews.AnyAsync(r => r.BookingId == model.BookingId))
            {
                TempData["Error"] = "You have already reviewed this booking.";
                return RedirectToAction(nameof(Details), new { id = model.BookingId });
            }

            if (ModelState.IsValid)
            {
                model.WorkerProfileId = booking.WorkerProfileId; // Fix IDOR
                model.CreatedAt = DateTime.UtcNow;
                _context.Reviews.Add(model);

                // Update Worker Average Rating
                var worker = await _context.WorkerProfiles
                    .Include(w => w.Reviews)
                    .FirstOrDefaultAsync(w => w.Id == model.WorkerProfileId);
                
                if (worker != null)
                {
                    int totalReviews = worker.TotalReviews + 1;
                    double currentTotal = worker.AverageRating * worker.TotalReviews;
                    worker.AverageRating = (currentTotal + model.Rating) / totalReviews;
                    worker.TotalReviews = totalReviews;
                }

                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Thank you for your review.";
                return RedirectToAction(nameof(Details), new { id = model.BookingId });
            }

            return View(model);
        }
    }
}
