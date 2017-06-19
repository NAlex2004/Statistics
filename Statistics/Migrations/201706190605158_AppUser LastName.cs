namespace Statistics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppUserLastName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String(nullable: false, maxLength: 150));
            CreateIndex("dbo.AspNetUsers", "LastName", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUsers", new[] { "LastName" });
            DropColumn("dbo.AspNetUsers", "LastName");
        }
    }
}
