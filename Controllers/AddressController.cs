using System;
using System.Linq;
using AbcBank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AbcBank.Controllers
{
    public class AddressController : Controller
    {
        private readonly MyDbContext _context;

        public AddressController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Addresses.OrderBy(i => i.DateCreated));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Address address, string Id)
        {
            if (ModelState.IsValid)
            {
                var ToString = address.HouseNumber + " " + address.Street + ", " + address.Town + ", " +
                               address.County + ", " + address.PostCode + ".";
                address.DateCreated = DateTime.Now;
                address.DateUpdated = DateTime.Now;
                address.ToString = ToString;
                _context.Addresses.Add(address);
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException as PostgresException;
                    if (innerException != null && innerException.Code == "23505")
                    {
                        ModelState.AddModelError(string.Empty, "Postal Code already exists on a different address.");
                        return View();
                    }
                }
                if (Id != null)
                {
                    Person person = _context.Persons.Find(Id);
                    person.AddressId = address.Id;
                    person.DateUpdated = DateTime.Now;
                    _context.Persons.Update(person);
                    _context.SaveChanges();

                    if (person.Descriminator == "Bank Personnel")
                    {
                        TempData["Response"] = "Enrollment for "+ person.FullName + " is complete. Account password creation link has also been sent to " + person.FullName + "'s email";
                        return RedirectToAction("Index", "Admin");
                    }
                }
                TempData["Response"] = "New address successfully created";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(string Id)
        {
            _context.Addresses.Remove(_context.Addresses.FirstOrDefault(i => i.Id == Id));
            _context.SaveChanges();
            TempData["Response"] = "Address successfully deleted";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
//            ViewBag.Data = _context.Addresses.FirstOrDefault(i => i.Id == Id);
            return View(_context.Addresses.Find(Id));
        }

        [HttpPost]
        public IActionResult Edit(Address address, string Id)
        {
            if (ModelState.IsValid)
            {
                var ToString = address.HouseNumber + " " + address.Street + ", " + address.Town + ", " +
                               address.County + ", " + address.PostCode + ".";
                address.DateUpdated = DateTime.Now;
                address.DateCreated = DateTime.Parse("2017-01-09 08:46:05");
                address.ToString = ToString;
                _context.Addresses.Update(address);
//                _context.Entry(address).State = EntityState.Modified;
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException as PostgresException;
                    if (innerException != null && innerException.Code == "23505")
                    {
                        ModelState.AddModelError(string.Empty, "Postal Code already exists on a different address.");
                        return View();
                    }
                }
                TempData["Response"] = "Address update is sucessful";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}