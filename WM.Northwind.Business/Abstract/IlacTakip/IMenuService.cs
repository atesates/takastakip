using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.Northwind.Business.Abstract.IlacTakip
{
    public interface IMenuService
    {
        Menu GetById(int menuId);
        List<Menu> GetList();
        //List<Menu> GetByRoleId(int roleId);
        void Insert(Menu menu);
        void Update(Menu menu);
        void Delete(int menuId);
    }
}
