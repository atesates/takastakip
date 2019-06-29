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
    public class SehirManager : ISehirService
    {
        private ISehirDal _sehirDal;

        public SehirManager(ISehirDal sehirDal)
        {
            _sehirDal = sehirDal;
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int sehirId)
        {
            _sehirDal.Delete(new Sehir { Id = sehirId });
        }

        public Sehir GetById(int sehirId)
        {
            return _sehirDal.Get(x => x.Id == sehirId);
        }
         [CacheAspect(typeof(MemoryCacheManager))]
        public List<Sehir> GetList()
        {
            return _sehirDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(Sehir sehir)
        {
            _sehirDal.Insert(sehir);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(Sehir sehir)
        {
            _sehirDal.Update(sehir);
        }
                        

    } 
}