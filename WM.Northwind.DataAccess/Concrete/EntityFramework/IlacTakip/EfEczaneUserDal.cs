using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WM.Core.DAL.EntityFramework;
using WM.Northwind.DataAccess.Abstract.IlacTakip;
using WM.Northwind.DataAccess.Concrete.EntityFramework.Contexts;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;

namespace WM.Northwind.DataAccess.Concrete.EntityFramework.IlacTakip
{
    public class EfEczaneUserDal : EfEntityRepositoryBase<EczaneUser, IlacTakipContext>, IEczaneUserDal
    {
        public EczaneUserDetay GetDetay(Expression<Func<EczaneUserDetay, bool>> filter)
        {
            using (var ctx = new IlacTakipContext())
            {
                return ctx.EczaneUserlar
                    .Select(s => new EczaneUserDetay
                    {
                        UserId = s.UserId,
                        //EczaneAdi = s.EczaneAdi,
                        //EczaneId = s..EczaneId,
                        UserAdi = s.User.FirstName,
                        
                    }).SingleOrDefault(filter);
            }
        }
        public List<EczaneUserDetay> GetDetayList(Expression<Func<EczaneUserDetay, bool>> filter = null)
        {
            using (var ctx = new IlacTakipContext())
            {
                var liste = ctx.EczaneUserlar
                    .Select(s => new EczaneUserDetay
                    {
                        UserId = s.UserId,
                        //EczaneAdi = s.Eczane.Adi,
                        //EczaneId = s.EczaneId,
                        UserAdi = s.User.FirstName,
                        
                    });

                return filter == null
                    ? liste.ToList()
                    : liste.Where(filter).ToList();
            }
        }
    }
}