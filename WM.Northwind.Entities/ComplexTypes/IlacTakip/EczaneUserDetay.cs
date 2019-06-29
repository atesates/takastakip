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
    public class EczaneUserDetay: IComplexType
 { 
        public int Id { get; set; }
        [Display(Name = "User")]
        public int UserId { get; set; }
        [Display(Name = "Eczane")]
        public int EczaneId { get; set; }
        [Display(Name = "User")]
        public string UserAdi { get; set; }
        [Display(Name = "Eczane")]
        public string EczaneAdi { get; set; }

    } 
} 