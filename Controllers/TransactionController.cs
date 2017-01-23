using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbcBank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AbcBank.Controllers
{
    public class TransactionController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public TransactionController(MyDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

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

        public async Task AddTransaction(string From, string AccountId, string Type, double Amount, string Description)
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
                Description = Description,
                From = From
            };
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
    }
}