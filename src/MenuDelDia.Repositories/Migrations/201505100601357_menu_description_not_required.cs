namespace MenuDelDia.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menu_description_not_required : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Menus", "Description", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Menus", "Description", c => c.String(nullable: false));
        }
    }
}
