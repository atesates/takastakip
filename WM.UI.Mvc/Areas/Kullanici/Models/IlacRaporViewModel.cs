using System.Collections.Generic;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using System.ComponentModel.DataAnnotations;
using System;

namespace WM.UI.Mvc.Areas.Kullanici.Models
{
    public class IlacRaporViewModel
    {
       public List<AlimDetay> AlimDetaylar { get; set; }
       public List<Ilac> Ilaclar { get; set; }
       public List<EczaneGrupDetay> EczaneGrupDetaylar { get; set; }
       public List<TeklifDetay> TeklifDetaylar { get; set; }
       public List<AlimGroupByIlacId> AlimlarGroupByIlacIdler { get; set; }
        public Pager Pager { get; set; }

    }
}