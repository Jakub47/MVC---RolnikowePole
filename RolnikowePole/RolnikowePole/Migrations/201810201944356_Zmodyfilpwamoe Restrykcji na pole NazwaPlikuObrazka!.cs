namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ZmodyfilpwamoeRestrykcjinapoleNazwaPlikuObrazka : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Zwierze", "NazwaPlikuObrazka", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Zwierze", "NazwaPlikuObrazka", c => c.String(maxLength: 100));
        }
    }
}
