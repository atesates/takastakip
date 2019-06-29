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
    public interface IIlacService
    {
        Ilac GetById(int ılacId);
        List<Ilac> GetList();
        //List<Ilac> GetByCategory(int categoryId);
        void Insert(Ilac ılac);
        void Update(Ilac ılac);
        void Delete(int ılacId);

    }
} 