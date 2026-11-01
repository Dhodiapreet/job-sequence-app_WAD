using JobSequenceApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSequenceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class WorkersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(bool pendingOnly = false)
        {
            var query = _context.WorkerProfiles
                .Include(w => w.ApplicationUser)
                .AsQueryable();

            if (pendingOnly)
            {
                query = query.Where(w => !w.IsVerified);
                ViewData["Title"] = "Pending Worker Verifications";
            }
            else
            {
                ViewData["Title"] = "Manage Workers";
            }

            ViewBag.PendingOnly = pendingOnly;
            var workers = await query.OrderBy(w => w.IsVerified)
                .ThenBy(w => w.ApplicationUser!.FirstName)
                .ToListAsync();

            return View(workers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(int id)
        {
            var worker = await _context.WorkerProfiles.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }

            worker.IsVerified = !worker.IsVerified;
            await _context.SaveChangesAsync();

            TempData["AdminMessage"] = worker.IsVerified
                ? "Worker verified successfully."
                : "Worker verification was revoked.";

            return RedirectToAction(nameof(Index), new { pendingOnly = false });
        }
    }
}