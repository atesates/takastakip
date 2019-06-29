using System.Data.Entity;
using WM.Northwind.DataAccess.Concrete.EntityFramework.Initializers;
using WM.Northwind.DataAccess.Concrete.EntityFramework.Mapping.IlacTakip;
using WM.Northwind.DataAccess.Concrete.EntityFramework.Mapping.Authorization;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.Concrete.Authorization;
using System.Collections.Generic;
using WM.Northwind.DataAccess.Migrations;

namespace WM.Northwind.DataAccess.Concrete.EntityFramework.Contexts
{
    public class IlacTakipContext : DbContext
    {
        static IlacTakipContext()
        {
            //Database.SetInitializer(new IlacTakipInitializer());
            //Database.SetInitializer(new IlacTakipInitializerAlanya());
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<IlacTakipContext>());

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<IlacTakipContext, Configuration>("IlacTakipContext"));
           // Database.SetInitializer<IlacTakipContext>(new DropCreateDatabaseIfModelChanges<IlacTakipContext>());
        }

        public IlacTakipContext() : base("Name=IlacTakipContext")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<EczaneUser> EczaneUserlar { get; set; }
        public DbSet<MenuRole> MenuRoles { get; set; }
        public DbSet<MenuAltRole> MenuAltRoles { get; set; }
        public DbSet<Menu> Menuler { get; set; }
        public DbSet<MenuAlt> MenuAltlar { get; set; }

        #region IlacTakip
        public DbSet<Alim> Alimlar { get; set; }
        public DbSet<AlimDurum> AlimDurumlar { get; set; }
        public DbSet<EczaneGrup> EczaneGruplar { get; set; }
       // public DbSet<AlimTeklif> AlimTeklifler { get; set; }

        public DbSet<Eczane> Eczaneler { get; set; }
        public DbSet<Grup> Gruplar { get; set; }

        public DbSet<IlacDurum> IlacDurumlar { get; set; }
        public DbSet<Ilac> Ilaclar { get; set; }
        public DbSet<ITStransferDurum> ITStransferDurumlar { get; set; }
        public DbSet<QRKod> QRKodlar { get; set; }
        public DbSet<Log> Loglar { get; set; }
        public DbSet<Sehir> Sehirler { get; set; }
        public DbSet<Teklif> Teklifler { get; set; }
        public DbSet<Talep> Talepler { get; set; }
        public DbSet<Katilim> Katilimlar { get; set; }
        public DbSet<TalepDurum> TalepDurumlar { get; set; }
        public DbSet<TeklifDurum> TeklifDurumlar { get; set; }
        public DbSet<TeklifTur> TeklifTurler { get; set; }
        public DbSet<YayinlamaTur> YayinlamaTurler { get; set; }
        #endregion

        #region Mapping
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           // modelBuilder.HasDefaultSchema("IlacTakip");

            #region Yetki
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new UserRoleMap());


            #endregion
            modelBuilder.Configurations.Add(new EczaneUserMap());
            modelBuilder.Configurations.Add(new AlimMap());
            modelBuilder.Configurations.Add(new KatilimMap());
            modelBuilder.Configurations.Add(new AlimDurumMap());
           // modelBuilder.Configurations.Add(new AlimTeklifMap());
            modelBuilder.Configurations.Add(new GrupMap());
            modelBuilder.Configurations.Add(new EczaneGrupMap());
            modelBuilder.Configurations.Add(new EczaneMap());
            modelBuilder.Configurations.Add(new MenuRoleMap());
            modelBuilder.Configurations.Add(new MenuAltRoleMap());

            modelBuilder.Configurations.Add(new MenuMap());
            modelBuilder.Configurations.Add(new MenuAltMap());
            modelBuilder.Configurations.Add(new IlacMap());
            modelBuilder.Configurations.Add(new ITStransferDurumMap());
            modelBuilder.Configurations.Add(new IlacDurumMap());
            
            modelBuilder.Configurations.Add(new QRKodMap());
            modelBuilder.Configurations.Add(new SehirMap());
            modelBuilder.Configurations.Add(new TeklifMap());
            modelBuilder.Configurations.Add(new TalepMap());
            modelBuilder.Configurations.Add(new TeklifDurumMap());
            modelBuilder.Configurations.Add(new TalepDurumMap());
            modelBuilder.Configurations.Add(new TeklifTurMap());
            modelBuilder.Configurations.Add(new YayinlamaTurMap());         
        }
        #endregion
    }
}


//Eczane grubu eczanelerin birlikte atanmasıyla ilgili
//Eczane nöbet grubu eczanelerin hangi grupta nöbetçi olacağını belirler