using System.Collections.Generic;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;

namespace WM.UI.Mvc.Areas.Kullanici.Models
{
    public class AlimGroupByTeklifIdAlimDetaylarViewModel
    {
        public string Keyword { get; set; }
        public Pager Pager { get; set; }
        
        public AlimGroupByTeklifId AlimGroupByTeklifId { get; set; }
        public List<AlimDetay> AlimDetaylar { get; set; }

    }
}