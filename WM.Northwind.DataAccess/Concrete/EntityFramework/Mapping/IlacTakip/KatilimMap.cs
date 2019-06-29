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
    public class KatilimMap : EntityTypeConfiguration<Katilim>
    {
        public KatilimMap()
        {
            this.HasKey(t => t.Id);
            this.ToTable("Katilimlar");

            #region columns
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.EczaneGrupId).HasColumnName("EczaneGrupId");
            this.Property(t => t.Miktar).HasColumnName("Miktar");
            this.Property(t => t.TalepId).HasColumnName("TalepId");
            this.Property(t => t.KatilimTarihi).HasColumnName("KatilimTarihi");
            #endregion

            #region properties
            this.Property(t => t.Id)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            this.Property(t => t.Id).IsRequired();
            this.Property(t => t.EczaneGrupId).IsRequired();
            this.Property(t => t.Miktar).IsRequired();
            this.Property(t => t.TalepId).IsRequired();
            this.Property(t => t.KatilimTarihi).IsRequired();
            #endregion

            #region relationship
            this.HasRequired(t => t.EczaneGrup)
                        .WithMany(et => et.Katilimlar)
                        .HasForeignKey(t =>t.EczaneGrupId)
                        .WillCascadeOnDelete(false);
            this.HasRequired(t => t.Talep)
                        .WithMany(et => et.Katilimlar)
                        .HasForeignKey(t =>t.TalepId)
                        .WillCascadeOnDelete(false);
            #endregion
        }
    }
}