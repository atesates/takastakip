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
    public class AlimDurumManager : IAlimDurumService
    {
        private IAlimDurumDal _alimDurumDal;

        public AlimDurumManager(IAlimDurumDal alimDurumDal)
        {
            _alimDurumDal = alimDurumDal;
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int alimDurumId)
        {
            _alimDurumDal.Delete(new AlimDurum { Id = alimDurumId });
        }

        public AlimDurum GetById(int alimDurumId)
        {
            return _alimDurumDal.Get(x => x.Id == alimDurumId);
        }
        [CacheAspect(typeof(MemoryCacheManager))]
        public List<AlimDurum> GetList()
        {
            return _alimDurumDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(AlimDurum alimDurum)
        {
            _alimDurumDal.Insert(alimDurum);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(AlimDurum alimDurum)
        {
            _alimDurumDal.Update(alimDurum);
        }
        public AlimDurumDetay GetDetayById(int alimDurumId)
        {
            return _alimDurumDal.GetDetay(x => x.Id == alimDurumId);
        }

        [CacheAspect(typeof(MemoryCacheManager))]
        public List<AlimDurumDetay> GetDetaylar()
        {
            return _alimDurumDal.GetDetayList();
        }

    }
}