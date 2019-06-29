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
    public class IlacDurumManager : IIlacDurumService
    {
        private IIlacDurumDal _ılacDurumDal;

        public IlacDurumManager(IIlacDurumDal ılacDurumDal)
        {
            _ılacDurumDal = ılacDurumDal;
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int ılacDurumId)
        {
            _ılacDurumDal.Delete(new IlacDurum { Id = ılacDurumId });
        }

        public IlacDurum GetById(int ılacDurumId)
        {
            return _ılacDurumDal.Get(x => x.Id == ılacDurumId);
        }
        [CacheAspect(typeof(MemoryCacheManager))]
        public List<IlacDurum> GetList()
        {
            return _ılacDurumDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(IlacDurum ılacDurum)
        {
            _ılacDurumDal.Insert(ılacDurum);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(IlacDurum ılacDurum)
        {
            _ılacDurumDal.Update(ılacDurum);
        }
        public IlacDurumDetay GetDetayById(int ılacDurumId)
        {
            return _ılacDurumDal.GetDetay(x => x.Id == ılacDurumId);
        }

        [CacheAspect(typeof(MemoryCacheManager))]
        public List<IlacDurumDetay> GetDetaylar()
        {
            return _ılacDurumDal.GetDetayList();
        }

    }
}