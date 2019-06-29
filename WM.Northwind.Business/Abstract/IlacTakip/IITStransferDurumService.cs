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
    public interface IITStransferDurumService
    {
        ITStransferDurum GetById(int ıTStransferDurumId);
        List<ITStransferDurum> GetList();
        //List<ITStransferDurum> GetByCategory(int categoryId);
        void Insert(ITStransferDurum ıTStransferDurum);
        void Update(ITStransferDurum ıTStransferDurum);
        void Delete(int ıTStransferDurumId);
                        
    }
} 