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
using WM.Northwind.Entities.Concrete.Authorization;
using WM.Northwind.Entities.Concrete.IlacTakip;
//using WM.Northwind.Entities.Concrete.Optimization.IlacTakip;
//using WM.Optimization.Abstract.Samples;

namespace WM.Northwind.Business.Concrete.Managers.IlacTakip
{
    public class IlacManager : IIlacService
    {
        private IIlacDal _ilacDal;
        private ITeklifService _teklifService;

        public IlacManager(IIlacDal ilacDal)
        {
            _ilacDal = ilacDal;
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int ilacId)
        {
            _ilacDal.Delete(new Ilac { Id = ilacId });
        }

        public Ilac GetById(int ilacId)
        {
            return _ilacDal.Get(x => x.Id == ilacId);
        }
        [CacheAspect(typeof(MemoryCacheManager))]
        public List<Ilac> GetList()
        {
            return _ilacDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(Ilac ilac)
        {
            _ilacDal.Insert(ilac);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(Ilac ilac)
        {
            _ilacDal.Update(ilac);
        }
        

    }
}