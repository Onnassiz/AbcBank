using System.Collections.Generic;
using System.Threading.Tasks;
using AbcBank.Models;
using Microsoft.AspNetCore.Authorization;
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

        public IActionResult Test(string CardNumber)
        {
//            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
//            {
//                return Json("Wath");
//            }
            var referrer = Request.Headers["Referer"].ToString();
            var current = MyMethod(HttpContext);
            Dictionary<string,string> response = new Dictionary<string, string>();

            response["status"] = "pass";
            response["token"] = "data";
            return Json(response);
        }


        public string MyMethod(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var host = $"{context.Request.Scheme}://{context.Request.Host}";
            return host;
        }
    }
}
