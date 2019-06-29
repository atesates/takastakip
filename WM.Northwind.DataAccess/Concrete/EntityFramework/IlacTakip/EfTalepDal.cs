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
    public class EfTalepDal : EfEntityRepositoryBase<Talep, IlacTakipContext>, ITalepDal
    {
        public TalepDetay GetDetay(Expression<Func<TalepDetay, bool>> filter)
        {
            using (var ctx = new IlacTakipContext())
            {
                return ctx.Talepler
                    .Select(s => new TalepDetay
                    {
                        Aciklama = s.Aciklama,
                        BitisTarihi = s.BitisTarihi,
                        DepoFiyati = s.DepoFiyati,                     
                        IlacAdi = s.Ilac.Adi,
                        IlacId = s.IlacId,
                        Id = s.Id,
                        KayitTarihi = s.KayitTarihi,
                        Minimum = s.Minimum,
                        Maximum = s.Maximum,
                        TalepMiktari = s.TalepMiktari,
                        TalepVerenEczaneGrupId = s.TalepVerenEczaneGrupId,
                        TalepVerenEczaneGrupAdi = s.EczaneGrup.Grup.Adi,
                        TalepDurumId = s.TalepDurumId,
                        TalepDurumAdi = s.TalepDurum.Adi,
                        TalepVerenEczaneAdi = s.EczaneGrup.Eczane.Adi,
                    }).SingleOrDefault(filter);
            }
        }
        public List<TalepDetay> GetDetayList(Expression<Func<TalepDetay, bool>> filter = null)
        {
            using (var ctx = new IlacTakipContext())
            {
                var liste = ctx.Talepler
                    .Select(s => new TalepDetay
                    {
                        Aciklama = s.Aciklama,
                        BitisTarihi = s.BitisTarihi,
                        DepoFiyati = s.DepoFiyati,
                        IlacAdi = s.Ilac.Adi,
                        IlacId = s.IlacId,
                        Id = s.Id,
                        KayitTarihi = s.KayitTarihi,
                        Minimum = s.Minimum,
                        Maximum = s.Maximum,
                        TalepMiktari = s.TalepMiktari,
                        TalepVerenEczaneGrupId = s.TalepVerenEczaneGrupId,
                        TalepVerenEczaneGrupAdi = s.EczaneGrup.Grup.Adi,
                        TalepDurumId = s.TalepDurumId,
                        TalepDurumAdi = s.TalepDurum.Adi,
                        TalepVerenEczaneAdi = s.EczaneGrup.Eczane.Adi,

                    });

                return filter == null
                    ? liste.ToList()
                    : liste.Where(filter).ToList();
            }
        }
    }
}