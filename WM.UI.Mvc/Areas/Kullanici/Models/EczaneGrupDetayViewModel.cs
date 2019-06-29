using System.Collections.Generic;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;

namespace WM.UI.Mvc.Areas.Kullanici.Models
{
    public class EczaneGrupDetayViewModel
    {
        //public int EczaneId { get;  set; }
        //public string EczaneAdi { get;  set; }
        //public int GrupId { get;  set; }
        //public string GrupAdi { get;  set; }
        //public int EczaneGrupID { get; set; }


        public List<Eczane> Eczaneler { get; set; }
        //public List<Grup> Gruplar { get; set; }
        public List<EczaneGrupDetay> EczaneGrupDetaylar { get; set;}
        public Pager Pager { get; set; }


    }
}