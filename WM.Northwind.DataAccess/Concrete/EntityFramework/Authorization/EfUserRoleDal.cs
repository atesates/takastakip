using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WM.Core.DAL.EntityFramework;
using WM.Northwind.DataAccess.Abstract.Authorization;
using WM.Northwind.DataAccess.Concrete.EntityFramework.Contexts;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.Authorization;

namespace WM.Northwind.DataAccess.Concrete.EntityFramework.Authorization
{
    public class EfUserRoleDal : EfEntityRepositoryBase<UserRole, IlacTakipContext>, IUserRoleDal
    {
        public UserRoleDetay GetDetay(Expression<Func<UserRoleDetay, bool>> filter)
        {
            using (var ctx = new IlacTakipContext())
            {
                return ctx.UserRoles
                    .Select(s => new UserRoleDetay
                    {
                        Id = s.Id,
                        KullaniciAdi = s.User.UserName,
                        RoleName = s.Role.Name,
                        RoleId = s.RoleId,
                        UserId = s.UserId
                    }).SingleOrDefault(filter);
            }
        }

        public List<UserRoleDetay> GetDetayList(Expression<Func<UserRoleDetay, bool>> filter = null)
        {
            using (var ctx = new IlacTakipContext())
            {
                return filter == null
                   ? ctx.UserRoles
                    .Select(s => new UserRoleDetay
                    {
                        Id = s.Id,
                        KullaniciAdi = s.User.UserName,
                        RoleName = s.Role.Name,
                        RoleId = s.RoleId,
                        UserId = s.UserId
                    }).ToList()
                   : ctx.UserRoles
                    .Select(s => new UserRoleDetay
                    {
                        Id = s.Id,
                        KullaniciAdi = s.User.UserName,
                        RoleName = s.Role.Name,
                        RoleId = s.RoleId,
                        UserId = s.UserId
                    })
                    .Where(filter)
                    .ToList();
            }
        }
    }
}
