﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbcBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace AbcBank.Controllers
{
    [Authorize(Roles = "Manager, Banker")]
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

        public IActionResult Target(string Id, string Amount, string From = "Self")
        {
            if (Amount != null && From != null )
            {
                double Number;
                var parse = Double.TryParse(Amount, out Number);

                if (!_context.Accounts.Find(Id).IsActive)
                {
                    TempData["Error"] = "Error. Accout is Closed.";
                    return RedirectToAction("Target", new {id = Id});
                }
                if (parse)
                {
                    if (ExceedsDailyLimit(Id, Number))
                    {
                        TempData["Error"] = "Daily withdrawal limit exceeded.";
                        return RedirectToAction("Target", new {id = Id});
                    }
                    return RedirectToAction("Task", new {id = Id, amount = Number, from = From});
                }
                TempData["Error"] = "The value you supplied is out of format";
                return RedirectToAction("Target", new {id = Id});
            }
            var abcAccountController = new AbcAccountController(_context, _userManager);
            ViewBag.Holders = abcAccountController.GetHolders(Id);
            return View(_context.Accounts.Find(Id));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Task(string Id, string From, double Amount)
        {
            var transactionController = new TransactionController(_context, _userManager);
            var referrer = Request.Headers["Referer"].ToString();
            var current = MyMethod(HttpContext);
            var thisUrl = referrer.Contains(current);
            var response = new Dictionary<string, string>();

            if (transactionController.IsSavings(Id))
            {
                if (OverMonthlyCount(Id))
                {
                    TempData["Error"] = "Monthly withdrawal limit exceeded.";
                    return RedirectToAction("Target", new {id = Id});
                }
                if (!DebitSavings(Id, Amount))
                {
                    TempData["Error"] = "Insufficient Balance";
                    return RedirectToAction("Target", new {id = Id});
                }
                response.Add("status", "fail");
                response.Add("token", "Sav Balance");
                return Json(JsonConvert.SerializeObject(response));
            }
            else
            {
                if (!DebitCurrent(Id, Amount))
                {
                    if (thisUrl)
                    {
                        response.Add("status", "fail");
                        response.Add("token", "Insufficient Balance");
                        return Json(JsonConvert.SerializeObject(response));
                    }
                    TempData["Error"] = "Insufficient Balance";
                    return RedirectToAction("Target", new {id = Id});
                }
            }
            if (thisUrl)
            {
                await AddExternalTransaction( From, Id, "Debit", Amount);
                response.Add("status", "pass");
                response.Add("token", "Tansaction Complete. Please take your cash");
                return Json(JsonConvert.SerializeObject(response));
            }
            var transId = await AddTransaction( From, Id, "Debit", Amount);
            var href = "Transaction/View/" + transId;
            TempData["ResponseDebit"] = href;
            return RedirectToAction("Index", "Transaction");
        }

        private bool DebitSavings(string Id, double Amount)
        {
            var account = _context.Accounts.OfType<Savings>().FirstOrDefault(x => x.Id == Id);
            if (SufficientBalance(Id, Amount))
            {
                account.Balance = Math.Round(account.Balance - Amount, 2);
                account.MonthlyCount += 1;
                account.DailyOut += Amount;
                _context.Accounts.Update(account);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DebitCurrent(string Id, double Amount)
        {
            if (!HasOverdraft(Id) && SufficientBalance(Id, Amount))
            {
                var account = _context.Accounts.OfType<Current>().FirstOrDefault(x => x.Id == Id);
                var balance = account.Balance - Amount;
                account.OverDraft = balance < 0 ? balance * -1 : 0;
                account.Balance = balance < 0
                    ? Math.Round(0 - ((balance * -1 * account.OverdraftInterestRate) / 100 + (balance * -1)), 2)
                    : Math.Round(account.Balance - Amount, 2);
                account.DailyOut += Amount;
                _context.Accounts.Update(account);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        private bool SufficientBalance(string Id, double Amount)
        {
            var transactionController = new TransactionController(_context, _userManager);
            var balance = _context.Accounts.Find(Id).Balance;
            if (transactionController.IsSavings(Id))
            {
                return balance - Amount >= 0;
            }
            var current = new Current();
            return balance - Amount >= -current.OverDraftLimit;
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
                Description = "Account Debited by " + From,
                From = From
            };
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
            return transaction.Id;
        }
        public async Task<string> AddExternalTransaction(string From, string AccountId, string Type, double Amount)
        {
            var personId = _context.AccountHolders.FirstOrDefault(x => x.AccountId == AccountId).PersonId;
            Transaction transaction = new Transaction
            {
                DateCreated = DateTime.Now,
                Type = Type,
                Amount = Amount,
                AccountId = AccountId,
                PersonId = personId,
                Description = "Account Debited by " + From,
                From = From
            };
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
            return transaction.Id;
        }

        private bool OverMonthlyCount(string Id)
        {
            var savings = new Savings();
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var count = _context.Transactions.Count(x => x.DateCreated >= startDate && x.DateCreated <= endDate && x.Type == "Debit" && x.AccountId == Id);
            return count >= savings.MonthlyLimit;
        }

        private bool ExceedsDailyLimit(string Id, double Amount)
        {
            var account = _context.Accounts.Find(Id);
            return account.DailyOut + Amount > account.DailyLimit;
        }

        private bool HasOverdraft(string Id)
        {
            return _context.Accounts.Find(Id).Balance < 0;
        }

        [AllowAnonymous]
        public IActionResult GetExternalRequest(string Id, string Id2)
        {
            var referrer = Request.Headers["Referer"].ToString();
            var current = MyMethod(HttpContext);
            var thisUrl = referrer.Contains(current);

            if (!thisUrl)
            {
                var response = new Dictionary<string, string>();
                var card = _context.Cards.FirstOrDefault(x => x.Token == Id);
                if (card != null)
                {
                    var accountId = card.AccountId;
                    double Number;
                    var parse = Double.TryParse(Id2, out Number);
                    if (!_context.Accounts.Find(accountId).IsActive)
                    {
                        response.Add("status", "fail");
                        response.Add("token", "Account is closed. Contact Admin");
                        return Json(JsonConvert.SerializeObject(response));
                    }
                    if (parse)
                    {
                        if (ExceedsDailyLimit(accountId, Number))
                        {
                            response.Add("status", "fail");
                            response.Add("token", "Daily withdrawal limit exceeded");
                            return Json(JsonConvert.SerializeObject(response));
                        }
                        card.Token = "";
                        _context.Cards.Update(card);
                        _context.SaveChanges();
                        response.Add("status", "fail");
                        response.Add("token", _context.Accounts.Find(accountId).Balance.ToString());
                        return RedirectToAction("Task", new {id = accountId, amount = Number, from = "ATM Withdrawal"});
                    }
                    response.Add("status", "fail");
                    response.Add("token", "Out of format.");
                    return Json(JsonConvert.SerializeObject(response));
                }
            }
            return Json(JsonConvert.SerializeObject(""));
        }

        public string MyMethod(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var host = $"{context.Request.Scheme}://{context.Request.Host}";
            return host;
        }
    }
}
