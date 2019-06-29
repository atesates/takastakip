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
    public interface IAlimService
    {
        Alim GetById(int alimId);
        List<Alim> GetList();
        //List<Alim> GetByCategory(int categoryId);
        void Insert(Alim alim);
        void Update(Alim alim);
        void Delete(int alimId);
        AlimDetay GetDetayById(int alimId);
        List<AlimDetay> GetDetaylar(Expression<Func<AlimDetay, bool>> filter = null);
        List<AlimDetay> GetListByEczaneGruplar(List<EczaneGrupDetay> eczaneGrupDetaylar);
        List<AlimDetay> GetListByTeklifId(int teklfiId);
        List<Alim> GetListByUser(User user);
        List<Alim> GetListByTeklifler(List<int> teklifler);
        List<AlimDetay> GetDetayListByTeklifler(List<int> teklifler);
        List<AlimDetay> GetMyListByEczaneGruplar(List<EczaneGrup> eczaneGruplar);
        List<AlimDetay> GetMyListByEczaneGruplar(List<EczaneGrupDetay> eczaneGrupDetaylar);

    }
}