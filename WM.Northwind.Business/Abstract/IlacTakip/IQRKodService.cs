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
    public interface IQRKodService
    {
        QRKod GetById(int qRKodId);
        List<QRKod> GetList();
        //List<QRKod> GetByCategory(int categoryId);
        void Insert(QRKod qRKod);
        void Update(QRKod qRKod);
        void Delete(int qRKodId);
        List<QRKod> GetListByAlimId(int alimId);


    }
} 