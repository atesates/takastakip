using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class Ilac : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Adı")]
        public string Adi { get; set; }
        public double Barkod { get; set; }
        public string AtcKodu { get; set; }
        public string AtcAdi { get; set; }
        public string Firma { get; set; }
        public int IlacDurumId { get; set; }
        public string Aciklama { get; set; }
        public float? EtiketFiyati { get; set; }

        public virtual List<Teklif> Teklifler { get; set; }
        public virtual List<Talep> Talepler { get; set; }


    }
}