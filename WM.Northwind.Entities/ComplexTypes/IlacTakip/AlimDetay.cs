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
    public class AlimDetay: IComplexType
    {
        public string Adres { get; set; }

        [Display(Name = "Alım Durum")]
        public string AlimDurumAdi { get; set; }
        public int AlimDurumId { get; set; }
        [Display(Name = "Alım Tarihi")]
        public DateTime AlimTarihi { get; set; }
        [Display(Name = "Baş. Tarihi")]
        public DateTime BaslangicTarihi { get; set; }
        [Display(Name = "BitişTarihi")]
        public DateTime? BitisTarihi { get; set; }
        public float DepoFiyati { get; set; }

        [Display(Name = "Eczane")]
        public string EczaneAdi { get; set; }
        public string EczaneGln { get; set; }
        public float EtiketFiyati { get; set; }

        [Display(Name = "Yönetici")]
        public string FaturaAdSoyad { get; set; }
        [Display(Name = "Gönderim Tarihi")]
        public DateTime? GonderimTarihi { get; set; }
        public string Email { get; set; }
        public int EczaneId { get; set; }
        public int EczaneGrupId { get; set; }
        public int GrupId { get; set; }
        [Display(Name = "Grup")]
        public string GrupAdi { get; set; }
        public int HedeflenenAlim { get; set; }
        public int Id { get; set; }
        [Display(Name = "İlaç Miadı")]
        public DateTime? IlacMiad { get; set; }
        [Display(Name = "ITS Transfer Durumu")]
        public string ITStransferDurumAdi { get; set; }
        [Display(Name = "İlaç")]
        public string IlacAdi { get; set; }
        public int IlacId { get; set; }
        [Display(Name = "ITS Transfer Durumu")]
        public int ITStransferDurumId { get; set; }
        public int MalFazlasi { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int Miktar { get; set; }
        [Display(Name = "Kayıt Tarihi")]
        public DateTime KayitTarihi { get; set; }
        public string AlimTarihiString => String.Format("{0:yyyy MM dd, ddd}", AlimTarihi); //{ get; set; }
        public float NetFiyat { get; set; }
        public string Sehir { get; set; }
        public int? SehirId { get; set; }
        [Display(Name = "TeklifId")]
        public int TeklifId { get; set; }
        [Display(Name = "Grup")]
        public string TeklifVerenEczaneGrupAdi { get; set; }
        [Display(Name = "Toplam Teklif Miktarı")]
        public int ToplamTeklifMiktari { get; set; }
        [Display(Name = "Dağıtıcı")]
        public string TeklifVerenEczaneAdi { get; set; }
        [Display(Name = "Teklif Durumu")]
        public string TeklifDurumAdi { get; set; }
        [Display(Name = "Teslim Alım Tarihi")]
        public DateTime? TeslimAlimTarihi { get; set; }
        public string TeklifTurAdi { get; set; }
        public int TeklifVerenEczaneGrupId { get; set; }
        public int TeklifDurumId { get; set; }
        [Display(Name = "Toplam Alım Miktarı")]
        public int ToplamAlimMiktari { get; set; }
        public int Kalan { get; set; }
        public List<Ilac> Ilaclar { get; set; }
        public List<Teklif> Teklifler { get; set; }
        public List<TeklifDetay> TeklifDetaylar { get; set; }
        public bool Checked { get; set; }
        public bool Expanded { get; set; }

    }
} 