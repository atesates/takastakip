using System.Collections.Generic;
using System.Linq;
using WM.Core.DAL.EntityFramework;
using WM.Northwind.DataAccess.Abstract.Authorization;
using WM.Northwind.DataAccess.Concrete.EntityFramework.Contexts;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.Authorization;

namespace WM.Northwind.DataAccess.Concrete.EntityFramework.Authorization
{
    public class EfUserDal : EfEntityRepositoryBase<User, IlacTakipContext>, IUserDal
    {
        public List<UserRoleItem> GetUserRoles(User user)
        {
            using (var context = new IlacTakipContext())
            {
                var result = (from ur in context.UserRoles
                              where ur.UserId == user.Id
                              select new UserRoleItem
                              {
                                  RoleId = ur.RoleId,
                                  RoleName = ur.Role.Name
                              })
                              .ToList();
                return result;
            }
        }
    }
}
