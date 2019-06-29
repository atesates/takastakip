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
    public class YayinlamaTurManager : IYayinlamaTurService
    {
        private IYayinlamaTurDal _yayinlamaTurDal;

        public YayinlamaTurManager(IYayinlamaTurDal yayinlamaTurDal)
        {
            _yayinlamaTurDal = yayinlamaTurDal;
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int yayinlamaTurId)
        {
            _yayinlamaTurDal.Delete(new YayinlamaTur { Id = yayinlamaTurId });
        }

        public YayinlamaTur GetById(int yayinlamaTurId)
        {
            return _yayinlamaTurDal.Get(x => x.Id == yayinlamaTurId);
        }
         [CacheAspect(typeof(MemoryCacheManager))]
        public List<YayinlamaTur> GetList()
        {
            return _yayinlamaTurDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(YayinlamaTur yayinlamaTur)
        {
            _yayinlamaTurDal.Insert(yayinlamaTur);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(YayinlamaTur yayinlamaTur)
        {
            _yayinlamaTurDal.Update(yayinlamaTur);
        }
                        

    } 
}