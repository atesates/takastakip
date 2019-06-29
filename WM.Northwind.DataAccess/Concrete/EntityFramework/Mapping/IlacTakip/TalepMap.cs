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
    public class TalepMap : EntityTypeConfiguration<Talep>
    {
        public TalepMap()
        {
            this.HasKey(t => t.Id);
            this.ToTable("Talepler");

            #region columns
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TalepVerenEczaneGrupId).HasColumnName("TalepVerenEczaneGrupId");
            this.Property(t => t.TalepMiktari).HasColumnName("TalepMiktari");
            this.Property(t => t.DepoFiyati).HasColumnName("DepoFiyati");
            this.Property(t => t.Minimum).HasColumnName("Minimum");
            this.Property(t => t.Maximum).HasColumnName("Maximum");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.TalepDurumId).HasColumnName("TalepDurumId");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.BitisTarihi).HasColumnName("BitisTarihi");
            this.Property(t => t.IlacId).HasColumnName("IlacId");
            #endregion

            #region properties
            this.Property(t => t.Id)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            this.Property(t => t.Id).IsRequired();
            this.Property(t => t.TalepVerenEczaneGrupId).IsRequired();
            this.Property(t => t.TalepMiktari).IsRequired();
            this.Property(t => t.DepoFiyati).IsRequired();
            this.Property(t => t.Minimum).IsRequired();
            this.Property(t => t.Maximum).IsRequired();
            this.Property(t => t.Aciklama).IsRequired()
                        .HasMaxLength(50);
            this.Property(t => t.TalepDurumId).IsRequired();
            this.Property(t => t.KayitTarihi).IsRequired();
            this.Property(t => t.BitisTarihi).IsOptional();
            this.Property(t => t.IlacId).IsRequired();
            #endregion

            #region relationship
            this.HasRequired(t => t.EczaneGrup)
                        .WithMany(et => et.Talepler)
                        .HasForeignKey(t =>t.TalepVerenEczaneGrupId)
                        .WillCascadeOnDelete(false);
            this.HasRequired(t => t.Ilac)
                        .WithMany(et => et.Talepler)
                        .HasForeignKey(t =>t.IlacId)
                        .WillCascadeOnDelete(false);
            this.HasRequired(t => t.TalepDurum)
                        .WithMany(et => et.Talepler)
                        .HasForeignKey(t =>t.TalepDurumId)
                        .WillCascadeOnDelete(false);
            this.HasRequired(t => t.Ilac)
                    .WithMany(et => et.Talepler)
                    .HasForeignKey(t => t.IlacId)
                    .WillCascadeOnDelete(false);
            #endregion
        }
    }
}