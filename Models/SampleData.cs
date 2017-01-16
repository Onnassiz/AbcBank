using System;
using System.Linq;
using System.Threading.Tasks;
using AbcBank.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AbcBank.Models
{
    public class SampleData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<ApplicationDbContext>();

            var user = new ApplicationUser
            {
                Email = "manager@abc.co.uk",
                NormalizedEmail = "MANAGER@ABC.CO.UK",
                UserName = "manager@abc.co.uk",
                NormalizedUserName = "MANAGER@ABC.CO.UK",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            if (!context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, "secret");
                user.PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(context);
                userStore.CreateAsync(user);
                context.SaveChangesAsync();
            }
        }
    }
}