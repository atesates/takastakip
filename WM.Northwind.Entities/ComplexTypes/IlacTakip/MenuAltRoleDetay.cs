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
    public class MenuAltRoleDetay : IComplexType
    {
        public int Id { get; set; }
        public int MenuAltId { get; set; }
        public int MenuId { get; set; }
        public int RoleId { get; set; }
        [Display(Name = "Menü Alt")]
        public string MenuAltAdi { get; set; }
        [Display(Name = "Rol")]
        public string RolAdi { get; set; }

      //  public List<MenuRole> MenuRoles { get; set; }
       // public MenuRole MenuRole { get; set; }

    }

}
