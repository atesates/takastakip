using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class EczaneGrup : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Eczane")]
        public int EczaneId { get; set; }
        [Display(Name = "Grup")]
        public int GrupId { get; set; }
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
    

        public virtual Eczane Eczane { get; set; }
        public virtual Grup Grup { get; set; }
        public virtual List<Katilim> Katilimlar { get; set; }
        public virtual List<Alim> Alimlar { get; set; }
        public virtual List<Teklif> Teklifler { get; set; }
        public virtual List<Talep> Talepler { get; set; }


    }
}