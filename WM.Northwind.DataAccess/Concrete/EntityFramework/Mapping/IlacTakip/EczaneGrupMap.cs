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
    public class EczaneGrupMap : EntityTypeConfiguration<EczaneGrup>
    {
        public EczaneGrupMap()
        {
            this.HasKey(t => t.Id);
            this.ToTable("EczaneGruplar");

            #region columns
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.EczaneId).HasColumnName("EczaneId");
            this.Property(t => t.GrupId).HasColumnName("GrupId");
            this.Property(t => t.BaslangicTarihi).HasColumnName("BaslangicTarihi");
            this.Property(t => t.BitisTarihi).HasColumnName("BitisTarihi");
           
            #endregion

            #region properties
            this.Property(t => t.Id)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            this.Property(t => t.Id).IsRequired();
            this.Property(t => t.EczaneId).IsRequired()
                        .HasColumnAnnotation("Index",
                         new IndexAnnotation(
                                     new IndexAttribute("UN_EczaneGruplar")
                                      {
                                                  IsUnique = true,
                                                  Order = 1
                                      }));
            this.Property(t => t.GrupId).IsRequired()
                        .HasColumnAnnotation("Index",
                         new IndexAnnotation(
                                     new IndexAttribute("UN_EczaneGruplar")
                                      {
                                                  IsUnique = true,
                                                  Order = 2
                                      }));
            this.Property(t => t.BaslangicTarihi).IsRequired()
                        .HasColumnAnnotation("Index",
                         new IndexAnnotation(
                                     new IndexAttribute("UN_EczaneGruplar")
                                      {
                                                  IsUnique = true,
                                                  Order = 3
                                      }));
            this.Property(t => t.BitisTarihi).IsOptional();
           
                   
            #endregion

            #region relationship
            this.HasRequired(t => t.Eczane)
                        .WithMany(et => et.EczaneGruplar)
                        .HasForeignKey(t =>t.EczaneId).WillCascadeOnDelete(false);
            this.HasRequired(t => t.Grup)
                        .WithMany(et => et.EczaneGruplar)
                        .HasForeignKey(t =>t.GrupId).WillCascadeOnDelete(false);
            #endregion
        }
    }
}