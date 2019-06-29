using System.Collections.Generic;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;

namespace WM.UI.Mvc.Areas.Kullanici.Models
{
    public class TeklifDetayViewModel
    {
        //public int EczaneId { get;  set; }
        //public string EczaneAdi { get;  set; }
        //public int GrupId { get;  set; }
        //public string GrupAdi { get;  set; }
  
        public List<EczaneGrup> EczaneGruplar { get; set; }
        public List<Eczane> Eczaneler { get; set;}
        public List<Ilac> Ilaclar { get; set; }
        public List<TeklifDetay> TeklifDetaylar { get; set; }
        public List<Alim> Alimlar { get; set; }
        public Pager Pager { get; set; }

    }
}