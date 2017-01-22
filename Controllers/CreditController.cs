using System;
using System.Linq;
using AbcBank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AbcBank.Controllers
{
    public class CreditController : Controller
    {
        private readonly MyDbContext _context;

        public CreditController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string Id = null)
        {
            if (Id != null)
            {
                return RedirectToAction("Target", new {id = Id});
            }
            ViewBag.Accounts = new SelectList(_context.Accounts.ToList(), "Id", "AccountName");
            return View();
        }

        public IActionResult Target(string Id, string Amount)
        {
            if (Amount != null)
            {
                double Number;
                bool parse = Double.TryParse(Amount, out Number);
                if (parse)
                {
                    return RedirectToAction("Task", new {id = Id, amount = Number});
                }
                TempData["Error"] = "The value you supplied is out of format";
                return RedirectToAction("Target", new {id = Id});
            }
            AbcAccountController abcAccountController = new AbcAccountController(_context);
            ViewBag.Holders = abcAccountController.GetHolders(Id);
            return View(_context.Accounts.Find(Id));
        }

        public IActionResult Task(string Id, double Amount)
        {
            if (IsSavings(Id))
            {
                CreditSavings(Id, Amount);
                TempData["Error"] = "Credit operation successful";
                return RedirectToAction("Target", new {id = Id});
            }
            CreditCurrent(Id, Amount);
            TempData["Error"] = "Credit operation successful";
            return RedirectToAction("Target", new {id = Id});
        }

        public void CreditSavings(string AccountId, double Amount)
        {
            Account account = _context.Accounts.Find(AccountId);
            account.Balance += Amount;
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public void CreditCurrent(string AccountId, double Amount)
        {
            Account account = _context.Accounts.Find(AccountId);
            account.Balance += Amount;
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public bool IsSavings(string AccountId)
        {
            return _context.Accounts.Find(AccountId).Descriminator == "Savings";
        }
    }
}