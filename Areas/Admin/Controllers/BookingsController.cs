using JobSequenceApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSequenceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.WorkerProfile)
                .ThenInclude(w => w!.ApplicationUser)
                .Include(b => b.CustomerProfile)
                .ThenInclude(c => c!.ApplicationUser)
                .OrderByDescending(b => b.RequestedDate)
                .ToListAsync();

            return View(bookings);
        }
    }
}
