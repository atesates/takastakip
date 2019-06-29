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
    public interface IIlacDurumService
    {
        IlacDurum GetById(int ılacDurumId);
        List<IlacDurum> GetList();
        //List<IlacDurum> GetByCategory(int categoryId);
        void Insert(IlacDurum ilacDurum);
        void Update(IlacDurum ilacDurum);
        void Delete(int ilacDurumId);
        IlacDurumDetay GetDetayById(int ilacDurumId);
        List<IlacDurumDetay> GetDetaylar();
    }
}