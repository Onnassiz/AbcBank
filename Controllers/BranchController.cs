using System.Collections.Generic;
using System.Linq;
using AbcBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AbcBank.Controllers
{
    [Authorize(Roles = "Manager")]
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
                        bankBranch, address
                    }
            );

            List<BranchIndexPage> model = new List<BranchIndexPage>();

            foreach (var item in result)
            {
                model.Add(new BranchIndexPage()
                {
                    Id = item.bankBranch.Id,
                    Branch = item.bankBranch.BranchName,
                    SortCode = item.bankBranch.SortCode,
                    Address = item.address.AddressToString
                });
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Address = new SelectList(_context.Addresses.ToList(), "Id", "AddressToString");
            return View();
        }

        [HttpPost]
        public IActionResult Create(BankBranch bankBranch)
        {
            ViewBag.Address = new SelectList(_context.Addresses.ToList(), "Id", "AddressToString");
            if (ModelState.IsValid)
            {
                bankBranch.BranchName = "Abc Bank - " + bankBranch.BranchName;
                _context.BankBranches.Add(bankBranch);
                _context.SaveChanges();
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
            TempData["Response"] = "Branch successfully deleted";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            ViewBag.Address = new SelectList(_context.Addresses.ToList(), "Id", "AddressToString");
            return View(_context.BankBranches.Find(Id));
        }

        [HttpPost]
        public IActionResult Edit(BankBranch bankBranch, string Id)
        {
            ViewBag.Address = new SelectList(_context.Addresses.ToList(), "Id", "AddressToString");
            if (ModelState.IsValid)
            {
                _context.Entry(bankBranch).State = EntityState.Modified;
                _context.SaveChanges();
                TempData["Response"] = "Bank branch successfully updated";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}