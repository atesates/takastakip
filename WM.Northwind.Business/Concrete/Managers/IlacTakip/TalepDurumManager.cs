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
    public class TalepDurumManager : ITalepDurumService
    {
        private ITalepDurumDal _talepDurumDal;

        public TalepDurumManager(ITalepDurumDal talepDurumDal)
        {
            _talepDurumDal = talepDurumDal;
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int talepDurumId)
        {
            _talepDurumDal.Delete(new TalepDurum { Id = talepDurumId });
        }

        public TalepDurum GetById(int talepDurumId)
        {
            return _talepDurumDal.Get(x => x.Id == talepDurumId);
        }
         [CacheAspect(typeof(MemoryCacheManager))]
        public List<TalepDurum> GetList()
        {
            return _talepDurumDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(TalepDurum talepDurum)
        {
            _talepDurumDal.Insert(talepDurum);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(TalepDurum talepDurum)
        {
            _talepDurumDal.Update(talepDurum);
        }
                        

    } 
}