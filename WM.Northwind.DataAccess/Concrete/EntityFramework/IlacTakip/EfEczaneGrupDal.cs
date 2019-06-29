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
    public class EfEczaneGrupDal : EfEntityRepositoryBase<EczaneGrup, IlacTakipContext>, IEczaneGrupDal
    {
        public EczaneGrupDetay GetDetay(Expression<Func<EczaneGrupDetay, bool>> filter)
        {
            using (var ctx = new IlacTakipContext())
            {
                return ctx.EczaneGruplar
                    .Select(s => new EczaneGrupDetay
                    {
                        Adres = s.Eczane.Adres,
                        Email = s.Eczane.Email,
                        EczaneGln = s.Eczane.EczaneGln,
                        FaturaAdSoyad = s.Eczane.FaturaAdSoyad,
                        EczaneId = s.EczaneId,
                        EczaneAdi = s.Eczane.Adi,
                        GrupId = s.GrupId,
                        GrupAdi = s.Grup.Adi,
                        Id = s.Id,
                        Sehir = s.Eczane.Sehir.Adi,
                        Telefon = s.Eczane.Telefon,
                        Telefon2 = s.Eczane.Telefon2,
                        VergiDairesi = s.Eczane.VergiDairesi,
                        VergiNumarasi = s.Eczane.VergiNumarasi,

                    }).SingleOrDefault(filter);
            }
        }
        public List<EczaneGrupDetay> GetDetayList(Expression<Func<EczaneGrupDetay, bool>> filter = null)
        {
            using (var ctx = new IlacTakipContext())
            {
                var liste = ctx.EczaneGruplar
                    .Select(s => new EczaneGrupDetay
                    {
                        Adres = s.Eczane.Adres,
                        Email = s.Eczane.Email,
                        EczaneGln = s.Eczane.EczaneGln,
                        FaturaAdSoyad = s.Eczane.FaturaAdSoyad,
                        EczaneId = s.EczaneId,
                        EczaneAdi = s.Eczane.Adi,
                        GrupId = s.GrupId,
                        GrupAdi = s.Grup.Adi,
                        Id = s.Id,
                        Sehir = s.Eczane.Sehir.Adi,
                        Telefon = s.Eczane.Telefon,
                        Telefon2 = s.Eczane.Telefon2,
                        VergiDairesi = s.Eczane.VergiDairesi,
                        VergiNumarasi = s.Eczane.VergiNumarasi,

                    });

                return filter == null
                    ? liste.ToList()
                    : liste.Where(filter).ToList();
            }
        }
    }
}