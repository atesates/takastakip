using System.Collections.Generic;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;

namespace WM.UI.Mvc.Areas.Kullanici.Models
{
    public class EczaneRaporViewModel
    {
        public int EczaneId { get;  set; }
        public string EczaneAdi { get;  set; }
        public int GrupId { get;  set; }
        public int EczaneGrupId { get;  set; }
        public string GrupAdi { get;  set; }
 
        public List<Alim> Alimlar { get; set; }
        public List<EczaneGrupDetay> EczaneGrupDetaylar { get; set; }
        public float Gelirler { get; set; }   
        public float Giderler { get; set;}
        public float Bakiye { get; set; }
        public Pager Pager { get; set; }


    }
}