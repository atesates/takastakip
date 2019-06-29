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
    public class LogManager : ILogService
    {
        private ILogDal _logDal;

        public LogManager(ILogDal logDal)
        {
            _logDal = logDal;
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int logId)
        {
            _logDal.Delete(new Log { Id = logId });
        }

        public Log GetById(int logId)
        {
            return _logDal.Get(x => x.Id == logId);
        }
         [CacheAspect(typeof(MemoryCacheManager))]
        public List<Log> GetList()
        {
            return _logDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(Log log)
        {
            _logDal.Insert(log);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(Log log)
        {
            _logDal.Update(log);
        }
                        

    } 
}