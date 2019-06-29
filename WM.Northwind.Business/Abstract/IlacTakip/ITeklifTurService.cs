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
    public interface ITeklifTurService
    {
        TeklifTur GetById(int teklifTurId);
        List<TeklifTur> GetList();
        //List<TeklifTur> GetByCategory(int categoryId);
        void Insert(TeklifTur teklifTur);
        void Update(TeklifTur teklifTur);
        void Delete(int teklifTurId);
                        
    }
} 