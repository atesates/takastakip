using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class AlimGroupByIlacId : IEntity
    {        
        public string AlimDurumAdi { get; set; }
        public int AlimDurumId { get; set; }
        [Display(Name = "Alım Tarihi")]
        public DateTime AlimTarihi { get; set; }   
        
        [Display(Name = "İlaç")]
        public string IlacAdi { get; set; }
        public int IlacId { get; set; }
        public int BuAyHareketleri { get; set; }
        public int DagitimToplami { get; set; }
        public int DagitilanTeklifSayisi { get; set; }
        public int GecenAyHareketleri { get; set; }
        [Display(Name = "Grup")]
        public string GrupAdi { get; set; }
        public int GrubaGirdigiTeklifSayisi { get; set; }
        [Display(Name = "Kayıt Tarihi")]
        public DateTime KayitTarihi { get; set; }
        public int Kalan { get; set; }
        [Display(Name = "Net Fiyat")]
        public float NetFiyat { get; set; }
        [Display(Name = "Dağıtıcı")]
        public string TeklifiVerenEczaneAdi { get; set; }  
        public int ToplamAlimMiktari { get; set; }
        public float TekliflerdenKazandirdigiMiktar { get; set; }
        public float ToplamFiyat { get; set; }
        [Display(Name = "Yayınlama Türü")]
        public string YayinlamaTurAdi { get; set; }
       
      
        public virtual AlimDurum AlimDurum { get; set; }
        public virtual EczaneGrup EczaneGrup { get; set; }
        public virtual ITStransferDurum ITStransferDurum { get; set; }
        public virtual Teklif Teklif { get; set; }
        




    }
}