using System;
using System.Linq;
using System.Threading.Tasks;
using AbcBank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AbcBank.Controllers
{
    public class CreditController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CreditController(MyDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        public IActionResult Target(string Id, string From, string Amount)
        {
            if (Amount != null && From != null )
            {
                double Number;
                bool parse = Double.TryParse(Amount, out Number);
                if (parse)
                {
                    return RedirectToAction("Task", new {id = Id, amount = Number, from = From});
                }
                TempData["Error"] = "The value you supplied is out of format";
                return RedirectToAction("Target", new {id = Id});
            }
            AbcAccountController abcAccountController = new AbcAccountController(_context);
            ViewBag.Holders = abcAccountController.GetHolders(Id);
            return View(_context.Accounts.Find(Id));
        }

        public async Task<IActionResult> Task(string Id, string From, double Amount)
        {
            var transId = await AddTransaction(
                    From,
                    Id,
                    "Credit",
                    Amount
            );
            if (IsSavings(Id))
            {
                CreditSavings(Id, Amount);
            }
            CreditCurrent(Id, Amount);
            var href = "Transaction/View/" + transId;
            TempData["Response"] = "Credit operation successful. To view transaction details, click <a href='" + href + "' >here<a/>";
            return RedirectToAction("Index", "Transaction");
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

        public async Task<string> AddTransaction(string From, string AccountId, string Type, double Amount)
        {
            var User = await _userManager.GetUserAsync(HttpContext.User);
            var person = _context.Persons.FirstOrDefault(x => x.Email == User.Email);
            Transaction transaction = new Transaction
            {
                DateCreated = DateTime.Now,
                Type = Type,
                Amount = Amount,
                AccountId = AccountId,
                PersonId = person.Id,
                Description = "Account Credited by " + From,
                From = From
            };
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
            return transaction.Id;
        }
    }
}