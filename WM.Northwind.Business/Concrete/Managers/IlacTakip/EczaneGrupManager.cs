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
using WM.Northwind.Entities.Concrete.Authorization;
using System.Linq.Expressions;
//using WM.Optimization.Abstract.Samples;

namespace WM.Northwind.Business.Concrete.Managers.IlacTakip
{
    public class EczaneGrupManager : IEczaneGrupService
    {
        private IEczaneGrupDal _eczaneGrupDal;
        private IEczaneUserDal _eczaneUserDal;
        private IGrupDal _grupDal;

        public EczaneGrupManager(IEczaneGrupDal eczaneGrupDal,
                                 IGrupDal grupDal,
                                 IEczaneUserDal eczaneUserDal)
        {
            _eczaneGrupDal = eczaneGrupDal;
            _grupDal = grupDal;
            _eczaneUserDal = eczaneUserDal;
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int eczaneGrupId)
        {
            _eczaneGrupDal.Delete(new EczaneGrup { Id = eczaneGrupId });
        }

        public EczaneGrup GetById(int eczaneGrupId)
        {
            return _eczaneGrupDal.Get(x => x.Id == eczaneGrupId);
        }
        [CacheAspect(typeof(MemoryCacheManager))]
        public List<EczaneGrup> GetList()
        {
            return _eczaneGrupDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(EczaneGrup eczaneGrup)
        {
            _eczaneGrupDal.Insert(eczaneGrup);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(EczaneGrup eczaneGrup)
        {
            _eczaneGrupDal.Update(eczaneGrup);
        }
        public EczaneGrupDetay GetDetayById(int eczaneGrupId)
        {
            return _eczaneGrupDal.GetDetay(x => x.Id == eczaneGrupId);
        }

        [CacheAspect(typeof(MemoryCacheManager))]
        public List<EczaneGrupDetay> GetDetaylar(Expression<Func<EczaneGrupDetay, bool>> filter = null)
        {
            return _eczaneGrupDal.GetDetayList( filter );
        }
        public List<EczaneGrup> GetListByUser(User user)
        {
            var eczaneUserlar = _eczaneUserDal.GetList();
            var eczaneId = eczaneUserlar.Where(e => e.UserId == user.Id)
            .Select(s => s.EczaneId).FirstOrDefault();


            return _eczaneGrupDal.GetList(w => w.EczaneId == eczaneId);
        }
        public List<EczaneGrupDetay> GetDetayListByUser(User user)
        {
            var eczaneUserlar = _eczaneUserDal.GetList();
            var eczaneIdler = eczaneUserlar.Where(e => e.UserId == user.Id)
                .Select(s => s.EczaneId).ToList();
            var grupIdler = _eczaneGrupDal.GetList(w =>eczaneIdler.Contains(w.EczaneId))
                .Select(s => s.GrupId).ToList();

            return _eczaneGrupDal.GetDetayList(w => grupIdler.Contains(w.GrupId));
        }
        public List<EczaneGrupDetay> GetMyDetayListByUser(User user)
        {
            var eczaneUserlar = _eczaneUserDal.GetList();
            var eczaneIdler = eczaneUserlar.Where(e => e.UserId == user.Id)
            .Select(s => s.EczaneId).ToList();


            return _eczaneGrupDal.GetDetayList(w => eczaneIdler.Contains(w.EczaneId));
        }
    }
}