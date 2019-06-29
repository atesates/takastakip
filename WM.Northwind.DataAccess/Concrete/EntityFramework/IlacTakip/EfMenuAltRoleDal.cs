using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WM.Core.DAL.EntityFramework;
using WM.Northwind.DataAccess.Abstract.IlacTakip;
using WM.Northwind.DataAccess.Concrete.EntityFramework.Contexts;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.Northwind.DataAccess.Concrete.EntityFramework.EczaneNobet
{
    public class EfMenuAltRoleDal : EfEntityRepositoryBase<MenuAltRole, IlacTakipContext>, IMenuAltRoleDal
    {
        public List<MenuAltRoleDetay> GetDetayList(Expression<Func<MenuAltRoleDetay, bool>> filter = null)
        {
            using (var ctx = new IlacTakipContext())
            {

                return filter == null
              ? ctx.MenuAltRoles
                   .Select(s => new MenuAltRoleDetay
                   {
                       Id = s.Id,
                       MenuAltId = s.MenuAltId,
                       MenuId = s.MenuAlt.MenuId,
                       RoleId = s.RoleId,
                       MenuAltAdi = s.MenuAlt.LinkText,
                       RolAdi = s.Role.Name
                   }).ToList()
                   : ctx.MenuAltRoles
                   .Select(s => new MenuAltRoleDetay
                   {
                       Id = s.Id,
                       MenuAltId = s.MenuAltId,
                       MenuId = s.MenuAlt.MenuId,
                       RoleId = s.RoleId,
                       MenuAltAdi = s.MenuAlt.LinkText,
                       RolAdi = s.Role.Name
                   })
                   .Where(filter)
                   .ToList();
            }
        }
        public MenuAltRoleDetay GetDetay(Expression<Func<MenuAltRoleDetay, bool>> filter = null)
        {
            using (var ctx = new IlacTakipContext())
            {
                return ctx.MenuAltRoles
                    .Select(s => new MenuAltRoleDetay
                    {
                        Id = s.Id,
                        MenuAltId = s.MenuAltId,
                        MenuId = s.MenuAlt.MenuId,
                        RoleId = s.RoleId,
                        MenuAltAdi = s.MenuAlt.LinkText,
                        RolAdi = s.Role.Name
                    }).SingleOrDefault(filter);
            }
        }
   
    }
}
