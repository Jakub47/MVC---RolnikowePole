namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ZmianaPolaZwierzeDoWiadomosciNaINT : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Wiadomosc", "ZwierzeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Wiadomosc", "ZwierzeId", c => c.String());
        }
    }
}
