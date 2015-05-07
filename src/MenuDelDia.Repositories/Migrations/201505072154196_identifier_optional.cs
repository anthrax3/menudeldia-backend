namespace MenuDelDia.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class identifier_optional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Locations", "Identifier", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Locations", "Identifier", c => c.String(nullable: false));
        }
    }
}
