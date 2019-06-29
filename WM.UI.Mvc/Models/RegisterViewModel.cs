using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using WM.Northwind.Entities.Concrete.Authorization;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;

namespace WM.UI.Mvc.Models
{
    public class RegisterViewModel
    {
        public User User { get; set; }
        public Eczane Eczane { get; set; }
        public Grup Grup { get; set; }
        public int GrupId { get; set; }

    }

}