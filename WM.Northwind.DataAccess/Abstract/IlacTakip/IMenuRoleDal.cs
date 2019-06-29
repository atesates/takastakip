using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WM.Core.DAL;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.Northwind.DataAccess.Abstract.IlacTakip
{
    public interface IMenuRoleDal : IEntityRepository<MenuRole>, IEntityDetayRepository<MenuRoleDetay>
    {
        //List<MenuRoleDetay> GetDetayList();
    }
}
