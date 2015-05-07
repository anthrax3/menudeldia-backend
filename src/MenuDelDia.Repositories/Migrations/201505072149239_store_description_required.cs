namespace MenuDelDia.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store_description_required : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Locations", "Description", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Locations", "Description", c => c.String(nullable: false));
        }
    }
}
