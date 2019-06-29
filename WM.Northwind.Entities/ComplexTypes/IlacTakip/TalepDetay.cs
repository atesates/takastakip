using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;
using WM.Northwind.Entities.Concrete.IlacTakip;


namespace WM.Northwind.Entities.ComplexTypes.IlacTakip
{
    public class TalepDetay: IComplexType
 { 
        public int Id { get; set; }
        public int TalepVerenEczaneGrupId { get; set; }
        [Display(Name = "Talep Miktarı")]
        public int TalepMiktari { get; set; }
        [Display(Name = "Depo Fiyatı")]
        public float DepoFiyati { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public string Aciklama { get; set; }
        public int TalepDurumId { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int IlacId { get; set; }
        public string TalepVerenEczaneGrupAdi { get; set; }
        [Display(Name = "Talep Sahibi")]
        public string TalepVerenEczaneAdi { get; set; }
        [Display(Name = "Talep Durumu")]
        public string TalepDurumAdi { get; set; }
        [Display(Name = "İlaç")]
        public string IlacAdi { get; set; }

        public bool Checked { get; set; }
        public bool Expanded { get; set; }
    } 
} 