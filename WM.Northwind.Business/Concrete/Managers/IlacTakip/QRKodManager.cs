using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Business.Abstract;
using WM.Northwind.Business.Abstract.IlacTakip;
using WM.Northwind.DataAccess.Abstract.IlacTakip;
using WM.Core.Aspects.PostSharp.CacheAspects;
using WM.Core.CrossCuttingConcerns.Caching.Microsoft;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;
//using WM.Northwind.Entities.Concrete.Optimization.IlacTakip;
//using WM.Optimization.Abstract.Samples;

namespace WM.Northwind.Business.Concrete.Managers.IlacTakip
{
    public class QRKodManager : IQRKodService
    {
        private IQRKodDal _qRKodDal;

        public QRKodManager(IQRKodDal qRKodDal)
        {
            _qRKodDal = qRKodDal;
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int qRKodId)
        {
            _qRKodDal.Delete(new QRKod { Id = qRKodId });
        }

        public QRKod GetById(int qRKodId)
        {
            return _qRKodDal.Get(x => x.Id == qRKodId);
        }
         [CacheAspect(typeof(MemoryCacheManager))]
        public List<QRKod> GetList()
        {
            return _qRKodDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(QRKod qRKod)
        {
            _qRKodDal.Insert(qRKod);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(QRKod qRKod)
        {
            _qRKodDal.Update(qRKod);
        }
        public List<QRKod> GetListByAlimId(int alimId)
        {
            return _qRKodDal.GetList().Where(x => x.AlimId == alimId).ToList();

        }


    } 
}