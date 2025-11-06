using Microsoft.AspNetCore.Mvc;
using StretchFitnessHub.Filters;
using StretchFitnessHub.Data;
using StretchFitnessHub.Models;
using StretchFitnessHub.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using QRCoder;
using System.Drawing;
using System.IO;

namespace StretchFitnessHub.Controllers
{
    [RoleAuthorize("Staff,Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Admin()
        {
            // --- Check if user is logged in ---
            string? role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("LandingPage", "Account");
            }

            if (role == "Member")
            {
                return RedirectToAction("Index", "Member");
            }

            // --- Continue normal dashboard loading ---
            UpdateMembershipStatuses();

            var today = DateTime.Today;
            var now = DateTime.Now;
            var sevenDaysLater = today.AddDays(7);

            var allMembers = _context.Members?.ToList() ?? new List<Member>();
            var activeMembers = allMembers.Where(m => !m.IsArchived).ToList();
            var archivedMembers = allMembers.Where(m => m.IsArchived).ToList();
            var requests = _context.RegistrationRequests?.ToList() ?? new List<RegistrationRequest>();
            var existingNotifications = _context.Notifications?.ToList() ?? new List<Notification>();

            // --- Cleanup old notifications ---
            var oldNotifs = existingNotifications
                .Where(n =>
                    (n.Message.Contains("membership will expire") &&
                     DateTime.TryParse(n.DateCreated.ToString(), out var dt) &&
                     (today - dt.Date).TotalDays > 1) ||
                    n.DateCreated.Date < today.AddDays(-7))
                .ToList();
            if (oldNotifs.Any())
            {
                _context.Notifications.RemoveRange(oldNotifs);
                _context.SaveChanges();
            }

            // --- Remove stale registration notifs ---
            var registrationNotifsToRemove = existingNotifications
                .Where(n => n.Message.Contains("requested to register") &&
                            !requests.Any(r => n.Message.Contains(r.FullName)))
                .ToList();
            if (registrationNotifsToRemove.Any())
            {
                _context.Notifications.RemoveRange(registrationNotifsToRemove);
                _context.SaveChanges();
            }

            // --- Remove expiry notifs that no longer apply ---
            var expiryNotifsToRemove = existingNotifications
                .Where(n => n.Message.Contains("membership will expire"))
                .ToList();
            foreach (var notif in expiryNotifsToRemove)
            {
                var namePart = notif.Message.Split("'")[0].Trim();
                var member = activeMembers.FirstOrDefault(m => m.FullName == namePart);
                if (member == null || member.MembershipEnd.Date > sevenDaysLater)
                    _context.Notifications.Remove(notif);
            }
            _context.SaveChanges();

            // --- Add expiring soon notifications ---
            var expiringSoonMembers = activeMembers
                .Where(m => m.MembershipEnd.Date >= today && m.MembershipEnd.Date <= sevenDaysLater)
                .ToList();
            foreach (var m in expiringSoonMembers)
            {
                var daysLeft = (m.MembershipEnd.Date - today).Days;
                string dayText = daysLeft == 0 ? "today"
                                  : daysLeft == 1 ? "in 1 day"
                                  : $"in {daysLeft} days";

                string message = $"{m.FullName}'s membership will expire {dayText} ({m.MembershipEnd:MMM dd, yyyy}).";
                bool exists = existingNotifications.Any(n => n.Message.Contains(m.FullName) && n.Message.Contains("membership will expire"));
                if (!exists)
                {
                    _context.Notifications.Add(new Notification
                    {
                        Message = message,
                        DateCreated = DateTime.Now
                    });
                }
            }

            // --- Add registration notifications ---
            foreach (var req in requests)
            {
                string message = $"{req.FullName} has requested to register.";
                bool exists = existingNotifications.Any(n => n.Message.Contains(req.FullName) && n.Message.Contains("requested to register"));
                if (!exists)
                {
                    _context.Notifications.Add(new Notification
                    {
                        Message = message,
                        DateCreated = req.RequestDate
                    });
                }
            }
            _context.SaveChanges();

            // --- Load final notifications ---
            var notifications = _context.Notifications?
                .Where(n =>
                    (n.Message.Contains("requested to register") ||
                     n.Message.Contains("wants to walk in") ||
                     n.Message.Contains("membership will expire")) &&
                    n.DateCreated.Date >= today.AddDays(-7))
                .OrderByDescending(n => n.DateCreated)
                .ToList() ?? new List<Notification>();

            int notificationCount = notifications.Count;

            // --- CLASS SCHEDULES ---
            var classSchedules = _context.ClassSchedules?.ToList() ?? new List<ClassSchedule>();
            var ongoingClasses = new List<ClassSchedule>();
            var upcomingClasses = new List<ClassSchedule>();

            var currentDay = now.DayOfWeek;
            var currentTime = now.TimeOfDay;

            foreach (var cls in classSchedules)
            {
                var classDays = cls.Days
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.Trim())
                    .Where(d => Enum.TryParse<DayOfWeek>(d, true, out _))
                    .Select(d => Enum.Parse<DayOfWeek>(d, true))
                    .ToList();

                // ✅ Ongoing: same day, current time between start & end
                if (classDays.Contains(currentDay) &&
                    currentTime >= cls.StartTime &&
                    currentTime <= cls.EndTime)
                {
                    ongoingClasses.Add(cls);
                }
                // ✅ Upcoming: same day, but not yet started
                else if (classDays.Contains(currentDay) &&
                         currentTime < cls.StartTime)
                {
                    upcomingClasses.Add(cls);
                }
            }

            int classesTodayCount = ongoingClasses.Count;

            var todayWalkIns = _context.WalkIns?
                .Where(w => w.TimeIn.Date == today)
                .OrderByDescending(w => w.TimeIn)
                .ToList() ?? new List<WalkIn>();

            // ✅ Pass everything including upcoming + ongoing
            var viewModel = new DashboardViewModel
            {
                Members = activeMembers,
                ArchivedMembers = archivedMembers,
                RegistrationRequests = requests,
                Notifications = notifications,
                NotificationCount = notificationCount,
                TotalMembers = activeMembers.Count,
                ExpiringSoon = expiringSoonMembers.Count,
                OngoingClasses = ongoingClasses.Select(c => c.ClassName).ToHashSet(),
                UpcomingClasses = upcomingClasses.Select(c => c.ClassName).ToHashSet(),
                ClassesToday = classesTodayCount,
                ClassSchedules = classSchedules,
                WalkIns = todayWalkIns,
                CurrentUserRole = role,
                Staffs = _context.Staffs?.ToList() ?? new List<Staff>()
            };

            return View(viewModel);
        }






        [HttpPost]
        public IActionResult RemoveClass(int id)
        {
            var classToRemove = _context.ClassSchedules.FirstOrDefault(c => c.Id == id);
            if (classToRemove == null)
                return Json(new { success = false, message = "Class not found." });

            _context.ClassSchedules.Remove(classToRemove);
            _context.SaveChanges();

            return Json(new { success = true, message = "Class removed successfully." });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddClass([FromBody] ClassSchedule newClass)
        {
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(role) || role != "Admin")
                return Json(new { success = false, message = "Unauthorized access. Only Admins can add classes." });

            if (newClass == null ||
                string.IsNullOrWhiteSpace(newClass.ClassName) ||
                string.IsNullOrWhiteSpace(newClass.Coach) ||
                string.IsNullOrWhiteSpace(newClass.Days))
            {
                return Json(new { success = false, message = "Invalid data. Please check all fields." });
            }

            newClass.ClassName = newClass.ClassName.Trim();
            newClass.Coach = newClass.Coach.Trim();

            if (newClass.EndTime <= newClass.StartTime)
                return Json(new { success = false, message = "End time must be later than start time." });

            // Check for schedule conflicts
            var existingClasses = _context.ClassSchedules.ToList();

            var newDays = newClass.Days
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(d => d.ToLower()).ToList();

            foreach (var cls in existingClasses)
            {
                var existingDays = cls.Days
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.ToLower()).ToList();

                var overlapDays = newDays.Intersect(existingDays);
                if (overlapDays.Any())
                {
                    bool isOverlap = newClass.StartTime < cls.EndTime && newClass.EndTime > cls.StartTime;
                    if (isOverlap)
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"Schedule conflict with '{cls.ClassName}' ({cls.StartTime:hh\\:mm}-{cls.EndTime:hh\\:mm}) on {string.Join(",", overlapDays)}."
                        });
                    }
                }
            }

            try
            {
                _context.ClassSchedules.Add(newClass);
                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    id = newClass.Id,
                    className = newClass.ClassName,
                    coach = newClass.Coach,
                    days = newClass.Days,
                    startTime = newClass.StartTime.ToString(@"hh\:mm"),
                    endTime = newClass.EndTime.ToString(@"hh\:mm")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving class: " + ex.Message });
            }
        }








        private void UpdateMembershipStatuses()
        {
            var today = DateTime.Today;
            var allMembers = _context.Members.ToList();

            foreach (var member in allMembers)
            {
                if (member.IsArchived)
                {
                    member.Status = "Archived";
                }
                else
                {
                    member.Status = member.MembershipEnd < today ? "Inactive" : "Active";
                }
            }

            _context.SaveChanges();
        }

         




        [HttpPost]
        public IActionResult ArchiveMember(int id)
        {
            var member = _context.Members.FirstOrDefault(m => m.Id == id);
            if (member == null)
            {
                TempData["ActionResult"] = "Failed";
                return RedirectToAction(nameof(Admin));
            }

            if (!member.IsArchived)
            {
                member.IsArchived = true;
                member.Status = "Archived";
                _context.SaveChanges();

                TempData["ActionResult"] = "Archived";
                TempData["MemberName"] = member.FullName;
            }

            return RedirectToAction(nameof(Admin));
        }

        [HttpPost]
        public IActionResult UnarchiveMember(int id)
        {
            var member = _context.Members.FirstOrDefault(m => m.Id == id);
            if (member == null)
            {
                TempData["ActionResult"] = "Failed";
                return RedirectToAction(nameof(Admin));
            }

            if (member.IsArchived)
            {
                member.IsArchived = false;
                member.Status = member.MembershipEnd < DateTime.Today ? "Inactive" : "Active";
                _context.SaveChanges();

                TempData["ActionResult"] = "Unarchived";
                TempData["MemberName"] = member.FullName;
            }

            return RedirectToAction(nameof(Admin));
        }

        [HttpPost]
        public IActionResult RenewMember(int id)
        {
            var member = _context.Members.FirstOrDefault(m => m.Id == id);
            if (member != null)
            {
                var today = DateTime.Now.Date;
                if (today > member.MembershipEnd)
                {
                    member.MembershipStart = today;
                    member.MembershipEnd = today.AddMonths(1);
                }
                else
                {
                    member.MembershipEnd = member.MembershipEnd.AddMonths(1);
                }

                member.IsArchived = false;
                member.Status = "Active";
                member.FreezeStartDate = null;

                _context.SaveChanges();
            }

            return Ok();
        }



        [HttpPost]
        public IActionResult DeleteMember(int id)
        {
            var member = _context.Members.FirstOrDefault(m => m.Id == id);
            if (member != null)
            {
                _context.Members.Remove(member);
                _context.SaveChanges();
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult GetRegistrationRequests()
        {
            var requests = _context.RegistrationRequests
                .Select(r => new { r.Id, r.FullName })
                .ToList();

            return Json(new { requests });
        }

        [HttpGet]
        public IActionResult GetMembers()
        {
            UpdateMembershipStatuses();

            var members = _context.Members
                .Select(m => new { m.Id, m.FullName, m.MembershipEnd, m.Status, m.IsArchived })
                .ToList();

            return Json(new { members, totalMembers = members.Count });
        }



        [HttpGet]
        public IActionResult GetMemberProfile(int id)
        {
            var member = _context.Members.FirstOrDefault(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            var profile = new
            {
                Pic = member.ProfileImagePath,
                Name = member.FullName,
                Email = member.Email,
                Phone = member.PhoneNumber,
                Membership = member.MembershipStart.ToString("MMM dd, yyyy") + " - " + member.MembershipEnd.ToString("MMM dd, yyyy"),
                PreferredClass = member.PreferredClasses,
                Status = member.Status,
                Password = member.Password
            };

            return Json(profile);
        }


        public IActionResult GenerateQRCode(string text)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeAsPng = qrCode.GetGraphic(20);

                return File(qrCodeAsPng, "image/png");
            }
        }

        private byte[] GenerateQrCodeBytes(string text)
        {
            using (var qrGenerator = new QRCoder.QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(text, QRCoder.QRCodeGenerator.ECCLevel.Q))
            {
                var pngQr = new QRCoder.PngByteQRCode(qrCodeData);
                return pngQr.GetGraphic(20);
            }
        }



        [HttpGet]
        public IActionResult VerifyMember(string memberId)
        {
            var member = _context.Members.FirstOrDefault(m => m.QrCodeValue == memberId);
            if (member == null)
                return Json(new { status = "error", message = "Member not found" });

            bool isActive = member.MembershipEnd >= DateTime.Now;

            return Json(new
            {
                status = isActive ? "active" : "inactive",
                name = member.FullName,
                validUntil = member.MembershipEnd.ToString("MMM dd, yyyy")
            });
        }


        [HttpGet]
        public IActionResult GenerateRegistrationQRCode()
        {
            string registrationUrl = "http://192.168.100.80:5002/Account/Registration";
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(registrationUrl, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeBytes = qrCode.GetGraphic(20);

                return File(qrCodeBytes, "image/png");
            }
        }

        [HttpGet]
        public IActionResult GenerateWalkinQRCode()
        {
            string walkinUrl = "http://192.168.100.80:5002/Account/Walkin";

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(walkinUrl, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeBytes = qrCode.GetGraphic(20);

                return File(qrCodeBytes, "image/png");
            }
        }


        [HttpPost]
        public IActionResult UpdateSettings(string username, string password, string language)
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
                return Unauthorized();

            var admin = _context.Admins.FirstOrDefault(a => a.Id == adminId);
            if (admin == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(username))
            {
                admin.Username = username;
                HttpContext.Session.SetString("Username", username);
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                admin.Password = BCrypt.Net.BCrypt.HashPassword(password);
            }

            _context.SaveChanges();

            return Json(new { success = true, username = admin.Username});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveWalkIn(int id, string memberName, string walkinPlan, string preferredClass)
        {
            // Hanapin ang walkin notification
            var notif = _context.Notifications.Find(id);
            if (notif == null)
            {
                TempData["ActionResult"] = "Failed";
                return RedirectToAction("Admin", "Admin");
            }

            string fullName = memberName;
            if (string.IsNullOrWhiteSpace(fullName) && notif.Message.Contains("wants to walk in"))
            {
                fullName = notif.Message.Replace("wants to walk in", "").Trim();
            }

            // Hanapin kung may existing WalkIn record
            var existingWalkIn = _context.WalkIns.FirstOrDefault(w => w.FullName == fullName && !w.IsApprove);
            if (existingWalkIn != null)
            {
                existingWalkIn.IsApprove = true;
                existingWalkIn.Plan = walkinPlan ?? existingWalkIn.Plan;
                existingWalkIn.PreferredClass = preferredClass ?? existingWalkIn.PreferredClass;
                existingWalkIn.TimeIn = DateTime.Now;
            }
            else
            {
                var walkin = new WalkIn
                {
                    FullName = fullName,
                    TimeIn = DateTime.Now,
                    Plan = walkinPlan ?? "Gym Plan",
                    PreferredClass = preferredClass ?? "None",
                    IsApprove = true
                };
                _context.WalkIns.Add(walkin);
            }

            // Remove the notification
            _context.Notifications.Remove(notif);
            _context.SaveChanges();

            TempData["ActionResult"] = "WalkInApproved";
            TempData["MemberName"] = fullName;

            return RedirectToAction("Admin", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectWalkIn(int id, string memberName)
        {
            var notif = _context.Notifications.Find(id);
            if (notif != null)
            {
                _context.Notifications.Remove(notif);
                _context.SaveChanges();
            }

            TempData["ActionResult"] = "WalkInRejected";
            TempData["MemberName"] = memberName;

            return RedirectToAction("Admin", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveRegistration(int id)
        {
            var registration = _context.RegistrationRequests.FirstOrDefault(r => r.Id == id);
            if (registration == null)
            {
                TempData["ActionResult"] = "Failed";
                return RedirectToAction("Admin", "Admin");
            }

            // Ensure PreferredClasses is never null
            string preferredClasses = string.IsNullOrWhiteSpace(registration.PreferredClasses)
                ? "None"
                : registration.PreferredClasses;

            // Create new Member
            var newMember = new Member
            {
                FullName = registration.FullName,
                Email = registration.Email,
                PhoneNumber = registration.PhoneNumber,
                Password = registration.Password,
                MembershipPlan = registration.MembershipPlan,
                PreferredClasses = preferredClasses, // Always copy if exists
                MembershipStart = DateTime.Now,
                MembershipEnd = DateTime.Now.AddMonths(1),
                IsArchived = false,
                Status = "Active"
            };

            _context.Members.Add(newMember);
            _context.SaveChanges();

            // Generate & save QR Code
            string baseUrl = "http://192.168.100.80:5002"; // replace with your base URL
            string qrUrl = $"{baseUrl}/Account/Details/{newMember.Id}";
            byte[] qrBytes = GenerateQrCodeBytes(qrUrl);

            newMember.QrCodeValue = qrUrl;
            newMember.QrCodeImage = qrBytes;
            _context.SaveChanges();

            // Remove registration request after approval
            _context.RegistrationRequests.Remove(registration);
            _context.SaveChanges();

            // Remove related notifications
            var relatedNotifs = _context.Notifications
                .Where(n => n.Message.Contains(registration.FullName))
                .ToList();

            if (relatedNotifs.Any())
            {
                _context.Notifications.RemoveRange(relatedNotifs);
                _context.SaveChanges();
            }

            TempData["ActionResult"] = "Approved";
            TempData["MemberName"] = newMember.FullName;

            return RedirectToAction("Admin", "Admin");
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectRegistration(int id)
        {
            var request = _context.RegistrationRequests.FirstOrDefault(r => r.Id == id);
            if (request == null)
            {
                TempData["ActionResult"] = "Failed";
                return RedirectToAction("Admin", "Admin");
            }

            _context.RegistrationRequests.Remove(request);
            _context.SaveChanges();

            var relatedNotifs = _context.Notifications
                .Where(n => n.Message.Contains(request.FullName))
                .ToList();

            if (relatedNotifs.Any())
            {
                _context.Notifications.RemoveRange(relatedNotifs);
                _context.SaveChanges();
            }

            TempData["ActionResult"] = "Rejected";
            TempData["MemberName"] = request.FullName;

            return RedirectToAction("Admin", "Admin");
        }


        [HttpPost]
        public IActionResult AddClassToMember([FromBody] AddClassRequest request)
        {
            var member = _context.Members.FirstOrDefault(m => m.Id == request.MemberId);
            var cls = _context.ClassSchedules.FirstOrDefault(c => c.Id == request.ClassId);

            if (member == null || cls == null)
                return Json(new { success = false, message = "Member or class not found." });

            /* ============================================================
               ✅ CASE 1: Member is currently GymOnly → convert to GymWithClass
               ============================================================ */
            if (member.MembershipPlan == "GymOnly")
            {
                member.MembershipPlan = "GymWithClass";
                member.PreferredClasses = cls.ClassName; // first class added
                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Class added successfully! Plan updated to GymWithClass."
                });
            }

            /* ============================================================
               ✅ CASE 2: Member already has a class plan → add more classes
               ============================================================ */
            if (string.IsNullOrEmpty(member.PreferredClasses))
            {
                member.PreferredClasses = cls.ClassName;
            }
            else
            {
                var existingClasses = member.PreferredClasses.Split(',')
                    .Select(c => c.Trim())
                    .ToList();

                if (existingClasses.Contains(cls.ClassName))
                {
                    return Json(new
                    {
                        success = false,
                        message = "This class is already added for this member."
                    });
                }

                member.PreferredClasses += $", {cls.ClassName}";
            }

            _context.SaveChanges();

            return Json(new { success = true, message = "Class added successfully!" });
        }


        [HttpPost]
        public IActionResult CreateStaff([FromBody] Staff staffData)
        {
            if (string.IsNullOrWhiteSpace(staffData.Username) || string.IsNullOrWhiteSpace(staffData.Password))
                return Json(new { success = false, message = "Username and Password are required." });

            // Check if username already exists in Staff table
            var exists = _context.Staffs.Any(s => s.Username == staffData.Username);
            if (exists)
                return Json(new { success = false, message = "Username already exists." });

            // ✅ Create staff account (with hashed password)
            var staff = new Staff
            {
                FullName = staffData.FullName,
                Username = staffData.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(staffData.Password), // ✅ Hash password here
                Role = "Staff"
            };

            _context.Staffs.Add(staff);
            _context.SaveChanges();

            return Json(new { success = true, message = "Staff account created successfully!" });
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
                TempData["Success"] = $"Staff '{staff.FullName}' removed successfully!";
            }
            else
            {
                TempData["Error"] = "Staff not found or already removed.";
            }

            return RedirectToAction("Admin");
        }

        [HttpGet]
        public IActionResult GetPricingSettings()
        {
            var p = _context.PricingSettings.FirstOrDefault();

            // ✅ Auto-create pricing row if empty (prevents null / 0.00 issues)
            if (p == null)
            {
                p = new PricingSettings
                {
                    GymOnlyPrice = 0,
                    GymWithClassPrice = 0,
                    ClassOnlyPrice = 0,
                    WalkInGymPrice = 0,
                    WalkInClassPrice = 0
                };

                _context.PricingSettings.Add(p);
                _context.SaveChanges();
            }

            return Json(new
            {
                gymOnly = p.GymOnlyPrice,
                gymWithClass = p.GymWithClassPrice,
                classOnly = p.ClassOnlyPrice,
                walkInGym = p.WalkInGymPrice,
                walkInClass = p.WalkInClassPrice
            });
        }



        [HttpPost]
        public IActionResult UpdatePricingSettings([FromBody] PricingSettings updated)
        {
            var p = _context.PricingSettings.FirstOrDefault();

            // ✅ Auto-create if row does not exist
            if (p == null)
            {
                p = new PricingSettings();
                _context.PricingSettings.Add(p);
            }

            // ✅ Assign posted values correctly
            p.GymOnlyPrice = updated.GymOnlyPrice;
            p.GymWithClassPrice = updated.GymWithClassPrice;
            p.ClassOnlyPrice = updated.ClassOnlyPrice;
            p.WalkInGymPrice = updated.WalkInGymPrice;
            p.WalkInClassPrice = updated.WalkInClassPrice;

            _context.SaveChanges();

            return Json(new { success = true });
        }







    }
}
