using System.Threading.Tasks;
using AbcBank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AbcBank.Controllers
{
    public class SampleController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SampleController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test(string Name)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json("Wath");
            }
            return Json(Name);
        }
    }
}
