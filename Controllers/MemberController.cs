using Microsoft.AspNetCore.Mvc;
using QRCoder;
using StretchFitnessHub.Data;
using StretchFitnessHub.Models;
using StretchFitnessHub.ViewModels;
using System.IO;

namespace StretchFitnessHub.Controllers
{
    public class MemberController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemberController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? memberId = HttpContext.Session.GetInt32("MemberId");

            if (memberId == null)
            {
                return RedirectToAction("LandingPage", "Account");
            }

            var member = _context.Members.FirstOrDefault(m => m.Id == memberId);

            if (member == null)
            {
                return RedirectToAction("LandingPage", "Account");
            }

            string? qrBase64 = null;
            if (member.QrCodeImage != null)
            {
                qrBase64 = $"data:image/png;base64,{Convert.ToBase64String(member.QrCodeImage)}";
            }

            var viewModel = new MemberDashboardViewModel
            {
                MemberId = member.Id,
                FullName = member.FullName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                MembershipStart = member.MembershipStart,
                MembershipEnd = member.MembershipEnd,
                ProfileImagePath = member.ProfileImagePath,
                QrCodeImage = qrBase64
            };

            return View("~/Views/Account/Member.cshtml", viewModel);
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LandingPage", "Account");
        }

        [HttpPost]
        public IActionResult UploadProfileImage(IFormFile profileImage)
        {
            int? memberId = HttpContext.Session.GetInt32("MemberId");
            if (memberId == null) return RedirectToAction("LandingPage", "Account");

            Member? member = _context.Members.FirstOrDefault(m => m.Id == memberId);
            if (member == null) return RedirectToAction("LandingPage", "Account");

            if (profileImage != null && profileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    profileImage.CopyTo(fileStream);
                }

                member.ProfileImagePath = "/uploads/" + uniqueFileName;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GenerateQRCode(int memberId)
        {
            var member = _context.Members.FirstOrDefault(m => m.Id == memberId);
            if (member == null)
            {
                return NotFound();
            }

            string baseUrl = "http://192.168.100.80:5000";
            string memberUrl = $"{baseUrl}/Member/Details/{member.Id}";

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(memberUrl, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeBytes = qrCode.GetGraphic(20);

                return File(qrCodeBytes, "image/png");
            }
        }



        [HttpGet]
        public IActionResult Details(int id)
        {
            var member = _context.Members.FirstOrDefault(m => m.Id == id);
            if (member == null)
            {
                return NotFound("Member not found.");
            }

            return View("~/Views/Account/Details.cshtml", member);
        }




        [HttpGet]
        public IActionResult DownloadQr(int id)
        {
            Member? member = _context.Members.FirstOrDefault(m => m.Id == id);
            if (member == null || member.QrCodeImage == null)
            {
                return NotFound();
            }

            return File(member.QrCodeImage, "image/png", $"{member.FullName}_QRCode.png");
        }

        [HttpGet]
        public IActionResult GetMemberByQRCode(string qr)
        {
            var member = _context.Members
                .FirstOrDefault(m => m.QrCodeValue == qr);

            if (member == null)
                return Json(new { error = "Member not found" });

            return Json(new
            {
                name = member.FullName,
                startDate = member.MembershipStart.ToString("yyyy-MM-dd"),
                endDate = member.MembershipEnd.ToString("yyyy-MM-dd")
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile([FromForm] string FullName, [FromForm] string Email, [FromForm] string PhoneNumber, [FromForm] string Password)
        {
            int? userId = HttpContext.Session.GetInt32("MemberId");
            if (userId == null) return RedirectToAction("LandingPage", "Account");

            var member = await _context.Members.FindAsync(userId.Value);
            if (member == null) return RedirectToAction("LandingPage", "Account");

            member.FullName = FullName;
            member.Email = Email;
            member.PhoneNumber = PhoneNumber;
            member.Password = Password; 

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
