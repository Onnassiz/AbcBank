using System;
using System.Linq;
using System.Threading.Tasks;
using AbcBank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
            return View();
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