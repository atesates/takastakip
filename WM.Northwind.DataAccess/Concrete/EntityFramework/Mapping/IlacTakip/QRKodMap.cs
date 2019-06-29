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
    public class QRKodMap : EntityTypeConfiguration<QRKod>
    {
        public QRKodMap()
        {
            this.HasKey(t => t.Id);
            this.ToTable("QRKodlar");

            #region columns
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.QRKodu).HasColumnName("QRKodu");
            this.Property(t => t.AlimId).HasColumnName("AlimId");
            #endregion

            #region properties
            this.Property(t => t.Id)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            this.Property(t => t.Id).IsRequired();
            this.Property(t => t.QRKodu).IsRequired()
                        .HasMaxLength(100)
                        .HasColumnAnnotation("Index",
                         new IndexAnnotation(
                                     new IndexAttribute("UN_QRKodlar")
                                      {
                                                  IsUnique = true,
                                                  Order = 1
                                      }));
            this.Property(t => t.AlimId).IsRequired()
                        .HasColumnAnnotation("Index",
                         new IndexAnnotation(
                                     new IndexAttribute("UN_QRKodlar")
                                      {
                                                  IsUnique = true,
                                                  Order = 2
                                      }));
            #endregion

            #region relationship
            #endregion
        }
    }
}