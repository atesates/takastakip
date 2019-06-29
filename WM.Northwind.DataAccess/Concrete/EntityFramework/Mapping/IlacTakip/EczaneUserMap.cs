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
    public class EczaneUserMap : EntityTypeConfiguration<EczaneUser>
    {
        public EczaneUserMap()
        {
            this.HasKey(t => t.Id);
            this.ToTable("EczaneUserlar");

            #region columns
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.EczaneId).HasColumnName("EczaneId");
            #endregion

            #region properties
            this.Property(t => t.Id)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            this.Property(t => t.Id).IsRequired();
            this.Property(t => t.UserId).IsRequired();
            this.Property(t => t.EczaneId).IsRequired();
            #endregion

            #region relationship
            this.HasRequired(t => t.Eczane)
                        .WithMany(et => et.EczaneUserlar)
                        .HasForeignKey(t =>t.EczaneId).WillCascadeOnDelete(false);
            this.HasRequired(t => t.User)
                        .WithMany(et => et.EczaneUserlar)
                        .HasForeignKey(t =>t.UserId).WillCascadeOnDelete(false);
            #endregion
        }
    }
}