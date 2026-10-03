using JobSequenceApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSequenceApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class WorkersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchString)
        {
            var query = _context.WorkerProfiles
                .Include(w => w.ApplicationUser)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(w => w.Profession.Contains(searchString) || 
                                         w.Skills.Contains(searchString) || 
                                         w.ApplicationUser!.FirstName.Contains(searchString));
            }

            var workers = await query.ToListAsync();
            return View(workers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var worker = await _context.WorkerProfiles
                .Include(w => w.ApplicationUser)
                .Include(w => w.Reviews)
                .ThenInclude(r => r.CustomerProfile)
                .ThenInclude(c => c!.ApplicationUser)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (worker == null)
            {
                return NotFound("No worker was found.");
            }

            return View(worker);
        }
    }
}
