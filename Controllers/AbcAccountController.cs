﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbcBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AbcBank.Controllers
{
    [Authorize(Roles = "Manager, Banker")]
    public class AbcAccountController:Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AbcAccountController(MyDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            //Return the list of all created accounts.
            var result = _context.Accounts.Join(_context.BankBranches, x => x.SortCode, y => y.SortCode,
                    (account, branch) => new {account, branch}
            ).OrderByDescending(x => x.account.DateCreated);

            List<AccountJoinModel> model = new List<AccountJoinModel>();
            foreach (var item in result)
            {
                model.Add(new AccountJoinModel()
                {
                    Id = item.account.Id,
                    AccountName = item.account.AccountName,
                    AccountNumber = item.account.AccountNumber,
                    SortCode = item.account.SortCode,
                    Branch = item.branch.BranchName,
                    Descriminator = item.account.Descriminator,
                    IsActive = item.account.IsActive,
                    IsJoint = item.account.IsJoint
                });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.AccountType = new List<string> {"Current", "Savings"};
            var Branch = new SelectList(_context.BankBranches.ToList(), "SortCode", "BranchName");
            ViewBag.Branch = Branch;

            //Restric the list of branches to 1 item if th user is an Admin
            //This is to ensure that Administrators Create Account only for thier branch.
            if (User.IsInRole("Banker"))
            {
                var CurrentUser = await GetUser();
                var BankId = _context.Persons.FirstOrDefault(x => x.Email == CurrentUser.Email).BankBranchId;
                List<SelectListItem> AdminBranch = new List<SelectListItem>();
                var branch = _context.BankBranches.Find(BankId);
                AdminBranch.Add(new SelectListItem{Text = branch.BranchName, Value = branch.SortCode, Selected = true});
                ViewBag.Branch = AdminBranch;
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(Savings savings, Current current)
        {
            ViewBag.AccountType = new List<string> {"Current", "Savings"};
            var Branch = new SelectList(_context.BankBranches.ToList(), "SortCode", "BranchName");
            ViewBag.Branch = Branch;

            if (ModelState.IsValid)
            {
                if (savings.Descriminator == "Savings")
                {
                    savings.MonthlyCount = 0;
                    savings.Balance = 0.00;
                    savings.DateCreated = DateTime.Now;
                    savings.DailyIn = 0.00;
                    _context.Accounts.Add(savings);
                    TempData["ResponseSuccess"] = "/AbcAccount/Setting/" + savings.Id;
                }
                else
                {
                    current.OverDraft = 0;
                    current.Balance = 0.00;
                    current.DateCreated = DateTime.Now;
                    current.DailyIn = 0.00;
                    _context.Accounts.Add(current);
                    TempData["ResponseSuccess"] = "/AbcAccount/Settings/" + current.Id;
                }
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Delete(string Id)
        {
            _context.Accounts.Remove(_context.Accounts.Find(Id));
            _context.SaveChanges();
            TempData["Response"] = "Account Successfully Deleted";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            var Branch = new SelectList(_context.BankBranches.ToList(), "SortCode", "BranchName");
            ViewBag.Branch = Branch;

            //Restric the list of branches to 1 item if th user is an Admin
            //This is to ensure that Administrators update Account only for thier branch.
            if (User.IsInRole("Banker"))
            {
                var CurrentUser = await GetUser();
                var BankId = _context.Persons.FirstOrDefault(x => x.Email == CurrentUser.Email).BankBranchId;
                List<SelectListItem> AdminBranch = new List<SelectListItem>();
                var branch = _context.BankBranches.Find(BankId);
                AdminBranch.Add(new SelectListItem{Text = branch.BranchName, Value = branch.SortCode, Selected = true});
                ViewBag.Branch = AdminBranch;
            }
            return View(_context.Accounts.Find(Id));
        }

        [HttpPost]
        public IActionResult Edit(Account account, string Id)
        {
            var Branch = new SelectList(_context.BankBranches.ToList(), "SortCode", "BranchName");
            ViewBag.Branch = Branch;

            if (ModelState.IsValid)
            {
                _context.Entry(account).State = EntityState.Modified;
                _context.SaveChanges();
                TempData["Response"] = "Account Record Updated";
                return RedirectToAction("Index");
            }

            return View(_context.Accounts.Find(Id));
        }

        public IActionResult Settings(string Id)
        {
            var currentAcc = _context.Accounts.OfType<Current>().FirstOrDefault(x => x.Id == Id);
            var savingsAcc = _context.Accounts.OfType<Savings>().FirstOrDefault(x => x.Id == Id);

            if (currentAcc != null)
            {
                ViewBag.Overdraft = currentAcc.OverDraft;
            }
            else
            {
                ViewBag.MonthlyCount = savingsAcc.MonthlyCount;
            }
            //Get all the holders tagged to an account and return it to the view
            ViewBag.Holders = GetHolders(Id);

            var result = _context.Accounts.Join(_context.BankBranches, x => x.SortCode, y => y.SortCode,
                    (account, branch) => new {account, branch}
                ).FirstOrDefault(x => x.account.Id == Id);

            Current current = new Current();
            Savings savings = new Savings();

            var model = new AccountJoinModel
            {
                Id = result.account.Id,
                AccountName = result.account.AccountName,
                AccountNumber = result.account.AccountNumber,
                SortCode = result.account.SortCode,
                Branch = result.branch.BranchName,
                Descriminator = result.account.Descriminator,
                IsActive = result.account.IsActive,
                IsJoint = result.account.IsJoint,
                OverdraftLimit = current.OverDraftLimit,
                OverdraftInterestRate = current.OverdraftInterestRate,
                MonthlyLimit = savings.MonthlyLimit,
                InterestRate = savings.InterestRate,
                Balance = result.account.Balance,
                DailyOut = result.account.DailyOut
            };
            return View(model);
        }

        public IActionResult AddHolder(string Id)
        {
            var account = _context.Accounts.Find(Id);
            ViewBag.Id = account.Id;
            ViewBag.AccountName = account.AccountName;
            ViewBag.AccountNumber = account.AccountNumber;

//            var BranchId = _context.BankBranches.FirstOrDefault(x => x.SortCode == account.SortCode).Id;
            ViewBag.Persons = new SelectList(_context.Persons.Where(x => x.Descriminator == "Customer"), "Id", "FullName");

            return View();
        }

        [HttpPost]
        public IActionResult AddHolder(AccountHolder accountHolder, string Id)
        {
            accountHolder.Id = null;
            if (!HolderExist(accountHolder.AccountId, accountHolder.PersonId))
            {
                if (IsJoint(Id))
                {
                    var holderCount = _context.AccountHolders.Count(x => x.AccountId == Id);
                    var account = new Account();
                    //Ensure holders don't exceed limit
                    if (holderCount >= account.JointHolderLimit)
                    {
                        TempData["Error"] = "Holders' limit exceeded. More than 3 holders cannot be tagged to an account.";
                        return RedirectToAction("Settings", new {id = Id});
                    }
                    _context.AccountHolders.Add(accountHolder);
                    _context.SaveChanges();
                    TempData["Response"] = "A holder has been successfully added to this account";
                    return RedirectToAction("Settings", new {id = Id});
                }
                if (HasHolder(accountHolder.AccountId))
                {
                    TempData["Error"] = "Account already has a holder";
                    return RedirectToAction("Settings", new {id = Id});
                }
                _context.AccountHolders.Add(accountHolder);
                _context.SaveChanges();
                TempData["Response"] = "A holder has been successfully added to this account";
                return RedirectToAction("Settings", new {id = Id});
            }
            TempData["Error"] = "This holder has already been mapped to this account";
            return RedirectToAction("Settings", new {id = Id});
        }

        private bool IsJoint(string Id)
        {
            if (_context.Accounts.Find(Id).IsJoint)
            {
                return true;
            }
            return false;
        }

        private bool HolderExist(string accountId, string personId)
        {
            if (_context.AccountHolders.Any(x => x.AccountId == accountId && x.PersonId == personId))
            {
                return true;
            }
            return false;
        }

        public bool HasHolder(string accountId)
        {
            if (_context.AccountHolders.Any(x => x.AccountId == accountId))
            {
                return true;
            }
            return false;
        }

        public List<string> GetHolders(string accountId)
        {
            List<string> Holders = new List<string>();

            var personIds = _context.AccountHolders.Where(x => x.AccountId == accountId)
                .Join(_context.Persons, b => b.PersonId, j => j.Id,
                (accountHolder, person) => new{accountHolder, person});

            foreach (var item in personIds)
            {
                Holders.Add(item.person.FullName);
            }

            return Holders;
        }

        public IActionResult Activate(string Id)
        {
            if (!HasHolder(Id))
            {
                TempData["Error"] = "Cannot activated an account without a holder. Set account holder first";
                return RedirectToAction("Settings", new {id = Id});
            }
            Account account = _context.Accounts.Find(Id);
            account.IsActive = true;
            _context.Accounts.Update(account);
            _context.SaveChanges();
            TempData["Response"] = "Account activated";
            return RedirectToAction("Settings", new {id = Id});
        }

        public IActionResult Close(string Id)
        {
            Account account = _context.Accounts.Find(Id);
            account.IsActive = false;
            _context.Accounts.Update(account);
            _context.SaveChanges();
            TempData["Response"] = "Account closed.";
            return RedirectToAction("Settings", new {id = Id});
        }

        public Task<ApplicationUser> GetUser()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
