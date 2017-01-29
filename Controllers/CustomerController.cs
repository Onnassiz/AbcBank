using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbcBank.Data;
using AbcBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;


namespace AbcBank.Controllers
{
    [Authorize(Roles = "Manager, Banker")]
    public class CustomerController:Controller
    {
        private readonly MyDbContext _context;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;


        public CustomerController(MyDbContext context,
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

        public IActionResult Index()
        {
            var result = _context.Persons.OfType<Customer>()
                .Join(_context.BankBranches,
                    x => x.BankBranchId, d => d.Id,
                    (person, branch) => new
                    {
                        person,
                        branch
                    }
                )
                .Join(_context.Persons,
                    x => x.person.BankerId, d => d.Id,
                    (person, admin) => new
                    {
                        person,
                        admin
                    }
                )
                .OrderByDescending(x => x.person.person.DateUpdated);

            List<PersonJoinView> model = new List<PersonJoinView>();

            foreach (var item in result)
            {
                model.Add(new PersonJoinView()
                {
                    Id = item.person.person.Id,
                    FullName = item.person.person.FullName,
                    Email = item.person.person.Email,
                    Sex = item.person.person.Sex,
                    BranchName = item.person.branch.BranchName,
                    Banker = item.admin.FullName
                });
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Sex = new List<string> {"Male", "Female"};
            ViewBag.MarritalStatus = new List<string> {"Single", "Married", "Divorced"};
            var Branch = new SelectList(_context.BankBranches.ToList(), "Id", "BranchName");
            ViewBag.Branch = Branch;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            ViewBag.Sex = new List<string> {"Male", "Female"};
            ViewBag.MarritalStatus = new List<string> {"Single", "Married", "Divorced"};
            var Branch = new SelectList(_context.BankBranches.ToList(), "Id", "BranchName");
            ViewBag.Branch = Branch;

            if (ModelState.IsValid)
            {
                customer.DateCreated = DateTime.Now;
                customer.DateUpdated = DateTime.Now;
                _context.Persons.Add(customer);
                _context.SaveChanges();
                await CreateAccount(customer.Email);
                await SetRole(customer.Email, "Customer");
                await SendAuthMail(customer.Email, customer.FullName);

                TempData["Response"] = "Record successfully created. Enter admin's address to continue";
                return RedirectToAction("Address", new {id = customer.Id});
            }
            return View();
        }

        public IActionResult Address(string Id)
        {
            ViewBag.Address = new SelectList(_context.Addresses.ToList(), "Id", "AddressToString");
            return View(_context.Persons.Find(Id));
        }

        [HttpPost]
        public IActionResult Address(Customer customer, string Id)
        {
            ViewBag.Address = new SelectList(_context.Addresses.ToList(), "Id", "AddressToString");

            var person = _context.Persons.OfType<Customer>().FirstOrDefault(x => x.Id == Id);
            var BankBranchId = person.BankBranchId;

            var MinCount = _context.Persons.OfType<Administrator>()
                .Where(x => x.BankBranchId == BankBranchId)
                .Min(j => j.CustomerCount);

            var BankerId = _context.Persons.OfType<Administrator>().FirstOrDefault(
                    b => b.BankBranchId == BankBranchId && b.CustomerCount == MinCount
            ).Id;

            Administrator administrator = _context.Persons.OfType<Administrator>()
                .FirstOrDefault(x => x.Id == BankerId);


            person.BankerId = BankerId;
            person.AddressId = customer.AddressId;
            person.DateUpdated = DateTime.Now;
            administrator.CustomerCount = MinCount + 1;
            _context.Persons.Update(person);
            _context.Persons.Update(administrator);
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
                await userStore.CreateAsync(user);
                await _applicationDbContext.SaveChangesAsync();
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
            var callbackUrl = Url.Action("CreatePasswordFirst", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

            Mailer.Send(email, "Set Up Password",
                $"Hello {fullName},<br/><br/>You have been registered as an ABC Bank customer. <br/><br/>Please click this : <a href='{callbackUrl}'>link</a> to setup your password and start using your account");
            TempData["Response"] = "Please check your email.";
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(string Id)
        {
            var user = _context.Persons.OfType<Customer>().FirstOrDefault(x => x.Id == Id);
            var BankerId = user.BankerId;
            var Banker = _context.Persons.OfType<Administrator>().FirstOrDefault(x => x.Id == BankerId);
            Banker.CustomerCount = Banker.CustomerCount - 1;
            var User = await _userManager.FindByNameAsync(user.Email);
            await _userManager.DeleteAsync(User);
            _context.Persons.Remove(user);
            _context.Persons.Update(Banker);
            _context.SaveChanges();
            TempData["Response"] = "User Record Successfully Deleted";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            ViewBag.Sex = new List<string> {"Male", "Female"};
            ViewBag.MarritalStatus = new List<string> {"Single", "Married", "Divorced"};
            ViewBag.Address = new SelectList(_context.Addresses.ToList(), "Id", "AddressToString");
            var Branch = new SelectList(_context.BankBranches.ToList(), "Id", "BranchName");
            ViewBag.Branch = Branch;

            return View(_context.Persons.OfType<Customer>().FirstOrDefault(x => x.Id == Id));
        }

        [HttpPost]
        public IActionResult Edit(Customer customer, string Id)
        {
            ViewBag.Sex = new List<string> {"Male", "Female"};
            ViewBag.MarritalStatus = new List<string> {"Single", "Married", "Divorced"};
            ViewBag.Address = new SelectList(_context.Addresses.ToList(), "Id", "AddressToString");

            var Branch = new SelectList(_context.BankBranches.ToList(), "Id", "BranchName");

            ViewBag.Branch = Branch;

            if (ModelState.IsValid)
            {
                customer.DateUpdated = DateTime.Now;

                _context.Persons.Update(customer);
                _context.SaveChanges();

                TempData["Response"] = "Record Successfully Updated";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult ViewItem(string Id)
        {
            var result = _context.Persons.OfType<Customer>()
                .Join(_context.BankBranches,
                    x => x.BankBranchId, d => d.Id,
                    (person, branch) => new {person, branch})
                .Join(_context.Addresses, x => x.person.AddressId, d => d.Id,
                    (person, address) => new {person, address}).FirstOrDefault(x => x.person.person.Id == Id);
            var model = new PersonJoinView()
            {

                Id = result.person.person.Id,
                FullName = result.person.person.FullName,
                Email = result.person.person.Email,
                Sex = result.person.person.Sex,
                BranchName = result.person.branch.BranchName,
                MarritalStatus = result.person.person.MarritalStatus,
                LastName = result.person.person.LastName,
                DateOfBirth = result.person.person.DateOfBirth,
                Age = result.person.person.Age,
                Address = result.address.AddressToString
            };

            ViewBag.Banker = _context.Persons.Find(_context.Persons.OfType<Customer>().FirstOrDefault(x => x.Id == Id).BankerId).FullName;
            return View(model);
        }
    }
}