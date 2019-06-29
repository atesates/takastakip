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
    public class TeklifTurManager : ITeklifTurService
    {
        private ITeklifTurDal _teklifTurDal;

        public TeklifTurManager(ITeklifTurDal teklifTurDal)
        {
            _teklifTurDal = teklifTurDal;
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int teklifTurId)
        {
            _teklifTurDal.Delete(new TeklifTur { Id = teklifTurId });
        }

        public TeklifTur GetById(int teklifTurId)
        {
            return _teklifTurDal.Get(x => x.Id == teklifTurId);
        }
         [CacheAspect(typeof(MemoryCacheManager))]
        public List<TeklifTur> GetList()
        {
            return _teklifTurDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(TeklifTur teklifTur)
        {
            _teklifTurDal.Insert(teklifTur);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(TeklifTur teklifTur)
        {
            _teklifTurDal.Update(teklifTur);
        }
                        

    } 
}