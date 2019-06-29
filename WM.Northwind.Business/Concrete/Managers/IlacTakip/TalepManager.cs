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
//using WM.Northwind.Entities.Concrete.Optimization.IlacTakip;
//using WM.Optimization.Abstract.Samples;

namespace WM.Northwind.Business.Concrete.Managers.IlacTakip
{
    public class TalepManager : ITalepService
    {
        #region ctor
        private ITalepDal _talepDal;
        private IEczaneGrupService _eczaneGrupService;
        private IEczaneUserService _eczaneUserService;
        private IKatilimService _katilimService;

        public TalepManager(ITalepDal talepDal,
                           IEczaneGrupService eczaneGrupService,
                           IKatilimService katilimService,
                           IEczaneUserService eczaneUserService)
        {
            _eczaneGrupService = eczaneGrupService;
            _talepDal = talepDal;
            _katilimService = katilimService;
            _eczaneUserService = eczaneUserService;
        }
        #endregion
        public TalepManager(ITalepDal talepDal)
        {
            _talepDal = talepDal;
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int talepId)
        {
            _talepDal.Delete(new Talep { Id = talepId });
        }

        public Talep GetById(int talepId)
        {
            return _talepDal.Get(x => x.Id == talepId);
        }
        [CacheAspect(typeof(MemoryCacheManager))]
        public List<Talep> GetList()
        {
            return _talepDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(Talep talep)
        {
            _talepDal.Insert(talep);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(Talep talep)
        {
            _talepDal.Update(talep);
        }
        public TalepDetay GetDetayById(int talepId)
        {
            return _talepDal.GetDetay(x => x.Id == talepId);
        }

        [CacheAspect(typeof(MemoryCacheManager))]
        public List<TalepDetay> GetDetaylar(Expression<Func<TalepDetay, bool>> filter = null)
        {
            return _talepDal.GetDetayList();
        }

        public List<TalepDetay> GetListByEczaneGrupId(int eczaneGrupId)
        {//grubundaki tüm teklifler
            var gruplar = _eczaneGrupService.GetList().Where(w => w.Id == eczaneGrupId).Select(s => s.GrupId);
            var gruplardakiTumEczaneGruplar = _eczaneGrupService.GetList().Where(w => gruplar.Contains(w.GrupId)).Select(s => s.Id);
            List<TalepDetay> talepDetaylar = _talepDal.GetDetayList();

            return talepDetaylar.Where(w => gruplardakiTumEczaneGruplar.Contains(w.TalepVerenEczaneGrupId)).ToList();

        }
        public List<TalepDetay> GetListByEczaneGruplar(List<int> eczaneGrupIdler, List<int> grupIdler)
        {//kendi eczaneGrupIdleri olmayacak ama kendi grubundaki grupIdlerin eczaneGrupIdleri olacak

            var teklifteGosterilecekEczaneGrupIdler = _eczaneGrupService.GetList()
                .Where(w => !eczaneGrupIdler.Contains(w.Id) && grupIdler.Contains(w.GrupId))
                .Select(s => s.Id).ToList();

            List<TalepDetay> teklifDetayList = _talepDal.GetDetayList();

            return teklifDetayList.Where(w => teklifteGosterilecekEczaneGrupIdler
            .Contains(w.TalepVerenEczaneGrupId)).ToList();

        }
        public List<TalepDetay> GetMyListByEczaneGrupId(int eczaneGrupId)
        {//kendi teklifleri

            List<TalepDetay> teklifDetay = _talepDal.GetDetayList();

            return teklifDetay.Where(w => w.TalepVerenEczaneGrupId == eczaneGrupId).ToList();

        }
        public List<Talep> GetListByUser(User user)
        {
            //var eczaneId = _eczaneUserDal.GetList(e => e.UserId == user.Id)
            //.Select(s => s.EczaneId).FirstOrDefault();
            var eczaneId = _eczaneUserService.GetListByUserId(user.Id).Select(s => s.EczaneId).FirstOrDefault();
            return _talepDal.GetList(w => w.TalepVerenEczaneGrupId == eczaneId);
        }
        public List<Talep> GetMyListByEczaneGruplar(List<EczaneGrupDetay> eczaneGruplar)
        {//teklifVerenEczaneGrupId leri o eczaneye ait olan teklifler döner
            List<Talep> teklif = _talepDal.GetList();
            var teklifIdler = teklif.Where(w => eczaneGruplar.Select(s => s.Id)
                .Contains(w.TalepVerenEczaneGrupId)).Select(s => s.Id);
            return teklif.Where(w => teklifIdler.Contains(w.Id)).ToList();
        }
        public List<Talep> GetMyListByEczaneGruplar(List<EczaneGrup> eczaneGruplar)
        {//teklifVerenEczaneGrupId leri o eczaneye ait olan teklifler döner
            List<Talep> teklif = _talepDal.GetList();
            var teklifIdler = teklif.Where(w => eczaneGruplar.Select(s => s.Id)
                .Contains(w.TalepVerenEczaneGrupId)).Select(s => s.Id);
            return teklif.Where(w => teklifIdler.Contains(w.Id)).ToList();
        }
        public List<TalepDetay> GetMyDetayListByEczaneGruplar(List<EczaneGrupDetay> eczaneGruplar)
        {//teklifVerenEczaneGrupId leri o eczaneye ait olan teklifler döner
            List<TalepDetay> teklifDetay = _talepDal.GetDetayList().Where(w => eczaneGruplar.Select(s => s.Id)
                .Contains(w.TalepVerenEczaneGrupId)).ToList();
            var teklifIdler = teklifDetay.Select(s => s.Id);
            return teklifDetay.Where(w => teklifIdler.Contains(w.Id)).ToList();
        }
        public List<TalepDetay> GetMyDetayListByEczaneGruplar(List<EczaneGrup> eczaneGruplar)
        {//teklifVerenEczaneGrupId leri o eczaneye ait olan teklifler döner
            List<TalepDetay> teklifDetay = _talepDal.GetDetayList();
            var teklifIdler = teklifDetay.Where(w => eczaneGruplar.Select(s => s.Id)
                .Contains(w.TalepVerenEczaneGrupId)).Select(s => s.Id);
            return teklifDetay.Where(w => teklifIdler.Contains(w.Id)).ToList();
        }

    }
}