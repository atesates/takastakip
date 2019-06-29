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
    public class EfEczaneDal : EfEntityRepositoryBase<Eczane, IlacTakipContext>, IEczaneDal
    {
        public EczaneDetay GetDetay(Expression<Func<EczaneDetay, bool>> filter)
        {
            using (var ctx = new IlacTakipContext())
            {
                return ctx.Eczaneler
                    .Select(s => new EczaneDetay
                    {
                        SehirId = s.SehirId,
                        Adi = s.Adi,
                        Id=s.Id,
                        Telefon = s.Telefon,
                        Adres = s.Adres,
                        Email = s.Email,
                        EczaneGln = s.EczaneGln,
                        FaturaAdSoyad =s.FaturaAdSoyad,
                        VergiNumarasi = s.VergiNumarasi,
                        VergiDairesi =s.VergiDairesi,
                        SehirAdi=s.Sehir.Adi,
                        Telefon2 =s.Telefon2
                         
    }).SingleOrDefault(filter);
            }
        }
        public List<EczaneDetay> GetDetayList(Expression<Func<EczaneDetay, bool>> filter = null)
        {
            using (var ctx = new IlacTakipContext())
            {
                var liste = ctx.Eczaneler
                    .Select(s => new EczaneDetay
                    {
                        SehirId = s.SehirId,
                        Adi = s.Adi,
                        Id = s.Id,
                        Telefon = s.Telefon,
                        Adres = s.Adres,
                        Email = s.Email,
                        EczaneGln = s.EczaneGln,
                        FaturaAdSoyad = s.FaturaAdSoyad,
                        VergiNumarasi = s.VergiNumarasi,
                        VergiDairesi = s.VergiDairesi,
                        SehirAdi = s.Sehir.Adi,
                        Telefon2 = s.Telefon2
                    });

                return filter == null
                    ? liste.ToList()
                    : liste.Where(filter).ToList();
            }
        }
    }
}