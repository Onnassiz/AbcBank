using System;
using System.Linq;
using AbcBank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AbcBank.Controllers
{
    public class DebitController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DebitController(MyDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index(string Id)
        {
            if (Id != null)
            {
                return RedirectToAction("Target", new {id = Id});
            }
            ViewBag.Accounts = new SelectList(_context.Accounts.ToList(), "Id", "AccountName");
            return View();
        }

        public IActionResult Target(string Id, string From, string Amount)
        {
            if (Amount != null && From != null )
            {
                double Number;
                bool parse = Double.TryParse(Amount, out Number);
                if (parse)
                {
//                    return RedirectToAction("Task", new {id = Id, amount = Number, from = From});
                }
                TempData["Error"] = "The value you supplied is out of format";
                return RedirectToAction("Target", new {id = Id});
            }
            AbcAccountController abcAccountController = new AbcAccountController(_context);
            ViewBag.Holders = abcAccountController.GetHolders(Id);
            return View(_context.Accounts.Find(Id));
        }
    }
}