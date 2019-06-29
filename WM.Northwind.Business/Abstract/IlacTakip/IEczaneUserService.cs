using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.Authorization;
using WM.Northwind.Entities.Concrete.IlacTakip;
//using WM.Northwind.Entities.Concrete.Optimization.IlacTakip;

namespace WM.Northwind.Business.Abstract.IlacTakip
{
    public interface IEczaneUserService
    {
        EczaneUser GetById(int eczaneUserId);
        List<EczaneUser> GetList();
        //List<EczaneUser> GetByCategory(int categoryId);
        void Insert(EczaneUser eczaneUser);
        void Update(EczaneUser eczaneUser);
        void Delete(int eczaneUserId);
        EczaneUserDetay GetDetayById(int eczaneUserId);
        List<EczaneUserDetay> GetDetaylar();
        List<EczaneUser> GetListByUserId(int userId);
        List<EczaneUser> GetListByUser(User user);

        List<EczaneUserDetay> GetDetayListByUser(User user);


    }
}