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
    public class EczaneGrupDetay: IComplexType
 { 
        public int Id { get; set; }
        public int EczaneId { get; set; }
        public int GrupId { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string Adi { get; set; }
        public string Adres { get; set; }
        public string Telefon { get; set; }
        public string Telefon2 { get; set; }
        public string Email { get; set; }
        public string FaturaAdSoyad { get; set; }
        public string VergiDairesi { get; set; }
        public string Sehir { get; set; }
        public string VergiNumarasi { get; set; }
        public string EczaneAdi { get; set; }
        public string EczaneGln { get; set; }
        [Display(Name = "Grup Adı")]
        public string GrupAdi { get; set; }
        public bool Checked { get; set; }
        public bool Expanded { get; set; }

        public Eczane Eczane { get; set; }
    } 
} 