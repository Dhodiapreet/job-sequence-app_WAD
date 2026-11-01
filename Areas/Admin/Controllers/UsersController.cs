using JobSequenceApp.Models;
using JobSequenceApp.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobSequenceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = new List<AdminUserRowViewModel>();
            foreach (var user in _userManager.Users.OrderBy(u => u.FirstName).ToList())
            {
                users.Add(new AdminUserRowViewModel
                {
                    Id = user.Id,
                    Name = $"{user.FirstName} {user.LastName}".Trim(),
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Roles = string.Join(", ", await _userManager.GetRolesAsync(user)),
                    EmailConfirmed = user.EmailConfirmed
                });
            }

            return View(users);
        }
    }
}