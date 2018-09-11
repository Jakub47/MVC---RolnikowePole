namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DodanieUserIdDoZwierzeModel : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Zwierze", name: "ApplicationUser_Id", newName: "UserId");
            RenameIndex(table: "dbo.Zwierze", name: "IX_ApplicationUser_Id", newName: "IX_UserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Zwierze", name: "IX_UserId", newName: "IX_ApplicationUser_Id");
            RenameColumn(table: "dbo.Zwierze", name: "UserId", newName: "ApplicationUser_Id");
        }
    }
}
