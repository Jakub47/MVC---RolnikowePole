using RolnikowePole.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace RolnikowePole.DAL
{
    public class RolnikowePoleContext : DbContext
    {
        public RolnikowePoleContext() : base("RolnikowePoleContext")
        {

        }

        static RolnikowePoleContext()
        {
            Database.SetInitializer<RolnikowePoleContext>(new RolnikowePoleInitializer());
        }

        public DbSet<Zwierze> Zwierzeta { get; set; }
        public DbSet<Gatunek> Gatunki { get; set; }
        public DbSet<Zamowienie> Zamowienia { get; set; }
        public DbSet<PozycjaZamowienia> PozycjeZamowienia { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}