namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Kilkaobrazkow : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Zwierze", "NazwaPlikuObrazka", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Zwierze", "NazwaPlikuObrazka");
        }
    }
}
