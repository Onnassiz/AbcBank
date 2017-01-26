using System.Linq;
using AbcBank.Models;
using Microsoft.AspNetCore.Mvc;

namespace AbcBank.Controllers
{
    public class TransferController : Controller
    {
        private MyDbContext _context;

        public TransferController(MyDbContext context)
        {
            _context = context;
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
                var transactionController = new TransactionController(_context);
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

            }
            var abcAccountController = new AbcAccountController(_context);
            var reciever = _context.Accounts.Find(id2);
            ViewBag.ReciverAccountName = reciever.AccountName;
            ViewBag.ReciverAccountNumber = reciever.AccountNumber;
            ViewBag.Id2 = reciever.Id;
            ViewBag.Holders = abcAccountController.GetHolders(id);
            return View(_context.Accounts.Find(id));
        }
    }
}