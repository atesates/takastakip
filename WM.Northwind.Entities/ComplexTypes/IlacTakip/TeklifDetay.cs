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
    public class TeklifDetay: IComplexType
    {
        [Display(Name = "Toplam Teklif Miktarı")]
        public int ToplamTeklifMiktari { get; set; }
        [Display(Name = "Teklif Miktarı")]
        public int AlimMiktari { get; set; }
        public double Barkod { get; set; }
        [Display(Name = "Baş. Tarihi")]
        public DateTime BaslangicTarihi { get; set; }
        [Display(Name = "BitişTarihi")]
        public DateTime? BitisTarihi { get; set; }
        [Display(Name = "Depo Fiyatı")]
        public float DepoFiyat { get; set; }
        [Display(Name = "Etiket Fiyatı")]
        public float EtiketFiyati { get; set; }
        //[Display(Name = "Grup")]
        // public string GrupAdi { get; set; }
       // [Display(Name = "Gönderim Tarihi")]
        public DateTime? GonderimTarihi { get; set; }

        [Display(Name = "Hedef Alım")]
        public int HedeflenenAlim { get; set; }
        [Display(Name = "İlaç Miadı")]
        public DateTime? IlacMiad { get; set; }
        public int Id { get; set; }
        public int IlacId { get; set; }
        [Display(Name = "İlaç Adı")]
        public string IlacAdi { get; set; }
        [Display(Name = "Kayıt Tarihi")]
        public DateTime KayitTarihi { get; set; }
        [Display(Name = "M.F.")]
        public int MalFazlasi { get; set; }
        [Display(Name = "Max")]
        public int Maksimum { get; set; }
        [Display(Name = "Min")]
        public int Minimum { get; set; }     
        [Display(Name = "Net Fiyat")]
        public float NetFiyat { get; set; }
        public int TeklifTurId { get; set; }
        [Display(Name = "Grup Adi")]
        public string TeklifiVerenEczaneGrupAdi { get; set; }
        public string TeklifiVerenEczaneGln { get; set; }
        public int TeklifiVerenEczaneGrupId { get; set; }
        public int TeklifDurumId { get; set; }  
        [Display(Name = "Dağıtıcı")]
        public string TeklifiVerenEczaneAdi { get; set; }    
        [Display(Name = "Teklif Türü")]
        public string TeklifTurAdi { get; set; }
        [Display(Name = "Teklif Durumu")]
        public string TeklifDurumAdi { get; set; }
        public int YayinlamaTurId { get; set; }
        [Display(Name = "Yayınlama Türü")]
        public string YayinlamaTurAdi { get; set; }
        public List<Grup> Gruplar { get; set; }
        public int? OzelEczaneGrupId { get; set; }
        public int BuTekliftenYapilanAlimSayisi { get; set; }
        public int BuTekliftenYapilanAlimMiktari { get; set; }

        public bool Checked { get; set; }
        public bool Expanded { get; set; }
    } 
} 