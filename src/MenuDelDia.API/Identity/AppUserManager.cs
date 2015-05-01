using System;
using MenuDelDia.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace MenuDelDia.API.Identity
{
    public class AppUserManager : UserManager<ApplicationUser, Guid>
    {
        public AppUserManager(IAppUserStore store)
            : base(store)
        {

            //No es posible asignar esto en el constructor, se evita la logica.

            //// Configure validation logic for usernames
            //manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            //{
            //    AllowOnlyAlphanumericUserNames = false,
            //    RequireUniqueEmail = true
            //};


            // Configure validation logic for passwords
            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;

            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("MDD");
            UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, Guid>(provider.Create("ASP.NET Identity"))
            {
                TokenLifespan = TimeSpan.FromHours(3)
            };
        }
    }
}