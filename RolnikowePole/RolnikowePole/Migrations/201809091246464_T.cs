namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class T : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Zwierze", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Zwierze", "ApplicationUser_Id");
            AddForeignKey("dbo.Zwierze", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Zwierze", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Zwierze", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Zwierze", "ApplicationUser_Id");
        }
    }
}
