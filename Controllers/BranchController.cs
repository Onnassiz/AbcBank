using System;
using System.Collections.Generic;
using System.Linq;
using AbcBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AbcBank.Controllers
{
    [Authorize(Roles = "Manager, Banker")]
    public class BranchController : Controller
    {
        private readonly MyDbContext _context;

        public BranchController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var result = _context.BankBranches.Join(_context.Addresses, e => e.AddressId, d => d.Id,
                    (bankBranch, address) => new
                    {
                        Id = bankBranch.Id,
                        SortCode = bankBranch.SortCode,
                        BankBranch = bankBranch.BranchName,
                        BankAddress = address.ToString
                    }
            );

            List<BranchIndexPage> model = new List<BranchIndexPage>();

            foreach (var item in result)
            {
                model.Add(new BranchIndexPage()
                {
                    Id = item.Id,
                    Branch = item.BankBranch,
                    SortCode = item.SortCode,
                    Address = item.BankAddress
                });
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Address = new SelectList(_context.Addresses.ToList(), "Id", "ToString");
            return View();
        }

        [HttpPost]
        public IActionResult Create(BankBranch bankBranch)
        {
            ViewBag.Address = new SelectList(_context.Addresses.ToList(), "Id", "ToString");
            if (ModelState.IsValid)
            {
                bankBranch.BranchName = "Abc Bank - " + bankBranch.BranchName;
                _context.BankBranches.Add(bankBranch);
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException as PostgresException;
                    if (innerException != null && innerException.Code == "23505")
                    {
                        ModelState.AddModelError(string.Empty, "Bank branch or sort code already in use");
                        return View();
                    }
                }
                TempData["Response"] = "Bank branch successfully created";
                return RedirectToAction("Index");
            }
            return View();
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Delete(string Id)
        {
            _context.BankBranches.Remove(_context.BankBranches.Find(Id));
            _context.SaveChanges();
            TempData["Response"] = "Branch Successfully Deleted";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            ViewBag.Address = new SelectList(_context.Addresses.ToList(), "Id", "ToString");
            return View(_context.BankBranches.Find(Id));
        }

        [HttpPost]
        public IActionResult Edit(BankBranch bankBranch, string Id)
        {
            ViewBag.Address = new SelectList(_context.Addresses.ToList(), "Id", "ToString");
            if (ModelState.IsValid)
            {
                _context.BankBranches.Update(bankBranch);
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException as PostgresException;
                    if (innerException != null && innerException.Code == "23505")
                    {
                        ModelState.AddModelError(string.Empty, "Bank branch or sort code already in use");
                        return View();
                    }
                }
                TempData["Response"] = "Bank branch successfully updated";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}