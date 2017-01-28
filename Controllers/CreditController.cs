using System;
using System.Linq;
using System.Threading.Tasks;
using AbcBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AbcBank.Controllers
{
    [Authorize(Roles = "Manager, Banker")]
    public class CreditController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CreditController(MyDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Find(Account account)
        {
            var result = _context.Accounts.FirstOrDefault(x => x.AccountNumber == account.AccountNumber);
            string type = Request.Form["Type"].ToString();
            if (result == null)
            {
                TempData["Error"] = "Account not found";
            }
            if (type == "Credit")
            {
                return result != null? RedirectToAction("Target", new {id = result.Id}) : RedirectToAction("Index");
            }
            return result != null? RedirectToAction("Target", "Debit", new {id = result.Id}) : RedirectToAction("Index", "Debit");
        }

        public IActionResult Target(string Id, string From, string Amount)
        {
            var abcAccountController = new AbcAccountController(_context);
            ViewBag.Holders = abcAccountController.GetHolders(Id);
            if (Amount != null && From != null )
            {
                double Number;
                var parse = Double.TryParse(Amount, out Number);

                if (!abcAccountController.HasHolder(Id))
                {
                    TempData["Error"] = "No holder was found on this account Number";
                    return RedirectToAction("Target", new {id = Id});
                }

                if (parse)
                {
                    Number = Math.Round(Number, 2);
                    return RedirectToAction("Task", new {id = Id, amount = Number, from = From});
                }
                TempData["Error"] = "The value you supplied is out of format";
                return RedirectToAction("Target", new {id = Id});
            }
            return View(_context.Accounts.Find(Id));
        }

        public async Task<IActionResult> Task(string Id, string From, double Amount)
        {
            var transId = await AddTransaction( From, Id, "Credit", Amount);
            var transaction = new TransactionController(_context, _userManager);
            if (transaction.IsSavings(Id))
            {
                CreditSavings(Id, Amount);
            }
            else
            {
                CreditCurrent(Id, Amount);
            }

            var href = "Transaction/View/" + transId;
            TempData["ResponseCredit"] = href;
            return RedirectToAction("Index", "Transaction");
        }

        public void CreditSavings(string AccountId, double Amount)
        {
            var account = _context.Accounts.Find(AccountId);
            account.Balance += Amount;
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public void CreditCurrent(string AccountId, double Amount)
        {
            var account = _context.Accounts.OfType<Current>().FirstOrDefault(x => x.Id == AccountId);
            account.Balance += Amount;
            account.OverDraft = account.Balance + Amount > 0 ? 0 : account.OverDraft;
            _context.Accounts.Update(account);
            _context.SaveChanges();
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