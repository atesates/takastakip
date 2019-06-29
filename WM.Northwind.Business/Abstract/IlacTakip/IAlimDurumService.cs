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
    public interface IAlimDurumService
    {
        AlimDurum GetById(int alimDurumId);
        List<AlimDurum> GetList();
        //List<AlimDurum> GetByCategory(int categoryId);
        void Insert(AlimDurum alimDurum);
        void Update(AlimDurum alimDurum);
        void Delete(int alimDurumId);
                        AlimDurumDetay GetDetayById(int alimDurumId);
                                   List <AlimDurumDetay> GetDetaylar();
    }
} 