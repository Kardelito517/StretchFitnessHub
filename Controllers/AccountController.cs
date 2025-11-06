using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StretchFitnessHub.Data;
using StretchFitnessHub.Models;
using StretchFitnessHub.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;

namespace StretchFitnessHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult LandingPage()
        {
            return View();
        }
        private PricingSettings GetOrCreatePricing()
        {
            var pricing = _context.PricingSettings.FirstOrDefault();

            if (pricing == null)
            {
                pricing = new PricingSettings
                {
                    GymOnlyPrice = 1000,
                    GymWithClassPrice = 3000,
                    ClassOnlyPrice = 2500,
                    WalkInGymPrice = 80,
                    WalkInClassPrice = 120
                };

                _context.PricingSettings.Add(pricing);
                _context.SaveChanges();
            }

            return pricing;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["LoginError"] = "Please enter username and password.";
                return RedirectToAction("LandingPage");
            }

            string username = model.Username.Trim();
            string password = model.Password.Trim();

            /* ==========================
               ✅ ADMIN LOGIN
            ========================== */
            var admin = _context.Admins.FirstOrDefault(a => a.Username == username);
            if (admin != null && BCrypt.Net.BCrypt.Verify(password, admin.Password))
            {
                HttpContext.Session.SetInt32("AdminId", admin.Id);
                HttpContext.Session.SetString("Username", admin.Username);
                HttpContext.Session.SetString("Role", "Admin");
                HttpContext.Session.SetString("IsLoggedIn", "true");

                return RedirectToAction("Admin", "Admin");
            }

            /* ==========================
               ✅ STAFF LOGIN
            ========================== */
            var staff = _context.Staffs.FirstOrDefault(s => s.Username == username);
            if (staff != null && BCrypt.Net.BCrypt.Verify(password, staff.Password))
            {
                HttpContext.Session.SetInt32("StaffId", staff.Id);
                HttpContext.Session.SetString("Username", staff.Username);
                HttpContext.Session.SetString("Role", "Staff");
                HttpContext.Session.SetString("IsLoggedIn", "true");

                // 🔹 Redirect to Admin dashboard with role-based filtering
                return RedirectToAction("Admin", "Admin");
            }

            /* ==========================
               ✅ MEMBER LOGIN
            ========================== */
            var member = _context.Members.FirstOrDefault(m => m.FullName == username && m.Password == password);
            if (member != null)
            {
                HttpContext.Session.SetInt32("MemberId", member.Id);
                HttpContext.Session.SetString("Username", member.FullName);
                HttpContext.Session.SetString("Role", "Member");
                HttpContext.Session.SetString("IsLoggedIn", "true");

                return RedirectToAction("Index", "Member");
            }

            /* ==========================
               ❌ INVALID LOGIN
            ========================== */
            TempData["LoginError"] = "Invalid username or password.";
            return RedirectToAction("LandingPage");
        }





        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LandingPage", "Account");
        }

        public IActionResult Walkin()
        {
            ViewBag.Pricing = GetOrCreatePricing();

            var viewModel = new WalkinViewModel
            {
                ClassList = _context.ClassSchedules.ToList()
            };

            return View(viewModel);
        }




        [HttpGet]
        public IActionResult Registration()
        {
            ViewBag.ExistingMember = false;
            ViewBag.Pricing = GetOrCreatePricing();

            var vm = new RegistrationPageViewModel
            {
                Request = new RegistrationRequest(),
                ClassList = _context.ClassSchedules.ToList()
            };

            return View(vm);
        }




        [HttpGet]
        public IActionResult GetClasses()
        {
            var classes = _context.ClassSchedules
                .Select(c => new { c.Id, c.ClassName })
                .ToList();

            return Json(classes);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registration(RegistrationPageViewModel vm)
        {
            if (vm?.Request == null)
            {
                ModelState.AddModelError("", "Invalid submission.");
                return View(vm);
            }

            var model = vm.Request;

            if (!ModelState.IsValid)
                return View(vm);

            // Check for duplicates
            string fullName = model.FullName.Trim().ToLower();
            string email = model.Email.Trim().ToLower();
            string phone = model.PhoneNumber.Trim();

            bool existsInMembers = _context.Members.Any(m =>
                m.FullName.ToLower() == fullName ||
                m.Email.ToLower() == email ||
                m.PhoneNumber == phone
            );

            bool existsInRequests = _context.RegistrationRequests.Any(r =>
                r.FullName.ToLower() == fullName ||
                r.Email.ToLower() == email ||
                r.PhoneNumber == phone
            );

            if (existsInMembers || existsInRequests)
            {
                ViewBag.ExistingMember = true;
                return View(vm);
            }

            // Validate PreferredClasses for plans that require it
            if ((model.MembershipPlan == "GymWithClass" || model.MembershipPlan == "ClassOnly")
                && string.IsNullOrEmpty(model.PreferredClasses))
            {
                ModelState.AddModelError("", "Please select a preferred class.");
                return View(vm);
            }

            // Save request
            model.RequestDate = DateTime.Now;
            _context.RegistrationRequests.Add(model);
            _context.SaveChanges();

            TempData["Message"] = "Registration submitted successfully. Please wait for admin approval.";
            return RedirectToAction("Registration");
        }





        [HttpGet]
        public IActionResult Admin()
        {
            var viewModel = new DashboardViewModel
            {
                RegistrationRequests = _context.RegistrationRequests?.ToList() ?? new List<RegistrationRequest>(),
                Members = _context.Members?.ToList() ?? new List<Member>(),
                Notifications = _context.Notifications?.ToList() ?? new List<Notification>()
            };
            foreach (var request in viewModel.RegistrationRequests)
            {
                bool alreadyExists = _context.Notifications?.Any(n => n.Message == $"{request.FullName} is requesting registration.") ?? false;
                if (!alreadyExists)
                {
                    _context.Notifications?.Add(new Notification
                    {
                        Message = $"{request.FullName} is requesting registration.",
                        IsRead = false,
                        DateCreated = DateTime.Now
                    });
                }
            }
            foreach (var member in viewModel.Members)
            {
                if (member.MembershipEnd.Date == DateTime.Now.Date.AddDays(7))
                {
                    bool alreadyExists = _context.Notifications?.Any(n => n.Message == $"{member.FullName}'s membership will end in 7 days.") ?? false;
                    if (!alreadyExists)
                    {
                        _context.Notifications?.Add(new Notification
                        {
                            Message = $"{member.FullName}'s membership will end in 7 days.",
                            IsRead = false,
                            DateCreated = DateTime.Now
                        });
                    }
                }
            }

            _context.SaveChanges();
            viewModel.Notifications = _context.Notifications?.ToList() ?? new List<Notification>();

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            var member = _context.Members.FirstOrDefault(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            return View("~/Views/Account/Details.cshtml", member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitWalkin(string FullName, string WalkinPlan, string PreferredClasses)
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                TempData["Message"] = "Name is required.";
                return RedirectToAction("Walkin");
            }

            // Save WalkIn record
            var walkin = new WalkIn
            {
                FullName = FullName,
                TimeIn = DateTime.Now,
                Plan = WalkinPlan,
                PreferredClass = WalkinPlan == "ClassPlan" ? PreferredClasses : null
            };

            _context.WalkIns.Add(walkin);

            // Optional: Add notification for admin
            var notification = new Notification
            {
                Message = $"{FullName} wants to walk in ({WalkinPlan}" +
                          (WalkinPlan == "ClassPlan" ? $", Class: {PreferredClasses}" : "") + ").",
                IsRead = false,
                DateCreated = DateTime.Now
            };

            _context.Notifications.Add(notification);
            _context.SaveChanges();

            TempData["Message"] = "Walk-in logged successfully!";
            return RedirectToAction("Walkin");
        }




    }
}
