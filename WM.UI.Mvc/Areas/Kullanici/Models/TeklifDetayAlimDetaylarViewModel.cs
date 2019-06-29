using System.Collections.Generic;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;

namespace WM.UI.Mvc.Areas.Kullanici.Models
{
    public class TeklifDetayAlimDetaylarViewModel
    {
        public string Keyword { get; set; }
        public Pager Pager { get; set; }

        public TeklifDetay TeklifDetay { get; set; }
        public List<AlimDetay> AlimDetaylar { get; set; }
    }
}