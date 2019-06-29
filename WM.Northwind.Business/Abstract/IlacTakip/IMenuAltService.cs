using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.Northwind.Business.Abstract.IlacTakip
{
    public interface IMenuAltService
    {
        MenuAlt GetById(int menuAltId);
        List<MenuAlt> GetList();
        List<MenuAlt> GetByMenuId(int menuAltId);
        void Insert(MenuAlt menuAlt);
        void Update(MenuAlt menuAlt);
        void Delete(int menuAltId);
    }
}
