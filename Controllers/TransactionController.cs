using System.Collections.Generic;
using System.Linq;
using AbcBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbcBank.Controllers
{
    [Authorize(Roles = "Manager, Banker, Customer")]
    public class TransactionController : Controller
    {
        private readonly MyDbContext _context;
        public TransactionController(MyDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Manager, Banker")]
        public IActionResult Index()
        {
            var result = _context.Transactions.Join(_context.Accounts, x => x.AccountId, j => j.Id,
                    (transaction, account) => new {transaction, account}
            ).Join(_context.Persons, a => a.transaction.PersonId, a => a.Id,
                    (transaction, person) => new {transaction, person}
            ).OrderByDescending(x => x.transaction.transaction.DateCreated);

            List<TransactionJoinView> model = new List<TransactionJoinView>();

            foreach (var item in result)
            {
                model.Add(new TransactionJoinView
                {
                    Id = item.transaction.transaction.Id,
                    DateCreated = item.transaction.transaction.DateCreated,
                    AccountName = item.transaction.account.AccountName,
                    AccountNumber = item.transaction.account.AccountNumber,
                    Amount = item.transaction.transaction.Amount,
                    From = item.transaction.transaction.From,
                    Type = item.transaction.transaction.Type,
                    Personnel = item.person.FullName
                });
            }
            return View(model);
        }

        public IActionResult View(string Id)
        {
            var item = _context.Transactions.Join(_context.Accounts, x => x.AccountId, j => j.Id,
                (transaction, account) => new {transaction, account}
            ).Join(_context.Persons, a => a.transaction.PersonId, a => a.Id,
                (transaction, person) => new {transaction, person}
            ).FirstOrDefault(x => x.transaction.transaction.Id == Id);

            TransactionJoinView model = new TransactionJoinView
            {
                Id = item.transaction.transaction.Id,
                DateCreated = item.transaction.transaction.DateCreated,
                AccountName = item.transaction.account.AccountName,
                AccountNumber = item.transaction.account.AccountNumber,
                Amount = item.transaction.transaction.Amount,
                From = item.transaction.transaction.From,
                Type = item.transaction.transaction.Type,
                Personnel = item.person.FullName,
                Description = item.transaction.transaction.Description
            };

            return View(model);
        }

        public bool IsSavings(string AccountId)
        {
            return _context.Accounts.Find(AccountId).Descriminator == "Savings";
        }

        [Authorize(Roles = "Manager")]
        public IActionResult ResetSavings()
        {
            var savings = _context.Accounts.OfType<Savings>();
            foreach (var item in savings)
            {
                item.MonthlyCount = 0;
                _context.Accounts.Update(item);
            }
            _context.SaveChanges();
            TempData["Response"] = "All savings monthly withdrawal counts have been reset.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Manager")]
        public IActionResult ResetDailyAmount()
        {
            var accounts = _context.Accounts.ToList();
            foreach (var item in accounts)
            {
                item.DailyOut = 0.00;
                _context.Accounts.Update(item);
            }
            _context.SaveChanges();
            TempData["Response"] = "Daily withdrawal sums have been reset.";
            return RedirectToAction("Index");
        }

        public IActionResult Transfer(string Id, string Id2)
        {
            var item = _context.Transactions.Join(_context.Accounts, x => x.AccountId, j => j.Id,
                (transaction, account) => new {transaction, account}
            ).Join(_context.Persons, a => a.transaction.PersonId, a => a.Id,
                (transaction, person) => new {transaction, person}
            ).FirstOrDefault(x => x.transaction.transaction.Id == Id);

            var receiver = _context.Accounts.Find(_context.Transactions.Find(Id2).AccountId);

            TransactionJoinView model = new TransactionJoinView
            {
                Id = item.transaction.transaction.Id,
                DateCreated = item.transaction.transaction.DateCreated,
                AccountName = item.transaction.account.AccountName,
                AccountNumber = item.transaction.account.AccountNumber,
                Amount = item.transaction.transaction.Amount,
                From = item.transaction.transaction.From,
                Type = item.transaction.transaction.Type,
                Personnel = item.person.FullName,
                Description = item.transaction.transaction.Description,
                Receiver = receiver.AccountName,
                ReceiverAccountNumber = receiver.AccountNumber
            };

            return View(model);
        }
    }
}