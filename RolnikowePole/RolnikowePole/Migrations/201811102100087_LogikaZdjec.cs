namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogikaZdjec : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Zdjecie",
                c => new
                    {
                        ZdjecieID = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        ZwierzeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ZdjecieID)
                .ForeignKey("dbo.Zwierze", t => t.ZwierzeId, cascadeDelete: true)
                .Index(t => t.ZwierzeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Zdjecie", "ZwierzeId", "dbo.Zwierze");
            DropIndex("dbo.Zdjecie", new[] { "ZwierzeId" });
            DropTable("dbo.Zdjecie");
        }
    }
}
