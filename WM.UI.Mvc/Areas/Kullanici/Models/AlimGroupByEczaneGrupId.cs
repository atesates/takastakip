using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class AlimGroupByEczaneGrupId : IEntity
    {      
        public float Bakiye { get; set; }
        public float Gelir { get; set; }
        public float Gider { get; set; }
        public string EczaneGln { get; set; }
        [Display(Name = "Eczane")]
        public string EczaneAdi { get; set; }
        public int EczaneGrupId { get; set; }
        public int BuAyHareketleri { get; set; }
        public int DagitimToplami { get; set; }
        public int DagittigiTeklifSayisi { get; set; }
        public int GecenAyHareketleri { get; set; }
        public int GrupId { get; set; }
        [Display(Name = "Grup")]
        public string GrupAdi { get; set; }
        public int GrubaGirdigiTeklifSayisi { get; set; }       
        public int ToplamAlimMiktari { get; set; }
        public float TekliflerdenKazandirdigiMiktar { get; set; }
        
        //public virtual AlimDurum AlimDurum { get; set; }
        //public virtual EczaneGrup EczaneGrup { get; set; }
        //public virtual ITStransferDurum ITStransferDurum { get; set; }
        //public virtual Teklif Teklif { get; set; }





    }
}