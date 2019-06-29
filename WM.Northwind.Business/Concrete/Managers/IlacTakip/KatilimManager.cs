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
    public class KatilimManager : IKatilimService
    {
        #region ctor
        private IEczaneGrupService _eczaneGrupService;
        private ITeklifDal _teklifDal;
        private IEczaneUserService _eczaneUserService;
        private IKatilimDal _katilimDal;

        public KatilimManager(IKatilimDal katilimDal,
                           ITeklifDal teklifDal,
                           IEczaneUserService eczaneUserService,
                           IEczaneGrupService eczaneGrupService)
        {
            _katilimDal = katilimDal;
            _eczaneUserService = eczaneUserService;
            _teklifDal = teklifDal;
            _eczaneGrupService = eczaneGrupService;
        }
        #endregion
     
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int katilimId)
        {
            _katilimDal.Delete(new Katilim { Id = katilimId });
        }

        public Katilim GetById(int katilimId)
        {
            return _katilimDal.Get(x => x.Id == katilimId);
        }
        [CacheAspect(typeof(MemoryCacheManager))]
        public List<Katilim> GetList()
        {
            return _katilimDal.GetList();
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(Katilim katilim)
        {
            _katilimDal.Insert(katilim);
        }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(Katilim katilim)
        {
            _katilimDal.Update(katilim);
        }
        public KatilimDetay GetDetayById(int katilimId)
        {
            return _katilimDal.GetDetay(x => x.Id == katilimId);
        }

        [CacheAspect(typeof(MemoryCacheManager))]
        public List<KatilimDetay> GetDetaylar()
        {
            return _katilimDal.GetDetayList();
        }

        [CacheAspect(typeof(MemoryCacheManager))]
        public List<KatilimDetay> GetDetaylar(Expression<Func<KatilimDetay, bool>> filter = null)
        {
            return _katilimDal.GetDetayList(filter);
        }
        public List<KatilimDetay> GetListByEczaneGruplar(List<EczaneGrupDetay> eczaneGrupDetaylar)
        {
            List<KatilimDetay> alimDetay = _katilimDal.GetDetayList();
            var alimIdler = alimDetay.Where(w => eczaneGrupDetaylar.Select(s => s.Id)
                .Contains(w.EczaneGrupId)).Select(s => s.Id);
            return alimDetay.Where(w => alimIdler.Contains(w.Id)).ToList();
        }
        public List<KatilimDetay> GetListByTeklifId(int teklifId)
        {
            return _katilimDal.GetDetayList(w => w.TalepId == teklifId);
        }
        public List<Katilim> GetListByTeklifler(List<int> teklifler)
        {
            return _katilimDal.GetList(w => teklifler.Contains(w.TalepId));

        }
        public List<KatilimDetay> GetDetayListByTeklifler(List<int> teklifler)
        {
            return _katilimDal.GetDetayList(w => teklifler.Contains(w.TalepId));

        }
        public List<Katilim> GetListByUser(User user)
        {
            var eczaneId = _eczaneUserService.GetListByUserId(user.Id).Select(s => s.EczaneId).FirstOrDefault();
            var teklifIdler = _teklifDal.GetList(w => w.TeklifiVerenEczaneGrupId == eczaneId).Select(s => s.Id);
            return _katilimDal.GetList(w => teklifIdler.Contains(w.TalepId));
        }

        public List<KatilimDetay> GetMyListByEczaneGruplar(List<EczaneGrup> eczaneGruplar)
        {//alim yapan eczaneGrupId leri o eczaneye ait olan alımlar döner
            List<KatilimDetay> alimDetay = _katilimDal.GetDetayList();
            var alimIdler = alimDetay.Where(w => eczaneGruplar.Select(s => s.Id)
                .Contains(w.EczaneGrupId)).Select(s => s.Id);
            return alimDetay.Where(w => alimIdler.Contains(w.Id)).ToList();
        }

        public List<KatilimDetay> GetMyListByEczaneGruplar(List<EczaneGrupDetay> eczaneGrupDetaylar)
        {//alim yapan eczaneGrupId leri o eczaneye ait olan alımlar döner
            List<KatilimDetay> alimDetay = _katilimDal.GetDetayList();
            var alimIdler = alimDetay.Where(w => eczaneGrupDetaylar.Select(s => s.Id)
                .Contains(w.EczaneGrupId)).Select(s => s.Id);
            return alimDetay.Where(w => alimIdler.Contains(w.Id)).ToList();
        }


    }
}