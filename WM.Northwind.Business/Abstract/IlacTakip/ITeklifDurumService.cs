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
    public interface ITeklifDurumService
    {
        TeklifDurum GetById(int teklifDurumId);
        List<TeklifDurum> GetList();
        //List<TeklifDurum> GetByCategory(int categoryId);
        void Insert(TeklifDurum teklifDurum);
        void Update(TeklifDurum teklifDurum);
        void Delete(int teklifDurumId);
        TeklifDurumDetay GetDetayById(int teklifDurumId);
        List<TeklifDurumDetay> GetDetaylar();
    }
}