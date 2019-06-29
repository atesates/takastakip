using System.Collections.Generic;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;

namespace WM.UI.Mvc.Areas.Kullanici.Models
{
    public class TalepDetayKatilimDetaylarViewModel
    {
        public string Keyword { get; set; }
        public Pager Pager { get; set; }

        public TalepDetay TalepDetay { get; set; }
        public List<KatilimDetay> KatilimDetaylar { get; set; }
    }
}