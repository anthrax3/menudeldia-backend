using MenuDelDia.Entities;
using MenuDelDia.Entities.Enums;

namespace MenuDelDia.Repositories.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MenuDelDia.Repository.AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "MenuDelDia.Repository.AppContext";
        }

        protected override void Seed(MenuDelDia.Repository.AppContext context)
        {
            context.Roles.AddOrUpdate(p => p.Name, new AppRole { Id = Guid.Parse("90C694AC-246F-45D6-9435-5A3330CD67D2"), Name = "Administrator" });
            context.Roles.AddOrUpdate(p => p.Name, new AppRole { Id = Guid.Parse("F8F21E7B-B4F5-4D1E-95FA-F9A48E2014A5"), Name = "User" });


            context.Cards.AddOrUpdate(c => new { c.Name, c.CardType }, new Card { Id = Guid.Parse("90C694AC-246F-45D6-9435-5A3330CD67D2"), Name = "Visa", CardType = CardType.Credit });
            context.Cards.AddOrUpdate(c => new { c.Name, c.CardType }, new Card { Id = Guid.Parse("FDEE4F3A-2677-47D3-9DF4-78CB633C0836"), Name = "Visa", CardType = CardType.Debit });
            context.Cards.AddOrUpdate(c => new { c.Name, c.CardType }, new Card { Id = Guid.Parse("ED2D36F3-8851-408F-9882-C6216787EF54"), Name = "Master", CardType = CardType.Credit });
            context.Cards.AddOrUpdate(c => new { c.Name, c.CardType }, new Card { Id = Guid.Parse("F8F21E7B-B4F5-4D1E-95FA-F9A48E2014A5"), Name = "Master", CardType = CardType.Debit });

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
