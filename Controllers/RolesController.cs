using System.Linq;
using System.Threading.Tasks;
using AbcBank.Data;
using AbcBank.Models;
using AbcBank.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AbcBank.Controllers
{
    [Authorize(Roles = "Manager")]
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public RolesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View(_context.Roles.OrderBy(i => i.Name).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            var Roles = new IdentityRole();
            return View(Roles);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                if (role.Name == null)
                {
                    ModelState.AddModelError(string.Empty, "Role field is required");
                    return View();
                }
                var roleExist = await _roleManager.RoleExistsAsync(role.Name);
                if (!roleExist)
                {
                    IdentityRole roleName = new IdentityRole(role.Name);
                    await _roleManager.CreateAsync(roleName);
                    TempData["Response"] = "A new role has been added to the role list.";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(role.Name, "Role already exists.");
                return View();
            }
            return View();
        }

        public IActionResult Delete(string  id)
        {
            _context.Roles.Remove(_context.Roles.FirstOrDefault(c => c.Id == id));
            _context.SaveChanges();
            TempData["Response"] = "Your record has been deleted";
            return RedirectToAction("Index");
        }

        [HttpGet]
//        [Authorize(Roles = "Guest")]
        public IActionResult RegisterRole()
        {
            ViewBag.Name = new SelectList(_context.Roles.ToList(), "Name", "Name");
            ViewBag.UserName = new SelectList(_context.Users.ToList(), "UserName", "UserName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterRole(RegisterViewModel model, ApplicationUser user)
        {
            var userId = _context.Users.FirstOrDefault(i => i.UserName == user.UserName);
            await _userManager.AddToRoleAsync(userId, model.Name);
            TempData["Response"] = "The role " + model.Name.ToUpper() + " has been assigned to the user " + user.UserName.ToUpper();
            return RedirectToAction("Index");
        }
    }
}