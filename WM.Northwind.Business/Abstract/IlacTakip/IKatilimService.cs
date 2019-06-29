using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.Authorization;
using WM.Northwind.Entities.Concrete.IlacTakip;
//using WM.Northwind.Entities.Concrete.Optimization.IlacTakip;

namespace WM.Northwind.Business.Abstract.IlacTakip
{
    public interface IKatilimService
    {
        Katilim GetById(int katilimId);
        List<Katilim> GetList();
        //List<Katilim> GetByCategory(int categoryId);
        void Insert(Katilim katilim);
        void Update(Katilim katilim);
        void Delete(int katilimId);
        KatilimDetay GetDetayById(int katilimId);
        List<KatilimDetay> GetDetaylar(Expression<Func<KatilimDetay, bool>> filter = null);
        List<KatilimDetay> GetListByEczaneGruplar(List<EczaneGrupDetay> eczaneGrupDetaylar);
        List<KatilimDetay> GetListByTeklifId(int teklfiId);
        List<Katilim> GetListByUser(User user);
        List<Katilim> GetListByTeklifler(List<int> teklifler);
        List<KatilimDetay> GetDetayListByTeklifler(List<int> teklifler);
        List<KatilimDetay> GetMyListByEczaneGruplar(List<EczaneGrup> eczaneGruplar);
        List<KatilimDetay> GetMyListByEczaneGruplar(List<EczaneGrupDetay> eczaneGrupDetaylar);
    }
}