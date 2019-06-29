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
    public class EfAlimDurumDal : EfEntityRepositoryBase<AlimDurum, IlacTakipContext>, IAlimDurumDal
    {
        public AlimDurumDetay GetDetay(Expression<Func<AlimDurumDetay, bool>> filter)
        {
            using (var ctx = new IlacTakipContext())
            {
                return ctx.AlimDurumlar
                    .Select(s => new AlimDurumDetay
                    {
                        
                    }).SingleOrDefault(filter);
            }
        }
        public List<AlimDurumDetay> GetDetayList(Expression<Func<AlimDurumDetay, bool>> filter = null)
        {
            using (var ctx = new IlacTakipContext())
            {
                var liste = ctx.AlimDurumlar
                    .Select(s => new AlimDurumDetay
                    {
                        
                    });

                return filter == null
                    ? liste.ToList()
                    : liste.Where(filter).ToList();
            }
        }
    }
}