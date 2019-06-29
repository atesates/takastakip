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
    public interface IGrupService
    {
        Grup GetById(int grupId);
        List<Grup> GetList();
        //List<Grup> GetByCategory(int categoryId);
        void Insert(Grup grup);
        void Update(Grup grup);
        void Delete(int grupId);
        List<Grup> GetListByUser(User user);


    }
} 