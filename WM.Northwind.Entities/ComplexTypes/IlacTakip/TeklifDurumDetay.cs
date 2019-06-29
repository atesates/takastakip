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
    public class TeklifDurumDetay: IComplexType
 { 
        [Display(Name = "Teklif Durumu")]
        public int Id { get; set; }
        public string Adi { get; set; }

    } 
} 