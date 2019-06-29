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
    public class EczaneDetay: IComplexType
 { 
        public int Id { get; set; }
        public string Adi { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
        public string Email { get; set; }
        public string EczaneGln { get; set; }
        public string FaturaAdSoyad { get; set; }
        public string VergiNumarasi { get; set; }
        public string VergiDairesi { get; set; }
        public int? SehirId { get; set; }
        public string Telefon2 { get; set; }
        public decimal Bakiye { get; set; }
        public string SehirAdi { get; set; }

    } 
} 