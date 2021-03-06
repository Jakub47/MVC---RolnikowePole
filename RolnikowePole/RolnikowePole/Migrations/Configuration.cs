namespace RolnikowePole.Migrations
{
    using RolnikowePole.DAL;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<RolnikowePole.DAL.RolnikowePoleContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "RolnikowePole.DAL.RolnikowePoleContext";
        }

        protected override void Seed(RolnikowePole.DAL.RolnikowePoleContext context)
        {
            //  This method will be called after migrating to the latest version.
            RolnikowePoleInitializer.SeedRolnikowePoleData(context);
            RolnikowePoleInitializer.SeedUzytkownicy(context);
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
