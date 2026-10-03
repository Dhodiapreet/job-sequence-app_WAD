using JobSequenceApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSequenceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string status)
        {
            var query = _context.JobSequences
                .Include(j => j.WorkerProfile)
                .ThenInclude(w => w!.ApplicationUser)
                .Include(j => j.Booking)
                .ThenInclude(b => b!.CustomerProfile)
                .ThenInclude(c => c!.ApplicationUser)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(j => j.Status == status);
            }

            var jobs = await query.OrderByDescending(j => j.StartTime).ToListAsync();
            return View(jobs);
        }
    }
}
