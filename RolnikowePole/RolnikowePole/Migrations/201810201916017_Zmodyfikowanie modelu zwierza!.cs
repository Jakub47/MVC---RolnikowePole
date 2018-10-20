namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Zmodyfikowaniemodeluzwierza : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Zwierze", "OpisZwierza", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.Zwierze", "Miasto", c => c.String(nullable: false));
            AlterColumn("dbo.Zwierze", "OpisSkrocony", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Zwierze", "OpisSkrocony", c => c.String());
            AlterColumn("dbo.Zwierze", "Miasto", c => c.String());
            AlterColumn("dbo.Zwierze", "OpisZwierza", c => c.String(nullable: false));
        }
    }
}
