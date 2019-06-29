using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WM.Northwind.Entities.Concrete;
using WM.Northwind.Entities.Concrete.Authorization;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.UI.Mvc.Models
{
    public class WMUIMvcContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public WMUIMvcContext() : base("name=WMUIMvcContext")
        {
        }
        

        public DbSet<Eczane> Eczanes { get; set; }
        public DbSet<EczaneGrup> EczaneGrups { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Sehir> Sehirs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<EczaneUser> EczaneUserlar { get; set; }

        public DbSet<Alim> Alimlar { get; set; }
        public DbSet<AlimDurum> AlimDurumlar { get; set; }
        public DbSet<EczaneGrup> EczaneGruplar { get; set; }

        public DbSet<Eczane> Eczaneler { get; set; }
        public DbSet<Grup> Gruplar { get; set; }

        public DbSet<IlacDurum> IlacDurumlar { get; set; }
        public DbSet<Ilac> Ilaclar { get; set; }
        public DbSet<ITStransferDurum> ITStransferDurumlar { get; set; }
        public DbSet<QRKod> QRKodlar { get; set; }
        public DbSet<Log> Loglar { get; set; }
        public DbSet<Sehir> Sehirler { get; set; }
        public DbSet<Teklif> Teklifler { get; set; }
        public DbSet<TeklifDurum> TeklifDurumlar { get; set; }
        public DbSet<TeklifTur> TeklifTurler { get; set; }
        public DbSet<YayinlamaTur> YayinlamaTurler { get; set; }

        public DbSet<MenuAltRole> MenuAltRoles { get; set; }     
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuAlt> MenuAlts { get; set; }
        public DbSet<MenuRole> MenuRoles { get; set; }

    }
}
