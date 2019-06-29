using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;
using WM.Northwind.Entities.Concrete.Authorization;
using System.Linq.Expressions;

namespace WM.Northwind.Business.Abstract.IlacTakip
{
    public interface IEczaneGrupService
    {
        EczaneGrup GetById(int eczaneGrupId);
        List<EczaneGrup> GetList();
        //List<EczaneGrup> GetByCategory(int categoryId);
        void Insert(EczaneGrup eczaneGrup);
        void Update(EczaneGrup eczaneGrup);
        void Delete(int eczaneGrupId);
        EczaneGrupDetay GetDetayById(int eczaneGrupId);
        List<EczaneGrupDetay> GetDetaylar(Expression<Func<EczaneGrupDetay, bool>> filter = null);
        List<EczaneGrup> GetListByUser(User user);
        List<EczaneGrupDetay> GetDetayListByUser(User user);
        List<EczaneGrupDetay> GetMyDetayListByUser(User user);

    }
}