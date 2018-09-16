namespace RolnikowePole.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Elmah : DbMigration
    {
        public override void Up()
        {
            //In case of exception please drag&drop elmah-1.2.sql from Migrations folder here
            SqlFile("C:\\Users\\Ragnus\\Desktop\\Projects\\MVC---RolnikowePole\\RolnikowePole\\RolnikowePole\\Migrations\\ELMAH-1.2-db-SQLServer.sql");
        }
        
        public override void Down()
        {
        }
    }
}
