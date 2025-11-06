using Microsoft.AspNetCore.Mvc;
using StretchFitnessHub.Data;
using StretchFitnessHub.Filters;
using StretchFitnessHub.Models;

namespace StretchFitnessHub.Controllers
{

    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var staffList = _context.Staffs.ToList(); 
            return View(staffList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStaff(string fullName, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Please fill out all fields.";
                return RedirectToAction("Admin", "Admin");
            }

            var existingStaff = _context.Staffs.FirstOrDefault(s => s.Username == username);
            if (existingStaff != null)
            {
                TempData["Error"] = "Username already exists.";
                return RedirectToAction("Admin", "Admin");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newStaff = new Staff
            {
                FullName = fullName,
                Username = username,
                Password = hashedPassword,
                Role = "Staff"
            };

            _context.Staffs.Add(newStaff);
            _context.SaveChanges();

            TempData["Success"] = "New staff account created successfully!";
            return RedirectToAction("Admin", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveStaff(int id)
        {
            var staff = _context.Staffs.FirstOrDefault(s => s.Id == id);
            if (staff != null)
            {
                _context.Staffs.Remove(staff);
                _context.SaveChanges();
                TempData["Success"] = "Staff removed successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to remove staff.";
            }
            return RedirectToAction("Admin", "Admin");
        }

    }
}
