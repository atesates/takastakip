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
    public interface IEczaneService
    {
        Eczane GetById(int eczaneId);
        Eczane GetByGln(string eczaneGln);
        List<Eczane> GetList();
        //List<Eczane> GetByCategory(int categoryId);
        void Insert(Eczane eczane);
        void Update(Eczane eczane);
        void Delete(int eczaneId);
        EczaneDetay GetDetayById(int eczaneId);
        List<EczaneDetay> GetDetaylar(Expression<Func<EczaneDetay, bool>> filter = null);
        List<Eczane> GetListByUser(User user);

    }
}