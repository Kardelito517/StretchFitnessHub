using Microsoft.AspNetCore.Mvc;

namespace StretchFitnessHub.Controllers
{
    public class MemDash : Controller
    {
        public IActionResult Member()
        {
            return View("Member");
        }
    }
}
