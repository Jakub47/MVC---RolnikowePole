namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ee1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Wiadomosc",
                c => new
                    {
                        WiadomoscId = c.Int(nullable: false, identity: true),
                        SenderId = c.String(nullable: false, maxLength: 128),
                        ReceiverId = c.String(nullable: false, maxLength: 128),
                        Body = c.String(nullable: false, maxLength: 150),
                        DateAndTimeOfSend = c.DateTime(nullable: false),
                        Read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.WiadomoscId)
                .ForeignKey("dbo.AspNetUsers", t => t.ReceiverId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.SenderId, cascadeDelete: false)
                .Index(t => t.SenderId)
                .Index(t => t.ReceiverId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Wiadomosc", "SenderId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Wiadomosc", "ReceiverId", "dbo.AspNetUsers");
            DropIndex("dbo.Wiadomosc", new[] { "ReceiverId" });
            DropIndex("dbo.Wiadomosc", new[] { "SenderId" });
            DropTable("dbo.Wiadomosc");
        }
    }
}
