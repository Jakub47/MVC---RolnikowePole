namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dodaniePolaTest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Zwierze", "Test", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Zwierze", "Test");
        }
    }
}
