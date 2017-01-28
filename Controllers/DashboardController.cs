using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AbcBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AbcBank.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly MyDbContext _context;

        public DashboardController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, MyDbContext context,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }


        public async Task<IActionResult> Index()
        {
            var CurrentUser = await GetCurrentUserAsync();
            var Role = await _userManager.GetRolesAsync(CurrentUser);

            await SetSeedData();

            var Id = _context.Persons.FirstOrDefault(x => x.Email == CurrentUser.Email).Id;

            if (User.IsInRole("Customer") && !_context.AccountHolders.Any(x => x.PersonId == Id))
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation(4, "User logged out.");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            if (await _userManager.IsInRoleAsync(CurrentUser, "Manager"))
            {
                var result = _context.Persons.Find(Id);

                var model = new PersonJoinView()
                {
                    FullName = result.FullName,
                    Email = result.Email,
                    Sex = result.Sex,
                    BranchName = "",
                    MarritalStatus = result.MarritalStatus,
                    DateOfBirth = result.DateOfBirth,
                    Age = result.Age,
                    Address = "",
                    Role = Role
                };
                return View(model);
            }
            else
            {
                var result = _context.Persons
                    .Join(_context.BankBranches,
                        x => x.BankBranchId, d => d.Id,
                        (person, branch) => new {person, branch})
                    .Join(_context.Addresses, x => x.person.AddressId, d => d.Id,
                        (person, address) => new {person, address}).FirstOrDefault(x => x.person.person.Id == Id);

                var model = new PersonJoinView()
                {
                    FullName = result.person.person.FullName,
                    Email = result.person.person.Email,
                    Sex = result.person.person.Sex,
                    BranchName = result.person.branch.BranchName,
                    MarritalStatus = result.person.person.MarritalStatus,
                    DateOfBirth = result.person.person.DateOfBirth,
                    Age = result.person.person.Age,
                    Address = result.address.ToString,
                    Role = Role
                };
                return View(model);
            }
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

                if (!_context.Persons.Any(x => x.Email == User.Email))
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