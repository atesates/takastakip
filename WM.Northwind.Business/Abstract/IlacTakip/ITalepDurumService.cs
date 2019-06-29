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
    public interface ITalepDurumService
    {
        TalepDurum GetById(int talepDurumId);
        List<TalepDurum> GetList();
        //List<TalepDurum> GetByCategory(int categoryId);
        void Insert(TalepDurum talepDurum);
        void Update(TalepDurum talepDurum);
        void Delete(int talepDurumId);
                        
    }
} 