using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Data.Entity.Infrastructure.Annotations;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Entities.Concrete;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.Northwind.DataAccess.Concrete.EntityFramework.Mapping.IlacTakip
{
    public class IlacMap : EntityTypeConfiguration<Ilac>
    {
        public IlacMap()
        {
            this.HasKey(t => t.Id);
            this.ToTable("Ilaclar");

            #region columns
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Adi).HasColumnName("Adi");
            this.Property(t => t.Barkod).HasColumnName("Barkod");
            this.Property(t => t.AtcKodu).HasColumnName("AtcKodu");
            this.Property(t => t.AtcAdi).HasColumnName("AtcAdi");
            this.Property(t => t.Firma).HasColumnName("Firma");
            this.Property(t => t.IlacDurumId).HasColumnName("IlacDurumId");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");

            #endregion

            #region properties
            this.Property(t => t.Id)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Id).IsRequired();
            this.Property(t => t.Adi).IsRequired();
            this.Property(t => t.Barkod).IsRequired();
            this.Property(t => t.AtcKodu).IsRequired();
            this.Property(t => t.AtcAdi).IsRequired();
            this.Property(t => t.Firma).IsRequired();
            this.Property(t => t.IlacDurumId).IsRequired();
            this.Property(t => t.Aciklama).IsOptional();
            #endregion

            #region relationship
            #endregion
        }
    }
}