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
    public class KatilimDetay: IComplexType
 { 
        public int Id { get; set; }
        public int EczaneGrupId { get; set; }
        public int Miktar { get; set; }
        public int TalepId { get; set; }
        public DateTime KatilimTarihi { get; set; }
        public string EczaneGrupAdi { get; set; }
        [Display(Name = "Katılım Sahibi")]
        public string KatilimYapanEczaneAdi { get; set; }
        [Display(Name = "İlaç")]
        public string IlacAdi { get; set; }
        [Display(Name = "Talep Sahibi")]
        public int TalepVerenEczaneGrupId { get; set; }
        [Display(Name = "Talep Miktarı")]
        public int TalepMiktari { get; set; }
        [Display(Name = "Depo Fiyatı")]
        public float DepoFiyati { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        [Display(Name = "Açıklma")]
        public string Aciklama { get; set; }
        public int TalepDurumId { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int IlacId { get; set; }
        [Display(Name = "Talep Sahibi")]
        public string TalepVerenEczaneAdi { get; set; }
        [Display(Name = "Grup")]
        public string TalepVerenEczaneGrupAdi { get; set; }
        [Display(Name = "Talep Durumu")]
        public string TalepDurumAdi { get; set; }
        public string EczaneGln { get; set; }
        public bool Checked { get; set; }
        public bool Expanded { get; set; }
        [Display(Name = "Toplam Katılım Miktarı")]
        public int ToplamKatilimMiktari { get; set; }
        [Display(Name = "Kalan")]
        public int Kalan { get; set; }


    }
} 