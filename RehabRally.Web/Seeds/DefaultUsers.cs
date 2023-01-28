using Microsoft.AspNetCore.Identity;
using RehabRally.Web.Core.Consts;
using RehabRally.Web.Core.Models;

namespace RehabRally.Web.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            var doctor = new ApplicationUser
            {
                UserName = "mohamed",
                FullName = "Mohamed Adel",
                Email = "mohamed@rehabrally.com",
                EmailConfirmed = true

            };
            var user = await userManager.FindByEmailAsync(doctor.Email);
            if (user is null)
            {
                await userManager.CreateAsync(doctor, "P@ssword123");
                await userManager.AddToRoleAsync(doctor, AppRoles.Doctor);
            };

        }
    }
}
