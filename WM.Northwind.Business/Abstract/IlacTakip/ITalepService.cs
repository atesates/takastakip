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
    public interface ITalepService
    {
        Talep GetById(int talepId);
        List<Talep> GetList();
        //List<Talep> GetByCategory(int categoryId);
        void Insert(Talep talep);
        void Update(Talep talep);
        void Delete(int talepId);
        TalepDetay GetDetayById(int talepId);

        
        List<TalepDetay> GetListByEczaneGruplar(List<int> eczaneGrupIdler, List<int> grupIdler);
        List<TalepDetay> GetListByEczaneGrupId(int eczaneGrupId);
        List<TalepDetay> GetMyListByEczaneGrupId(int eczaneGrupId);
        List<Talep> GetListByUser(User user);
        List<TalepDetay> GetDetaylar(Expression<Func<TalepDetay, bool>> filter = null);

        List<Talep> GetMyListByEczaneGruplar(List<EczaneGrupDetay> eczaneGruplar);
        List<Talep> GetMyListByEczaneGruplar(List<EczaneGrup> eczaneGruplar);

        List<TalepDetay> GetMyDetayListByEczaneGruplar(List<EczaneGrupDetay> eczaneGruplar);
        List<TalepDetay> GetMyDetayListByEczaneGruplar(List<EczaneGrup> eczaneGruplar);
    }
}