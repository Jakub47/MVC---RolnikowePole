namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usunieciePolaTest : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Zwierze", "Test");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Zwierze", "Test", c => c.String());
        }
    }
}
