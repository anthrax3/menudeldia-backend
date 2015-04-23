
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MenuDelDia.Entities
{
    public class AppUserLogin : IdentityUserLogin<Guid> { }

    public class AppUserRole : IdentityUserRole<Guid> { }

    public class AppUserClaim : IdentityUserClaim<Guid> { }

    public class AppRole : IdentityRole<Guid, AppUserRole> { }

    [Table("Users")]
    public class ApplicationUser : IdentityUser<Guid, AppUserLogin, AppUserRole, AppUserClaim>
    {
        public override Guid Id { get; set; }

        public Guid? RestaurantId { get; set; }
        [ForeignKey("RestaurantId")]
        public Restaurant Restaurant { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, Guid> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
