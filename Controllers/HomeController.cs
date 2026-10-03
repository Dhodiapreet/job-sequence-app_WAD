using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using JobSequenceApp.Models;

namespace JobSequenceApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            if (User.IsInRole("Admin")) return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            if (User.IsInRole("Worker")) return RedirectToAction("Index", "Dashboard", new { area = "Worker" });
            if (User.IsInRole("Customer")) return RedirectToAction("Index", "Dashboard", new { area = "Customer" });
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
