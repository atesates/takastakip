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
    public class EfTeklifDurumDal : EfEntityRepositoryBase<TeklifDurum, IlacTakipContext>, ITeklifDurumDal
    {
        public TeklifDurumDetay GetDetay(Expression<Func<TeklifDurumDetay, bool>> filter)
        {
            using (var ctx = new IlacTakipContext())
            {
                return ctx.TeklifDurumlar
                    .Select(s => new TeklifDurumDetay
                    {
                        
                    }).SingleOrDefault(filter);
            }
        }
        public List<TeklifDurumDetay> GetDetayList(Expression<Func<TeklifDurumDetay, bool>> filter = null)
        {
            using (var ctx = new IlacTakipContext())
            {
                var liste = ctx.TeklifDurumlar
                    .Select(s => new TeklifDurumDetay
                    {
                        
                    });

                return filter == null
                    ? liste.ToList()
                    : liste.Where(filter).ToList();
            }
        }
    }
}