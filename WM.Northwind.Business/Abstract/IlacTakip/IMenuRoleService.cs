using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.Northwind.Business.Abstract.IlacTakip
{
    public interface IMenuRoleService
    {
        MenuRole GetById(int menuRoleId);
        List<MenuRole> GetList();
        List<MenuRole> GetByRoleId(int roleId);
        void Insert(MenuRole menuRole);
        void Update(MenuRole menuRole);
        void Delete(int menuRoleId);

        List<MenuRoleDetay> GetDetayList(int roleId);
        
    }
}
