﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;
using WM.Northwind.Entities.Concrete.IlacTakip;
using System.ComponentModel.DataAnnotations;

namespace WM.Northwind.Entities.Concrete.Authorization
{
    public class User : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Kulllanıcı Adı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        public string UserName { get; set; }

        [Display(Name = "Parolası")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        [StringLength(50, ErrorMessage = "Şifre en az 6(altı) karakter olmalıdır..!", MinimumLength = 6)]
        public string Password { get; set; }

        [Display(Name = "Adı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        public string FirstName { get; set; }

        [Display(Name = "Soyadı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        public string LastName { get; set; }

        [Display(Name = "E-Posta")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz..!")]
        [RegularExpression(@"^[a-zA-Z0-9_&.-]+@[a-zA-Z0-9.-]+\.[a-zA-Z0-9]{2,5}$", ErrorMessage = "Lütfen geçerli bir e-posta adresi yazınız..!")]
        public string Email { get; set; }
        public virtual List<EczaneUser> EczaneUserlar { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
