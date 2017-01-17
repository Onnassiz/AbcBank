using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbcBank.Data;
using AbcBank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AbcBank.Controllers
{
    public class AdminController : Controller
    {
        private readonly MyDbContext _context;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;


        public AdminController(MyDbContext context,
            ApplicationDbContext applicationDbContext,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager
        )
        {
            _context = context;
            _applicationDbContext = applicationDbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string Type)
        {
            var result = _context.Persons.OfType<Administrator>().Join(_context.BankBranches,
                    x => x.BankBranchId, d => d.Id,
                    (person, branch) => new
                    {
                        person, branch
                    }
            ).OrderByDescending(i => i.person.DateUpdated);

            List<PersonJoinView> model = new List<PersonJoinView>();

            foreach (var item in result)
            {
                model.Add(new PersonJoinView()
                {
                    Id = item.person.Id,
                    FullName = item.person.FullName,
                    Email = item.person.Email,
                    AdminType = item.person.AdminType,
                    Sex = item.person.Sex,
                    HireDate = item.person.HireDate,
                    BranchName = item.branch.BranchName
                });
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Sex = new List<string> {"Male", "Female"};
            ViewBag.MarritalStatus = new List<string> {"Single", "Married", "Divorced"};

            List<SelectListItem> AdminType = new List<SelectListItem>();
            AdminType.Add(new SelectListItem {Text = "Banker", Value = "Banker"});
            AdminType.Add(new SelectListItem {Text = "Teller", Value = "Teller", Selected = true});

            var Branch = new SelectList(_context.BankBranches.ToList(), "Id", "BranchName");

            ViewBag.AdminType = AdminType;
            ViewBag.Branch = Branch;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Administrator administrator)
        {
            ViewBag.Sex = new List<string> {"Male", "Female"};
            ViewBag.MarritalStatus = new List<string> {"Single", "Married", "Divorced"};

            List<SelectListItem> AdminType = new List<SelectListItem>();
            AdminType.Add(new SelectListItem {Text = "Banker", Value = "Banker"});
            AdminType.Add(new SelectListItem {Text = "Teller", Value = "Teller", Selected = true});
            ViewBag.AdminType = AdminType;

            if (ModelState.IsValid)
            {
                administrator.DateCreated = DateTime.Now;
                administrator.DateUpdated = DateTime.Now;
                _context.Persons.Add(administrator);
                try
                {
                    _context.SaveChanges();
                    await CreateAccount(administrator.Email);
                    await SetRole(administrator.Email, administrator.AdminType);
                    await SendAuthMail(administrator.Email, administrator.FullName);
                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException as PostgresException;
                    if (innerException != null && innerException.Code == "23505")
                    {
                        ModelState.AddModelError(string.Empty, "Email already exists on a different user.");
                        return View();
                    }
                }
                TempData["Response"] = "Record successfully created. Enter admin's address to continue";
                return RedirectToAction("Address", new {id = administrator.Id});
            }
            return View();
        }

        [HttpGet]
        public IActionResult Address(string Id)
        {
            ViewBag.Address = new SelectList(_context.Addresses.ToList(), "Id", "ToString");
            return View(_context.Persons.Find(Id));
        }

        [HttpPost]
        public IActionResult Address(Administrator administrator, string Id)
        {
            ViewBag.Address = new SelectList(_context.Addresses.ToList(), "Id", "ToString");

            Person person = _context.Persons.Find(Id);
            person.AddressId = administrator.AddressId;
            person.DateUpdated = DateTime.Now;
            _context.Persons.Update(person);
            _context.SaveChanges();

            TempData["Response"] = "Enrollment for " + person.FullName +
                                   " is complete. Account password creation link has also been sent to " +
                                   person.FullName + "'s email";
            return RedirectToAction("Index");
        }

        private async Task CreateAccount(string email)
        {
            var user = new ApplicationUser
            {
                Email = email,
                NormalizedEmail = email.ToUpper(),
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            if (!_applicationDbContext.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, RandomString(10));
                user.PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(_applicationDbContext);
                userStore.CreateAsync(user);
                _applicationDbContext.SaveChangesAsync();
            }
        }

        private async Task SetRole(string email, string role)
        {
            var User = _applicationDbContext.Users.FirstOrDefault(x => x.Email == email);
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (roleExist)
            {
                TempData["Response"] = "Yes I found the role";
                await _userManager.AddToRoleAsync(User, role);
            }
            else
            {
                TempData["Response"] = "Yes I the role";
            }
        }

        private string RandomString(int Size)
        {
            Random random = new Random();
            string input = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, Size)
                .Select(x => input[random.Next(0, input.Length)]);
            return new string(chars.ToArray());
        }

        private async Task SendAuthMail(string email, string fullName)
        {
            var user = await _userManager.FindByNameAsync(email);
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("CreatePassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

            Mailer.Send(email, "Set Up Password",
                $"Hello {fullName},<br/><br/>You have been registered as an ABC Bank administrator. <br/><br/>Please click this : <a href='{callbackUrl}'>link</a> to setup your password");
            TempData["Response"] = "Please check your email.";
        }

        public async Task<IActionResult> Delete(string Id)
        {
            var user = _context.Persons.Find(Id);
            var User = await _userManager.FindByNameAsync(user.Email);
            await _userManager.DeleteAsync(User);
            _context.Persons.Remove(user);
            _context.SaveChanges();
            TempData["Response"] = "Admin Record Successfully Deleted";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            ViewBag.Sex = new List<string> {"Male", "Female"};
            ViewBag.MarritalStatus = new List<string> {"Single", "Married", "Divorced"};

            List<SelectListItem> AdminType = new List<SelectListItem>();
            AdminType.Add(new SelectListItem {Text = "Banker", Value = "Banker"});
            AdminType.Add(new SelectListItem {Text = "Teller", Value = "Teller", Selected = true});

            var Branch = new SelectList(_context.BankBranches.ToList(), "Id", "BranchName");

            ViewBag.AdminType = AdminType;
            ViewBag.Branch = Branch;

            return View(_context.Persons.Find(Id));
        }

        [HttpPost]
        public IActionResult Edit(string Id, Administrator administrator)
        {
            ViewBag.Sex = new List<string> {"Male", "Female"};
            ViewBag.MarritalStatus = new List<string> {"Single", "Married", "Divorced"};

            List<SelectListItem> AdminType = new List<SelectListItem>();
            AdminType.Add(new SelectListItem {Text = "Banker", Value = "Banker"});
            AdminType.Add(new SelectListItem {Text = "Teller", Value = "Teller", Selected = true});

            var Branch = new SelectList(_context.BankBranches.ToList(), "Id", "BranchName");

            ViewBag.AdminType = AdminType;
            ViewBag.Branch = Branch;

            administrator.DateUpdated = DateTime.Now;
            _context.Persons.Update(administrator);
            _context.SaveChanges();

            TempData["Response"] = "Record Successfully Updated";
            return RedirectToAction("Index");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}