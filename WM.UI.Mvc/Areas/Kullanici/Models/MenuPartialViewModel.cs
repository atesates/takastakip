using System.Collections.Generic;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.UI.Mvc.Areas.Kullanici.Models
{
    public class MenuPartialViewModel
    {
       
        public List<Menu> Menuler { get; internal set; }
        public List<MenuAlt> MenuAltlar { get; internal set; }
        public List<MenuAlt> MenuAltlarTumu { get; internal set; }
    }
}