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
        public DashboardController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var User = await GetCurrentUserAsync();
            var Rile = await _userManager.GetRolesAsync(User);
            await SetSeedData();
            ViewBag.Role = User.Email;
            return View();
        }
        private Task<ApplicationUser> GetCurrentUserAsync()
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
            }
        }
    }
}