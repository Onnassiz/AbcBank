using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbcBank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Internal.Networking;

namespace AbcBank.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}