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
    public interface ILogService
    {
        Log GetById(int logId);
        List<Log> GetList();
        //List<Log> GetByCategory(int categoryId);
        void Insert(Log log);
        void Update(Log log);
        void Delete(int logId);
                        
    }
} 