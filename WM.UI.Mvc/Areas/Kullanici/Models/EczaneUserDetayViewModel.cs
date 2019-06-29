using System.Collections.Generic;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.Authorization;

namespace WM.UI.Mvc.Areas.Kullanici.Models
{
    public class EczaneUserDetayViewModel
    {
        //public int EczaneId { get;  set; }
        //public string EczaneAdi { get;  set; }
        //public int GrupId { get;  set; }
        //public string GrupAdi { get;  set; }
        //public int EczaneGrupID { get; set; }


        public List<Eczane> Eczaneler { get; set; }
        public List<User> Userlar { get; set; }
        public List<EczaneUserDetay> EczaneUserDetaylar { get; set; }


    }
}