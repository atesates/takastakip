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
    public class AlimMap : EntityTypeConfiguration<Alim>
    {
        public AlimMap()
        {
            this.HasKey(t => t.Id);
            this.ToTable("Alimlar");

            #region columns
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.EczaneGrupId).HasColumnName("EczaneGrupId");
            this.Property(t => t.Miktar).HasColumnName("Miktar");
            this.Property(t => t.AlimTarihi).HasColumnName("AlimTarihi");
            this.Property(t => t.TeslimAlimTarihi).HasColumnName("TeslimAlimTarihi");
            this.Property(t => t.GonderimTarihi).HasColumnName("GonderimTarihi");
            this.Property(t => t.AlimDurumId).HasColumnName("AlimDurumId");
            this.Property(t => t.ITStransferDurumId).HasColumnName("ITStransferDurumId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            #endregion

            #region properties
            this.Property(t => t.Id)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            this.Property(t => t.Id).IsRequired();
            this.Property(t => t.EczaneGrupId).IsRequired();
            this.Property(t => t.Miktar).IsRequired();
            this.Property(t => t.AlimTarihi).IsRequired();
            this.Property(t => t.TeslimAlimTarihi).IsOptional();
            this.Property(t => t.GonderimTarihi).IsOptional();
            this.Property(t => t.AlimDurumId).IsRequired();
            this.Property(t => t.ITStransferDurumId).IsRequired();
            this.Property(t => t.TeklifId).IsRequired();
            #endregion

            #region relationship
            this.HasRequired(t => t.AlimDurum)
                        .WithMany(et => et.Alimlar)
                        .HasForeignKey(t =>t.AlimDurumId)
                        .WillCascadeOnDelete(false);
            this.HasRequired(t => t.EczaneGrup)
                        .WithMany(et => et.Alimlar)
                        .HasForeignKey(t =>t.EczaneGrupId)
                        .WillCascadeOnDelete(false);
            this.HasRequired(t => t.ITStransferDurum)
                        .WithMany(et => et.Alimlar)
                        .HasForeignKey(t =>t.ITStransferDurumId)
                        .WillCascadeOnDelete(false);
            this.HasRequired(t => t.Teklif)
                        .WithMany(et => et.Alimlar)
                        .HasForeignKey(t => t.TeklifId)
                        .WillCascadeOnDelete(false);
            #endregion
        }
    }
}