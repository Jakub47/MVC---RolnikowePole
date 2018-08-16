namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Gatunek",
                c => new
                    {
                        GatunekId = c.Int(nullable: false, identity: true),
                        NazwaGatunku = c.String(nullable: false, maxLength: 100),
                        OpisGatunku = c.String(nullable: false),
                        NazwaPlikuIkony = c.String(),
                    })
                .PrimaryKey(t => t.GatunekId);
            
            CreateTable(
                "dbo.Zwierze",
                c => new
                    {
                        ZwierzeId = c.Int(nullable: false, identity: true),
                        GatunekId = c.Int(nullable: false),
                        Rasa = c.String(),
                        Nazwa = c.String(nullable: false, maxLength: 100),
                        DataNarodzin = c.DateTime(nullable: false),
                        DataDodania = c.DateTime(nullable: false),
                        NazwaPlikuObrazka = c.String(maxLength: 100),
                        OpisZwierza = c.String(nullable: false),
                        CenaZwierza = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Wojewodztwo = c.String(),
                        Miasto = c.String(),
                        Odrobaczony = c.Boolean(nullable: false),
                        Szczepiony = c.Boolean(nullable: false),
                        Wykastrowany = c.Boolean(nullable: false),
                        Wyrozniony = c.Boolean(nullable: false),
                        Ukryty = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ZwierzeId)
                .ForeignKey("dbo.Gatunek", t => t.GatunekId, cascadeDelete: true)
                .Index(t => t.GatunekId);
            
            CreateTable(
                "dbo.PozycjaZamowienia",
                c => new
                    {
                        PozycjaZamowieniaId = c.Int(nullable: false, identity: true),
                        ZamowienieId = c.Int(nullable: false),
                        ZwierzeId = c.Int(nullable: false),
                        MyProperty = c.Int(nullable: false),
                        Ilosc = c.Int(nullable: false),
                        CenaZakupu = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.PozycjaZamowieniaId)
                .ForeignKey("dbo.Zamowienie", t => t.ZamowienieId, cascadeDelete: true)
                .ForeignKey("dbo.Zwierze", t => t.ZwierzeId, cascadeDelete: true)
                .Index(t => t.ZamowienieId)
                .Index(t => t.ZwierzeId);
            
            CreateTable(
                "dbo.Zamowienie",
                c => new
                    {
                        ZamowienieId = c.Int(nullable: false, identity: true),
                        Imie = c.String(nullable: false, maxLength: 50),
                        Nazwisko = c.String(nullable: false, maxLength: 50),
                        Ulica = c.String(nullable: false, maxLength: 100),
                        Miasto = c.String(nullable: false, maxLength: 100),
                        KodPocztowy = c.String(nullable: false, maxLength: 15),
                        Telefon = c.String(),
                        Email = c.String(),
                        Komentarz = c.String(),
                        DataDodania = c.DateTime(nullable: false),
                        StanZamowienia = c.Int(nullable: false),
                        WartoscZamowienia = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ZamowienieId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PozycjaZamowienia", "ZwierzeId", "dbo.Zwierze");
            DropForeignKey("dbo.PozycjaZamowienia", "ZamowienieId", "dbo.Zamowienie");
            DropForeignKey("dbo.Zwierze", "GatunekId", "dbo.Gatunek");
            DropIndex("dbo.PozycjaZamowienia", new[] { "ZwierzeId" });
            DropIndex("dbo.PozycjaZamowienia", new[] { "ZamowienieId" });
            DropIndex("dbo.Zwierze", new[] { "GatunekId" });
            DropTable("dbo.Zamowienie");
            DropTable("dbo.PozycjaZamowienia");
            DropTable("dbo.Zwierze");
            DropTable("dbo.Gatunek");
        }
    }
}
