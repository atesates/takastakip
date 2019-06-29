using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class AlimGroupByTeklifId : IEntity
    {        
        [Display(Name = "Alım Durumu")]
        public string AlimDurumAdi { get; set; }
        public int AlimDurumId { get; set; }
        public int ToplamAlimlar { get; set; }
        [Display(Name = "Alım Tarihi")]
        public DateTime AlimTarihi { get; set; }
       
        public string EczaneGln { get; set; }
        [Display(Name = "Eczane")]
        public string AliciEczaneAdi { get; set; }
        [Display(Name = "BitişTarihi")]
        public DateTime? BitisTarihi { get; set; }          
        [Display(Name = "Depo Fiyatı")]
        public float DepoFiyat { get; set; }
        [Display(Name = "Etiket Fiyatı")]
        public float EtiketFiyati { get; set; }
        [Display(Name = "Grup")]
        public string GrupAdi { get; set; }
        [Display(Name = "Hedef Alım")]
        public int HedeflenenAlim { get; set; }
        [Display(Name = "İlaç Miadı")]
        public DateTime? IlacMiad { get; set; }
        public int IlacId { get; set; }
        public int AliciEczaneGrupId { get; set; }
        [Display(Name = "İlaç Adı")]
        public string IlacAdi { get; set; }
        [Display(Name = "Kayıt Tarihi")]
        public DateTime KayitTarihi { get; set; }
        public int Kalan { get; set; }
        [Display(Name = "M.F.")]
        public int MalFazlasi { get; set; }
        [Display(Name = "Max")]
        public int Maksimum { get; set; }
        [Display(Name = "Min")]
        public int Minimum { get; set; }
        [Display(Name = "Net Fiyat")]
        public float NetFiyat { get; set; }
        [Display(Name = "Dağıtıcı")]
        public string TeklifiVerenEczaneAdi { get; set; }
        public int TeklifDurumId { get; set; }
        public int TeklifId { get; set; }
        [Display(Name = "Toplam Alım Miktarı")]
        public int ToplamAlimMiktari { get; set; }
        public float ToplamFiyat { get; set; }
        [Display(Name = "Yayınlama Türü")]
        public string YayinlamaTurAdi { get; set; }

        [Display(Name = "Teklif Durumu")]
        public string TeklifDurumAdi { get; set; }
        [Display(Name = "Dağıtıcı")]
        public string TeklifVerenEczaneAdi { get; set; }
        [Display(Name = "Teklif Türü")]
        public string TeklifTurAdi { get; set; }
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime? BaslangicTarihi { get; set; }
        
        [Display(Name = "ITS Transfer Durumu")]
        public string ITStransferDurumAdi { get; set; }
        [Display(Name = "Grup")]
        public string TeklifVerenEczaneGrupAdi { get; set; }
        [Display(Name = "Miktar")]
        public int Miktar { get; set; }
        [Display(Name = "Eczane")]
        public string EczaneAdi { get; set; }
        [Display(Name = "Gönderim Tarihi")]
        public DateTime? GonderimTarihi { get; set; }
        [Display(Name = "Teslim Alım Tarihi")]
        public DateTime? TeslimAlimTarihi { get; set; }
        public int TeklifVerenEczaneGrupId { get; set; }
        public int Id { get; set; }


        public virtual AlimDurum AlimDurum { get; set; }
        public virtual EczaneGrup EczaneGrup { get; set; }
        public virtual ITStransferDurum ITStransferDurum { get; set; }
        public virtual Teklif Teklif { get; set; }
        




    }
}