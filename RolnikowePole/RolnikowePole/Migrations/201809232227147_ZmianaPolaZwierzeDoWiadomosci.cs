namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ZmianaPolaZwierzeDoWiadomosci : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Wiadomosc", "Zwierze_ZwierzeId", "dbo.Zwierze");
            DropIndex("dbo.Wiadomosc", new[] { "Zwierze_ZwierzeId" });
            AddColumn("dbo.Wiadomosc", "ZwierzeId", c => c.String());
            DropColumn("dbo.Wiadomosc", "Zwierze_ZwierzeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Wiadomosc", "Zwierze_ZwierzeId", c => c.Int());
            DropColumn("dbo.Wiadomosc", "ZwierzeId");
            CreateIndex("dbo.Wiadomosc", "Zwierze_ZwierzeId");
            AddForeignKey("dbo.Wiadomosc", "Zwierze_ZwierzeId", "dbo.Zwierze", "ZwierzeId");
        }
    }
}
