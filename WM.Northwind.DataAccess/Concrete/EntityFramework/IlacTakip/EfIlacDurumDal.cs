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
    public class EfIlacDurumDal : EfEntityRepositoryBase<IlacDurum, IlacTakipContext>, IIlacDurumDal
    {
        public IlacDurumDetay GetDetay(Expression<Func<IlacDurumDetay, bool>> filter)
        {
            using (var ctx = new IlacTakipContext())
            {
                return ctx.IlacDurumlar
                    .Select(s => new IlacDurumDetay
                    {
                        
                    }).SingleOrDefault(filter);
            }
        }
        public List<IlacDurumDetay> GetDetayList(Expression<Func<IlacDurumDetay, bool>> filter = null)
        {
            using (var ctx = new IlacTakipContext())
            {
                var liste = ctx.IlacDurumlar
                    .Select(s => new IlacDurumDetay
                    {
                        
                    });

                return filter == null
                    ? liste.ToList()
                    : liste.Where(filter).ToList();
            }
        }
    }
}