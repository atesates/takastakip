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
using WM.Northwind.Business.Abstract.Authorization;
using WM.Core.Aspects.PostSharp.TranstionAspects;
using WM.Core.Aspects.PostSharp.LogAspects;
using WM.Core.CrossCuttingConcerns.Logging.Log4Net.Logger;
using System.Linq.Expressions;
//using WM.Northwind.Entities.Concrete.Optimization.IlacTakip;
//using WM.Optimization.Abstract.Samples;

namespace WM.Northwind.Business.Concrete.Managers.IlacTakip
{
    public class AlimManager : IAlimService
    {
        #region ctor
        private IEczaneGrupService _eczaneGrupService;
        private ITeklifDal _teklifDal;
        private IEczaneUserService _eczaneUserService;
        private IAlimDal _alimDal;

        public AlimManager(IAlimDal alimDal,
                           ITeklifDal teklifDal,
                           IEczaneUserService eczaneUserService,
                           IEczaneGrupService eczaneGrupService)
        {
            _alimDal = alimDal;
            _eczaneUserService = eczaneUserService;
            _teklifDal = teklifDal;
            _eczaneGrupService = eczaneGrupService;
        }
        #endregion
        [LogAspect(typeof(DatabaseLogger))]

        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Delete(int alimId)
        {
            _alimDal.Delete(new Alim { Id = alimId });
        }

        public Alim GetById(int alimId)
        {
            return _alimDal.Get(x => x.Id == alimId);
        }

        [CacheAspect(typeof(MemoryCacheManager))]
        public List<Alim> GetList()
        {
            return _alimDal.GetList().Select(s => new Alim
            {
                AlimDurumId = s.AlimDurumId,
                AlimTarihi = s.AlimTarihi,
                EczaneGrupId = s.EczaneGrupId,
                GonderimTarihi = s.GonderimTarihi,
                ITStransferDurumId = s.ITStransferDurumId,
                Id = s.Id,
                Miktar = s.Miktar,
                TeklifId = s.TeklifId
            }).ToList();
        }
        [TransactionScopeAspect]
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Insert(Alim alim)
            {
                _alimDal.Insert(alim);
            }
        [CacheRemoveAspect(typeof(MemoryCacheManager))]
        public void Update(Alim alim)
        {
            _alimDal.Update(alim);
        }
        public AlimDetay GetDetayById(int alimId)
        {
            return _alimDal.GetDetay(x => x.Id == alimId);
        }

        [CacheAspect(typeof(MemoryCacheManager))]
        public List<AlimDetay> GetDetaylar(Expression<Func<AlimDetay, bool>> filter = null)
        {
            return _alimDal.GetDetayList(filter);
        }
        public List<AlimDetay> GetListByEczaneGruplar(List<EczaneGrupDetay> eczaneGrupDetaylar)
        {
            List<AlimDetay> alimDetay = _alimDal.GetDetayList();
            var alimIdler = alimDetay.Where(w => eczaneGrupDetaylar.Select(s => s.Id)
                .Contains(w.EczaneGrupId)).Select(s => s.Id);
            return alimDetay.Where(w => alimIdler.Contains(w.Id)).ToList();
        }
        public List<AlimDetay> GetListByTeklifId(int teklifId)
        {
            return _alimDal.GetDetayList(w => w.TeklifId == teklifId);
        }
        public List<Alim> GetListByTeklifler(List<int> teklifler)
        {
            return _alimDal.GetList(w => teklifler.Contains(w.TeklifId));

        }
        public List<AlimDetay> GetDetayListByTeklifler(List<int> teklifler)
        {
            return _alimDal.GetDetayList(w => teklifler.Contains(w.TeklifId));

        }
        public List<Alim> GetListByUser(User user)
        {
            var eczaneId = _eczaneUserService.GetListByUserId(user.Id).Select(s => s.EczaneId).FirstOrDefault();
            var teklifIdler = _teklifDal.GetList(w => w.TeklifiVerenEczaneGrupId == eczaneId).Select(s => s.Id);
            return _alimDal.GetList(w => teklifIdler.Contains(w.TeklifId));
        }

        public List<AlimDetay> GetMyListByEczaneGruplar(List<EczaneGrup> eczaneGruplar)
        {//alim yapan eczaneGrupId leri o eczaneye ait olan alımlar döner
            List<AlimDetay> alimDetay = _alimDal.GetDetayList();
            var alimIdler = alimDetay.Where(w => eczaneGruplar.Select(s => s.Id)
                .Contains(w.EczaneGrupId)).Select(s => s.Id);
            return alimDetay.Where(w => alimIdler.Contains(w.Id)).ToList();
        }

        public List<AlimDetay> GetMyListByEczaneGruplar(List<EczaneGrupDetay> eczaneGrupDetaylar)
        {//alim yapan eczaneGrupId leri o eczaneye ait olan alımlar döner
            List<AlimDetay> alimDetay = _alimDal.GetDetayList();
            var alimIdler = alimDetay.Where(w => eczaneGrupDetaylar.Select(s => s.Id)
                .Contains(w.EczaneGrupId)).Select(s => s.Id);
            return alimDetay.Where(w => alimIdler.Contains(w.Id)).ToList();
        }
    }
}