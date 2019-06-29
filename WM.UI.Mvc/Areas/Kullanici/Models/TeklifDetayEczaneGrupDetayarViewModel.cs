using System.Collections.Generic;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;

namespace WM.UI.Mvc.Areas.Kullanici.Models
{
    public class TeklifDetayEczaneGrupDetayarViewModel
    {
        public string Keyword { get; set; }
        public Pager Pager { get; set; }

        public List<EczaneGrupDetay> EczaneGrupDetaylar { get; set; }
        public List<TeklifDetay> TeklifDetay { get; set; }
    }
}