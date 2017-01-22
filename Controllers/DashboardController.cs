using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AbcBank.Controllers;
using AbcBank.Models;
using AbcBank.Models.ManageViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NuGet.Protocol.Core.v3;

namespace AbcBank.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyDbContext _context;

        public DashboardController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, MyDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var User = await GetCurrentUserAsync();
            var Rile = await _userManager.GetRolesAsync(User);
            await SetSeedData();
            ViewBag.Role = User.Email;
            return View();
        }

        public Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private async Task SetSeedData()
        {
            var User = await GetCurrentUserAsync();
            if (User.Email == "manager@abc.co.uk")
            {
                var roleExist = await _roleManager.RoleExistsAsync("Manager");
                if (!roleExist)
                {
                    IdentityRole roleName = new IdentityRole("Manager");
                    await _roleManager.CreateAsync(roleName);
                    await _userManager.AddToRoleAsync(User, "Manager");
                }

                if (!_context.Persons.Where(x => x.Email == User.Email).Any())
                {
                    Administrator person = new Administrator
                    {
                        LastName = "Charles",
                        FirstName = "Duke",
                        Email = User.Email,
                        MarritalStatus = "Married",
                        Sex = "Male",
                        DateOfBirth = DateTime.Parse("1995-06-21 00:00:00"),
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        Descriminator = "Bank Personnel",
                        HireDate = DateTime.Parse("2005-06-24 00:00:00")
                    };

                    _context.Persons.Add(person);
                    _context.SaveChanges();
                }
            }
        }

    }
}