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
    public class EfKatilimDal : EfEntityRepositoryBase<Katilim, IlacTakipContext>, IKatilimDal
    {
        public KatilimDetay GetDetay(Expression<Func<KatilimDetay, bool>> filter)
        {
            using (var ctx = new IlacTakipContext())
            {
                return ctx.Katilimlar
                    .Select(s => new KatilimDetay
                    {
                        EczaneGrupId = s.EczaneGrupId,                       
                        EczaneGrupAdi = s.EczaneGrup.Grup.Adi,
                        Id = s.Id,
                        IlacAdi = s.Talep.Ilac.Adi,
                        KatilimTarihi = s.KatilimTarihi,
                        Miktar=s.Miktar,
                        KatilimYapanEczaneAdi = s.EczaneGrup.Eczane.Adi,

                        Aciklama = s.Talep.Aciklama,
                        BitisTarihi = s.Talep.BitisTarihi,
                        DepoFiyati = s.Talep.DepoFiyati,
                        IlacId = s.Talep.Ilac.Id,
                        KayitTarihi = s.Talep.KayitTarihi,
                        Minimum = s.Talep.Minimum,
                        Maximum = s.Talep.Maximum,
                        TalepMiktari = s.Talep.TalepMiktari,
                        TalepVerenEczaneGrupId = s.Talep.TalepVerenEczaneGrupId,
                        TalepVerenEczaneGrupAdi = s.Talep.EczaneGrup.Grup.Adi,
                        TalepDurumId = s.Talep.TalepDurumId,
                        TalepDurumAdi = s.Talep.TalepDurum.Adi,
                        TalepId = s.TalepId,
                        TalepVerenEczaneAdi = s.Talep.EczaneGrup.Eczane.Adi,
                        ToplamKatilimMiktari = s.Talep.Katilimlar.Sum(m=>m.Miktar),
                        Kalan = s.Talep.TalepMiktari - s.Talep.Katilimlar.Sum(m => m.Miktar)

                    }).SingleOrDefault(filter);
            }
        }
        public List<KatilimDetay> GetDetayList(Expression<Func<KatilimDetay, bool>> filter = null)
        {
            using (var ctx = new IlacTakipContext())
            {
                var liste = ctx.Katilimlar
                    .Select(s => new KatilimDetay
                    {
                        EczaneGrupId = s.EczaneGrupId,
                        EczaneGrupAdi = s.EczaneGrup.Eczane.Adi,
                        Id = s.Id,
                        IlacAdi = s.Talep.Ilac.Adi,
                        KatilimTarihi = s.KatilimTarihi,
                        Miktar = s.Miktar,
                        TalepId = s.TalepId,
                        KatilimYapanEczaneAdi = s.EczaneGrup.Eczane.Adi,
                        Aciklama = s.Talep.Aciklama,
                        BitisTarihi = s.Talep.BitisTarihi,
                        DepoFiyati = s.Talep.DepoFiyati,
                        IlacId = s.Talep.Ilac.Id,
                        KayitTarihi = s.Talep.KayitTarihi,
                        Minimum = s.Talep.Minimum,
                        Maximum = s.Talep.Maximum,
                        TalepMiktari = s.Talep.TalepMiktari,
                        TalepVerenEczaneGrupId = s.Talep.TalepVerenEczaneGrupId,
                        TalepVerenEczaneGrupAdi = s.Talep.EczaneGrup.Grup.Adi,
                        TalepDurumId = s.Talep.TalepDurumId,
                        TalepDurumAdi = s.Talep.TalepDurum.Adi,
                        ToplamKatilimMiktari = s.Talep.Katilimlar.Sum(m=>m.Miktar),
                        TalepVerenEczaneAdi = s.Talep.EczaneGrup.Eczane.Adi,
                        Kalan = s.Talep.TalepMiktari - s.Talep.Katilimlar.Sum(m => m.Miktar)
                    });

                return filter == null
                    ? liste.ToList()
                    : liste.Where(filter).ToList();
            }
        }
    }
}