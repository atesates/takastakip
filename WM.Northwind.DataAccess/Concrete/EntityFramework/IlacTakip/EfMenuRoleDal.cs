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
    public class EfMenuRoleDal : EfEntityRepositoryBase<MenuRole, IlacTakipContext>, IMenuRoleDal
    {
        public MenuRoleDetay GetDetay(Expression<Func<MenuRoleDetay, bool>> filter)
        {
            using (var ctx = new IlacTakipContext())
            {
                return ctx.MenuRoles
                    .Select(s => new MenuRoleDetay
                    {
                        Id = s.Id,
                        MenuId = s.MenuId,
                        RoleId = s.RoleId,
                        MenuAdi = s.Menu.LinkText,
                        RolAdi = s.Role.Name
                    }).SingleOrDefault(filter);
            }
        }
        public List<MenuRoleDetay> GetDetayList(Expression<Func<MenuRoleDetay, bool>> filter = null)
        {
            using (var ctx = new IlacTakipContext())
            {
               
                      return filter == null
                   ? ctx.MenuRoles

                        .Select(s=> new MenuRoleDetay
                        {
                            Id = s.Id,
                            MenuId = s.MenuId,
                            RoleId = s.RoleId,
                            MenuAdi = s.Menu.LinkText,
                            RolAdi = s.Role.Name
                        }).ToList()
                        : ctx.MenuRoles
                         .Select(s => new MenuRoleDetay
                         {
                             Id = s.Id,
                             MenuId = s.MenuId,
                             RoleId = s.RoleId,
                             MenuAdi = s.Menu.LinkText,
                             RolAdi = s.Role.Name
                         })
                         .Where(filter)
                         .ToList()
                        ;
            }
        }
    }
}
