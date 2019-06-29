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
    public class MenuRoleDetay : IComplexType
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public int RoleId { get; set; }
        [Display(Name = "Menü")]
        public string MenuAdi { get; set; }
        [Display(Name = "Rol")]
        public string RolAdi { get; set; }
        //public List<MenuAltRole> MenuAltRoles { get; set; }

    }

}
