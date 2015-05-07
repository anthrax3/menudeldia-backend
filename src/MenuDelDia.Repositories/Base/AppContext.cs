using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuDelDia.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MenuDelDia.Repository
{
    public class AppContext : IdentityDbContext<ApplicationUser, AppRole, Guid, AppUserLogin, AppUserRole, AppUserClaim>
    {
        public AppContext() : base("AppContext") { }

        public IDbSet<Card> Cards { get; set; }
        public IDbSet<Comment> Comments { get; set; }
        public IDbSet<AppComment> AppComments { get; set; }
        public IDbSet<Location> Locations { get; set; }
        public IDbSet<Menu> Menus { get; set; }
        public IDbSet<OpenDay> OpenDays { get; set; }
        public IDbSet<Restaurant> Restaurants { get; set; }
        public IDbSet<Tag> Tags { get; set; }
        public IDbSet<Suggestion> Suggestion { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Entity<Location>().Property(t => t.Description).IsOptional();
            modelBuilder.Entity<Location>().Property(t => t.Identifier).IsOptional();
        }
        public static AppContext Create()
        {
            return new AppContext();
        }
    }
}
