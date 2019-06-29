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
    public class TeklifDurumManager : ITeklifDurumService
    {
        private ITeklifDurumDal _teklifDurumDal;

        public TeklifDurumManager(ITeklifDurumDal teklifDurumDal)
        {
            _teklifDurumDal = teklifDurumDal;
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int teklifDurumId)
        {
            _teklifDurumDal.Delete(new TeklifDurum { Id = teklifDurumId });
        }

        public TeklifDurum GetById(int teklifDurumId)
        {
            return _teklifDurumDal.Get(x => x.Id == teklifDurumId);
        }
         [CacheAspect(typeof(MemoryCacheManager))]
        public List<TeklifDurum> GetList()
        {
            return _teklifDurumDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(TeklifDurum teklifDurum)
        {
            _teklifDurumDal.Insert(teklifDurum);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(TeklifDurum teklifDurum)
        {
            _teklifDurumDal.Update(teklifDurum);
        }
                                  public TeklifDurumDetay GetDetayById(int teklifDurumId)
            {
                return _teklifDurumDal.GetDetay(x => x.Id == teklifDurumId);
            }
            
            [CacheAspect(typeof(MemoryCacheManager))]
            public List<TeklifDurumDetay> GetDetaylar()
            {
                return _teklifDurumDal.GetDetayList();
            }

    } 
}