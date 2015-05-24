namespace MenuDelDia.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class script_combo_options : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Menus", "IncludeBeverage", c => c.Boolean(nullable: false));
            AddColumn("dbo.Menus", "IncludeDesert", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Menus", "IncludeDesert");
            DropColumn("dbo.Menus", "IncludeBeverage");
        }
    }
}
