using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM.Northwind.Entities.ComplexTypes.IlacTakip
{
    public class LoginItem
    {
        //[Required(ErrorMessage ="Bu alan zorunludur...")]
        [Display(Name = "Eposta")]
       // [EmailAddress(ErrorMessage = "Lütfen geçerli bir Eposta adresi giriniz.")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Paorla zorunludur.")]
        [Display(Name = "Parola")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public int UserId { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
