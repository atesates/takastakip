using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class Eczane : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "EczaneAdı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        public string Adi { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }

        [Display(Name = "EPosta")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        [RegularExpression(@"^[a-zA-Z0-9_&.-]+@[a-zA-Z0-9.-]+\.[a-zA-Z0-9]{2,5}$", ErrorMessage = "Lütfen geçerli bir e-posta adresi yazınız..!")]
        public string Email { get; set; }

        [Display(Name = "EczaneGln")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        [RegularExpression(@"^[0-9]{13}$", ErrorMessage = "Lütfen geçerli bir GLN  yazınız..!")]
        public string EczaneGln { get; set; }
        public string FaturaAdSoyad { get; set; }
        public string VergiNumarasi { get; set; }
        public string VergiDairesi { get; set; }
        [Display(Name = "Şehir")]
        public int? SehirId { get; set; }
        public string Telefon2 { get; set; }

        public virtual Sehir Sehir { get; set; }
        public virtual List<EczaneGrup> EczaneGruplar { get; set; }
        public virtual List<EczaneUser> EczaneUserlar { get; set; }
    }
}