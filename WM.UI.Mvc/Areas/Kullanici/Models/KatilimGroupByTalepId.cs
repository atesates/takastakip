using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class KatilimGroupByTalepId : IEntity
    {        
       
        public int Katilimim { get; set; }
        [Display(Name = "Katilım Tarihi")]
        public DateTime KatilimTarihi { get; set; }
       
        public string EczaneGln { get; set; }
        [Display(Name = "Eczane")]
        public string KatilimYapanEczaneAdi { get; set; }
        [Display(Name = "BitişTarihi")]
        public DateTime? BitisTarihi { get; set; }          
        [Display(Name = "Depo Fiyatı")]
        public float DepoFiyat { get; set; }
        [Display(Name = "Grup")]
        public string GrupAdi { get; set; }
        [Display(Name = "Talep Miktarı")]
        public int TalepMiktari { get; set; }
        public int IlacId { get; set; }
        [Display(Name = "İlaç Adı")]
        public string IlacAdi { get; set; }
        [Display(Name = "Kayıt Tarihi")]
        public DateTime KayitTarihi { get; set; }
        public int Kalan { get; set; }
        [Display(Name = "Max")]
        public int Maksimum { get; set; }
        [Display(Name = "Min")]
        public int Minimum { get; set; }
        [Display(Name = "Talep Sahibi")]
        public string TalepVerenEczaneAdi { get; set; }
        public int TalepId { get; set; }
        public int ToplamKatilimMiktari { get; set; }
        [Display(Name = "Talep Durumu")]
        public string TalepDurumAdi { get; set; }
        [Display(Name = "Grup")]
        public string TalepVerenEczaneGrupAdi { get; set; }
        public int Id { get; set; }
        public int Miktar { get; set; }
        public int TalepVerenEczaneGrupId { get; set; }
        public int TalepDurumId { get; set; }




        public virtual EczaneGrup EczaneGrup { get; set; }
        public virtual Talep Talep { get; set; }
        




    }
}