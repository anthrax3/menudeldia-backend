namespace MenuDelDia.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppComments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Message = c.String(nullable: false),
                        CreationDateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        Ip = c.String(),
                        Uuid = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Cards",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        CardType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Restaurants",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        LogoPath = c.String(),
                        LogoExtension = c.String(),
                        LogoName = c.String(),
                        Description = c.String(),
                        Url = c.String(),
                        Email = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Identifier = c.String(nullable: false),
                        Streets = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Delivery = c.Boolean(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        SpatialLocation = c.Geography(),
                        RestaurantId = c.Guid(nullable: false),
                        Zone = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Restaurants", t => t.RestaurantId)
                .Index(t => t.RestaurantId);
            
            CreateTable(
                "dbo.Menus",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Ingredients = c.String(),
                        MenuDays_Monday = c.Boolean(nullable: false),
                        MenuDays_Tuesday = c.Boolean(nullable: false),
                        MenuDays_Wednesday = c.Boolean(nullable: false),
                        MenuDays_Thursday = c.Boolean(nullable: false),
                        MenuDays_Friday = c.Boolean(nullable: false),
                        MenuDays_Saturday = c.Boolean(nullable: false),
                        MenuDays_Sunday = c.Boolean(nullable: false),
                        SpecialDay_Date = c.DateTime(),
                        SpecialDay_Recurrent = c.Boolean(nullable: false),
                        Cost = c.Double(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Email = c.String(nullable: false),
                        DateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        Value = c.Int(nullable: false),
                        Message = c.String(),
                        MenuId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Menus", t => t.MenuId)
                .Index(t => t.MenuId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        ApplyToRestaurant = c.Boolean(nullable: false),
                        ApplyToLocation = c.Boolean(nullable: false),
                        ApplyToMenu = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OpenDays",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DayOfWeek = c.Int(nullable: false),
                        OpenHour = c.Int(nullable: false),
                        OpenMinutes = c.Int(nullable: false),
                        CloseHour = c.Int(nullable: false),
                        CloseMinutes = c.Int(nullable: false),
                        Location_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.Location_Id)
                .Index(t => t.Location_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        RoleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Suggestions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Message = c.String(nullable: false),
                        CreationDateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        Ip = c.String(),
                        Uuid = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RestaurantId = c.Guid(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Restaurants", t => t.RestaurantId)
                .Index(t => t.RestaurantId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.RestaurantCards",
                c => new
                    {
                        Restaurant_Id = c.Guid(nullable: false),
                        Card_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Restaurant_Id, t.Card_Id })
                .ForeignKey("dbo.Restaurants", t => t.Restaurant_Id)
                .ForeignKey("dbo.Cards", t => t.Card_Id)
                .Index(t => t.Restaurant_Id)
                .Index(t => t.Card_Id);
            
            CreateTable(
                "dbo.MenuLocations",
                c => new
                    {
                        Menu_Id = c.Guid(nullable: false),
                        Location_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Menu_Id, t.Location_Id })
                .ForeignKey("dbo.Menus", t => t.Menu_Id)
                .ForeignKey("dbo.Locations", t => t.Location_Id)
                .Index(t => t.Menu_Id)
                .Index(t => t.Location_Id);
            
            CreateTable(
                "dbo.TagLocations",
                c => new
                    {
                        Tag_Id = c.Guid(nullable: false),
                        Location_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Location_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id)
                .ForeignKey("dbo.Locations", t => t.Location_Id)
                .Index(t => t.Tag_Id)
                .Index(t => t.Location_Id);
            
            CreateTable(
                "dbo.TagMenus",
                c => new
                    {
                        Tag_Id = c.Guid(nullable: false),
                        Menu_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Menu_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id)
                .ForeignKey("dbo.Menus", t => t.Menu_Id)
                .Index(t => t.Tag_Id)
                .Index(t => t.Menu_Id);
            
            CreateTable(
                "dbo.TagRestaurants",
                c => new
                    {
                        Tag_Id = c.Guid(nullable: false),
                        Restaurant_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Restaurant_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id)
                .ForeignKey("dbo.Restaurants", t => t.Restaurant_Id)
                .Index(t => t.Tag_Id)
                .Index(t => t.Restaurant_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Locations", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.OpenDays", "Location_Id", "dbo.Locations");
            DropForeignKey("dbo.TagRestaurants", "Restaurant_Id", "dbo.Restaurants");
            DropForeignKey("dbo.TagRestaurants", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.TagMenus", "Menu_Id", "dbo.Menus");
            DropForeignKey("dbo.TagMenus", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.TagLocations", "Location_Id", "dbo.Locations");
            DropForeignKey("dbo.TagLocations", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.MenuLocations", "Location_Id", "dbo.Locations");
            DropForeignKey("dbo.MenuLocations", "Menu_Id", "dbo.Menus");
            DropForeignKey("dbo.Comments", "MenuId", "dbo.Menus");
            DropForeignKey("dbo.RestaurantCards", "Card_Id", "dbo.Cards");
            DropForeignKey("dbo.RestaurantCards", "Restaurant_Id", "dbo.Restaurants");
            DropIndex("dbo.TagRestaurants", new[] { "Restaurant_Id" });
            DropIndex("dbo.TagRestaurants", new[] { "Tag_Id" });
            DropIndex("dbo.TagMenus", new[] { "Menu_Id" });
            DropIndex("dbo.TagMenus", new[] { "Tag_Id" });
            DropIndex("dbo.TagLocations", new[] { "Location_Id" });
            DropIndex("dbo.TagLocations", new[] { "Tag_Id" });
            DropIndex("dbo.MenuLocations", new[] { "Location_Id" });
            DropIndex("dbo.MenuLocations", new[] { "Menu_Id" });
            DropIndex("dbo.RestaurantCards", new[] { "Card_Id" });
            DropIndex("dbo.RestaurantCards", new[] { "Restaurant_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "RestaurantId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.OpenDays", new[] { "Location_Id" });
            DropIndex("dbo.Comments", new[] { "MenuId" });
            DropIndex("dbo.Locations", new[] { "RestaurantId" });
            DropTable("dbo.TagRestaurants");
            DropTable("dbo.TagMenus");
            DropTable("dbo.TagLocations");
            DropTable("dbo.MenuLocations");
            DropTable("dbo.RestaurantCards");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Suggestions");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.OpenDays");
            DropTable("dbo.Tags");
            DropTable("dbo.Comments");
            DropTable("dbo.Menus");
            DropTable("dbo.Locations");
            DropTable("dbo.Restaurants");
            DropTable("dbo.Cards");
            DropTable("dbo.AppComments");
        }
    }
}
