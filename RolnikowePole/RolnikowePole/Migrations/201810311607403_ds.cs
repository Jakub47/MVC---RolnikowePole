namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ds : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Zwierze", "NazwaPlikuObrazka", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Zwierze", "NazwaPlikuObrazka", c => c.String(nullable: false, maxLength: 100));
        }
    }
}
