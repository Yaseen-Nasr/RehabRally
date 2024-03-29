﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RehabRally.Core.Models;
 using System.Security.Claims;

namespace RehabRally.Web.Helpers
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory
            (UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FullName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id)); 


            return identity;
        }

    }
}
