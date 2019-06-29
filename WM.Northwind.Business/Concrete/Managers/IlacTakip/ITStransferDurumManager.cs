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
    public class ITStransferDurumManager : IITStransferDurumService
    {
        private IITStransferDurumDal _ıTStransferDurumDal;

        public ITStransferDurumManager(IITStransferDurumDal ıTStransferDurumDal)
        {
            _ıTStransferDurumDal = ıTStransferDurumDal;
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int ıTStransferDurumId)
        {
            _ıTStransferDurumDal.Delete(new ITStransferDurum { Id = ıTStransferDurumId });
        }

        public ITStransferDurum GetById(int ıTStransferDurumId)
        {
            return _ıTStransferDurumDal.Get(x => x.Id == ıTStransferDurumId);
        }
         [CacheAspect(typeof(MemoryCacheManager))]
        public List<ITStransferDurum> GetList()
        {
            return _ıTStransferDurumDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(ITStransferDurum ıTStransferDurum)
        {
            _ıTStransferDurumDal.Insert(ıTStransferDurum);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(ITStransferDurum ıTStransferDurum)
        {
            _ıTStransferDurumDal.Update(ıTStransferDurum);
        }
                        

    } 
}