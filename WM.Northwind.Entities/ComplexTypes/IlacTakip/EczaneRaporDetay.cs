using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;
using WM.Northwind.Entities.Concrete.IlacTakip;


namespace WM.Northwind.Entities.ComplexTypes.IlacTakip
{
    public class EczaneRaporDetay : IComplexType
 { 
        public int Id { get; set; }
        public int EczaneId { get; set; }
        public int EczaneGrupId { get; set; }
        public int GrupId { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string Adi { get; set; }
        public string EczaneAdi { get; set; }
        [Display(Name = "Grup Adı")]
        public string GrupAdi { get; set; }

    } 
} 