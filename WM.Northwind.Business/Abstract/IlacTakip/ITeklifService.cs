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
    public interface ITeklifService
    {
        Teklif GetById(int teklifId);
        List<Teklif> GetList();
        //List<Teklif> GetByCategory(int categoryId);
        void Insert(Teklif teklif);
        void Update(Teklif teklif);
        void Delete(int teklifId);
        TeklifDetay GetDetayById(int teklifId);
        List<TeklifDetay> GetListByEczaneGruplar(List<int> eczaneGrupIdler, List<int> grupIdler);
        List<TeklifDetay> GetListByEczaneGrupId(int eczaneGrupId);
        List<TeklifDetay> GetMyListByEczaneGrupId(int eczaneGrupId);
        List<Teklif> GetListByUser(User user);
        List<TeklifDetay> GetDetaylar(Expression<Func<TeklifDetay, bool>> filter = null);
    
        List<Teklif> GetMyListByEczaneGruplar(List<EczaneGrupDetay> eczaneGruplar);
        List<Teklif> GetMyListByEczaneGruplar(List<EczaneGrup> eczaneGruplar);

        List<TeklifDetay> GetMyDetayListByEczaneGruplar(List<EczaneGrupDetay> eczaneGruplar);
        List<TeklifDetay> GetMyDetayListByEczaneGruplar(List<EczaneGrup> eczaneGruplar);
    }
}