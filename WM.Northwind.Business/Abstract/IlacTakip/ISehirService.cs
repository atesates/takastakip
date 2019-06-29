using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;
//using WM.Northwind.Entities.Concrete.Optimization.IlacTakip;

namespace WM.Northwind.Business.Abstract.IlacTakip
{
    public interface ISehirService
    {
        Sehir GetById(int sehirId);
        List<Sehir> GetList();
        //List<Sehir> GetByCategory(int categoryId);
        void Insert(Sehir sehir);
        void Update(Sehir sehir);
        void Delete(int sehirId);
                        
    }
} 