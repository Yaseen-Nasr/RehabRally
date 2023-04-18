using Microsoft.AspNetCore.Identity;
using RehabRally.Core.Consts;

namespace RehabRally.Web.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Doctor));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Patient)); 
            }
        }
    }
}
