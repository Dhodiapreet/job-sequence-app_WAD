using JobSequenceApp.Data;
using JobSequenceApp.Models;
using JobSequenceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JobSequenceApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
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

            var customer = await _context.CustomerProfiles
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
            
            if (customer == null) return NotFound("Customer profile not found.");

            var bookings = await _context.Bookings
                .Where(b => b.CustomerProfileId == customer.Id)
                .OrderBy(b => b.RequestedDate)
                .ToListAsync();

            var vm = new CustomerDashboardViewModel
            {
                WelcomeMessage = $"Welcome back, {customer.ApplicationUser?.FirstName}!",
                UpcomingBookings = bookings.Where(b => b.Status == "Pending").ToList(),
                ActiveBookings = bookings.Where(b => b.Status == "Assigned").ToList(),
                CompletedBookings = bookings.Where(b => b.Status == "Completed").Take(5).ToList()
            };

            return View(vm);
        }
    }
}
