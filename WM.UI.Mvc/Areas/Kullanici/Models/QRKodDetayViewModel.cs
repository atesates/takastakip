using System.Collections.Generic;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using System.ComponentModel.DataAnnotations;

namespace WM.UI.Mvc.Areas.Kullanici.Models
{
    public class QRKodDetayViewModel
    {
        //public int EczaneId { get;  set; }
        //public string EczaneAdi { get;  set; }
        //public int GrupId { get;  set; }
        //public string GrupAdi { get;  set; }
        public int AlimId { get; set; }
        [DataType(DataType.MultilineText)]
        public string QRKodlar { get; set; }
        //public List<QRKod> QRKodlar { get; set; }


    }
}