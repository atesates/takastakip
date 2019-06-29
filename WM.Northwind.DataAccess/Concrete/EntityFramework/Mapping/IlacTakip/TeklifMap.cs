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
    public class TeklifMap : EntityTypeConfiguration<Teklif>
    {
        public TeklifMap()
        {
            this.HasKey(t => t.Id);
            this.ToTable("Teklifler");

            #region columns
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.IlacId).HasColumnName("IlacId");
            this.Property(t => t.EtiketFiyati).HasColumnName("EtiketFiyati");
            this.Property(t => t.HedeflenenAlim).HasColumnName("HedeflenenAlim");
            this.Property(t => t.NetFiyat).HasColumnName("NetFiyat");
            this.Property(t => t.DepoFiyat).HasColumnName("DepoFiyat");
            this.Property(t => t.TeklifiVerenEczaneGrupId).HasColumnName("TeklifiVerenEczaneGrupId");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.BitisTarihi).HasColumnName("BitisTarihi");
            this.Property(t => t.MalFazlasi).HasColumnName("MalFazlasi");
            this.Property(t => t.Maksimum).HasColumnName("Maksimum");
            this.Property(t => t.Minimum).HasColumnName("Minimum");
            this.Property(t => t.BaslangicTarihi).HasColumnName("BaslangicTarihi");
            this.Property(t => t.YayinlamaTurId).HasColumnName("YayinlamaTurId");
            this.Property(t => t.TeklifTurId).HasColumnName("TeklifTurId");
            this.Property(t => t.AlimMiktari).HasColumnName("AlimMiktari");
            this.Property(t => t.TeklifDurumId).HasColumnName("TeklifDurumId");
            this.Property(t => t.IlacMiad).HasColumnName("IlacMiad");
            #endregion

            #region properties
            this.Property(t => t.Id)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            this.Property(t => t.Id).IsRequired();
            this.Property(t => t.IlacId).IsRequired();
            this.Property(t => t.EtiketFiyati).IsRequired();
            this.Property(t => t.HedeflenenAlim).IsRequired();
            this.Property(t => t.NetFiyat).IsRequired();
            this.Property(t => t.DepoFiyat).IsRequired();
            this.Property(t => t.TeklifiVerenEczaneGrupId).IsRequired();
            this.Property(t => t.KayitTarihi).IsRequired();
            this.Property(t => t.BitisTarihi).IsOptional();
            this.Property(t => t.MalFazlasi).IsRequired();
            this.Property(t => t.Maksimum).IsRequired();
            this.Property(t => t.Minimum).IsRequired();
            this.Property(t => t.BaslangicTarihi).IsRequired();
            this.Property(t => t.YayinlamaTurId).IsRequired();
            this.Property(t => t.TeklifTurId).IsRequired();
            this.Property(t => t.AlimMiktari).IsRequired();
            this.Property(t => t.TeklifDurumId).IsRequired();
            this.Property(t => t.IlacMiad).IsRequired();
            #endregion

            #region relationship
            this.HasRequired(t => t.EczaneGrup)
                        .WithMany(et => et.Teklifler)
                        .HasForeignKey(t => t.TeklifiVerenEczaneGrupId)
                        .WillCascadeOnDelete(false);
            this.HasRequired(t => t.TeklifDurum)
                        .WithMany(et => et.Teklifler)
                        .HasForeignKey(t =>t.TeklifDurumId)
                        .WillCascadeOnDelete(false); 
            this.HasRequired(t => t.TeklifTur)
                        .WithMany(et => et.Teklifler)
                        .HasForeignKey(t =>t.TeklifTurId)
                        .WillCascadeOnDelete(false); 
            this.HasRequired(t => t.Ilac)
                    .WithMany(et => et.Teklifler)
                    .HasForeignKey(t => t.IlacId)
                    .WillCascadeOnDelete(false);
            this.HasRequired(t => t.YayinlamaTur)
                        .WithMany(et => et.Teklifler)
                        .HasForeignKey(t =>t.YayinlamaTurId)
                        .WillCascadeOnDelete(false);
            #endregion
        }
    }
}