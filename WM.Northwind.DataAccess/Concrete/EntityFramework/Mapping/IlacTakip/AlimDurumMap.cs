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
    public class AlimDurumMap : EntityTypeConfiguration<AlimDurum>
    {
        public AlimDurumMap()
        {
            this.HasKey(t => t.Id);
            this.ToTable("AlimDurumlar");

            #region columns
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Adi).HasColumnName("Adi");
            this.Property(t => t.Adi).HasColumnName("AliciTarafMi");
            #endregion

            #region properties
            this.Property(t => t.Id)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            this.Property(t => t.Id).IsRequired();
            this.Property(t => t.Adi).IsRequired()
                        .HasMaxLength(50);
            this.Property(t => t.AliciTarafiMi).IsOptional();
                       
            #endregion

            #region relationship

            #endregion
        }
    }
}