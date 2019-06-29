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
    public class EczaneMap : EntityTypeConfiguration<Eczane>
    {
        public EczaneMap()
        {
            this.HasKey(t => t.Id);
            this.ToTable("Eczaneler");

            #region columns
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Adi).HasColumnName("Adi");
            this.Property(t => t.Telefon).HasColumnName("Telefon");
            this.Property(t => t.Adres).HasColumnName("Adres");
            this.Property(t => t.Email).HasColumnName("Email");
           
            this.Property(t => t.EczaneGln).HasColumnName("EczaneGln");
            this.Property(t => t.FaturaAdSoyad).HasColumnName("FaturaAdSoyad");
            this.Property(t => t.VergiNumarasi).HasColumnName("VergiNumarasi");
            this.Property(t => t.VergiDairesi).HasColumnName("VergiDairesi");
            this.Property(t => t.SehirId).HasColumnName("SehirId");
            this.Property(t => t.Telefon2).HasColumnName("Telefon2");
            #endregion

            #region properties
            this.Property(t => t.Id)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            this.Property(t => t.Id).IsRequired();
            //this.Property(t => t.Adi).IsRequired()
            //            .HasMaxLength(50)
            //            .HasColumnAnnotation("Index",
            //             new IndexAnnotation(
            //                         new IndexAttribute("UN_Eczaneler")
            //                         {
            //                             IsUnique = true,
            //                             Order = 1
            //                         }));
            this.Property(t => t.EczaneGln).IsRequired()
                        .HasMaxLength(50)
                        .HasColumnAnnotation("Index",
                         new IndexAnnotation(
                                     new IndexAttribute("UN_Eczaneler_Gln")
                                      {
                                                  IsUnique = true,
                                                  Order = 2
                                      }));
            this.Property(t => t.Email).IsRequired()
                        .HasMaxLength(100)
                        .HasColumnAnnotation("Index",
                         new IndexAnnotation(
                                     new IndexAttribute("UN_Eczaneler_Email")
                                      {
                                                  IsUnique = true,
                                                  Order = 3
                                      }));
            this.Property(t => t.Adres).IsOptional()
                        .HasMaxLength(50);
            this.Property(t => t.Telefon).IsOptional()
                           .HasMaxLength(50);
            this.Property(t => t.EczaneGln).IsRequired()
                        .HasMaxLength(50);
            this.Property(t => t.FaturaAdSoyad).IsOptional()
                        .HasMaxLength(50);
            this.Property(t => t.VergiNumarasi).IsOptional()
                        .HasMaxLength(50);
            this.Property(t => t.VergiDairesi).IsOptional()
                        .HasMaxLength(50);
            this.Property(t => t.SehirId).IsOptional();
            this.Property(t => t.Telefon2).IsOptional()
                        .HasMaxLength(50);
            #endregion

            #region relationship
         
            #endregion
        }
    }
}