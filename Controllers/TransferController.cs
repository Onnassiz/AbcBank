using System;
using System.Linq;
using System.Threading.Tasks;
using AbcBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AbcBank.Controllers
{
    [Authorize(Roles = "Manager, Manager")]
    public class TransferController : Controller
    {
        private MyDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransferController(MyDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Transfer transfer)
        {
            if (ModelState.IsValid)
            {
                var transactionController = new TransactionController(_context, _userManager);
                var fromId = _context.Accounts.FirstOrDefault(x => x.AccountNumber == transfer.PrincipalAccount).Id;
                var toId = _context.Accounts.FirstOrDefault(x => x.AccountNumber == transfer.ReceivingAccount).Id;
                if (transactionController.IsSavings(fromId))
                {
                    ModelState.AddModelError(string.Empty, "Transfer operation is not allowed on savings account");
                    return View();
                }
                return RedirectToAction("FormatUrl", new {id = fromId, id2 = toId});
            }
            return View();
        }

        public IActionResult FormatUrl(string Id, string Id2, string Amount)
        {
            return Amount == null
                ? Redirect("/Transfer/Target/" + Id + "/" + Id2)
                : Redirect("/Transfer/Target/" + Id + "/" + Id2 + "/" + Amount);
        }

        public IActionResult Target(string id, string id2, string Id3)
        {
            if (Id3 != null)
            {
                double Amount;
                var parse = Double.TryParse(Id3, out Amount);
                if (parse)
                {
                    return RedirectToAction("Task", new {sender = id, receiver = id2, amount = Amount});
                }
                TempData["Error"] = "The value you supplied is out of format";
            }
            var abcAccountController = new AbcAccountController(_context, _userManager);
            var receiver = _context.Accounts.Find(id2);
            ViewBag.ReciverAccountName = receiver.AccountName;
            ViewBag.ReciverAccountNumber = receiver.AccountNumber;
            ViewBag.Id2 = receiver.Id;
            ViewBag.Holders = abcAccountController.GetHolders(id);
            return View(_context.Accounts.Find(id));
        }

        public async Task<IActionResult> Task(string Sender, string Receiver, double Amount)
        {
            var account = new Account();
            var debitController = new DebitController(_context, _userManager);
            var creditController = new CreditController(_context, _userManager);
            var transactionController = new TransactionController(_context, _userManager);
            if (Amount > account.DailyLimit)
            {
                TempData["Error"] = "Maximal transfet limit exceeded";
                return Redirect("/Transfer/Target/" + Sender + "/" + Receiver);
            }
            if (!debitController.DebitCurrent(Sender, Amount))
            {
                TempData["Error"] = "Insufficient Balance";
                return Redirect("/Transfer/Target/" + Sender + "/" + Receiver);
            }
            if (transactionController.IsSavings(Receiver))
            {
                creditController.CreditSavings(Receiver, Amount);
            }
            else
            {
                creditController.CreditCurrent(Receiver, Amount);
            }
            var senderTransactionId = await AddTransaction( "Bank Transfer", Sender, "Debit", Amount);
            var receiverTransactionId = await AddTransaction( _context.Accounts.Find(Sender).AccountName, Sender, "Credit", Amount);
            TempData["ResponseTransfer"] = "/Transaction/Transfer/" + senderTransactionId + "/" + receiverTransactionId;
            return RedirectToAction("Index", "Transaction");
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
                Description = "Account "+ Type + ". " + From,
                From = From
            };
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
            return transaction.Id;
        }
    }
}