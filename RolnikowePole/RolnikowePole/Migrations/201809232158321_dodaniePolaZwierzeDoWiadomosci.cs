namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dodaniePolaZwierzeDoWiadomosci : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Wiadomosc", "Zwierze_ZwierzeId", c => c.Int());
            CreateIndex("dbo.Wiadomosc", "Zwierze_ZwierzeId");
            AddForeignKey("dbo.Wiadomosc", "Zwierze_ZwierzeId", "dbo.Zwierze", "ZwierzeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Wiadomosc", "Zwierze_ZwierzeId", "dbo.Zwierze");
            DropIndex("dbo.Wiadomosc", new[] { "Zwierze_ZwierzeId" });
            DropColumn("dbo.Wiadomosc", "Zwierze_ZwierzeId");
        }
    }
}
